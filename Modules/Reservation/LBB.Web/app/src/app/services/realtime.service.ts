import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';

export interface HubEvent {
  topic: string;
  payload: any;
}

@Injectable({
  providedIn: 'root',
})
export class RealtimeService implements OnDestroy {
  private hubConnection?: signalR.HubConnection;
  private readonly hubUrl = '/reservation/realtime';

  // Stream of all incoming events
  private eventSubject = new Subject<HubEvent>();
  public events$ = this.eventSubject.asObservable();

  // Optional: expose connection state
  private connectionStateSubject = new BehaviorSubject<boolean>(false);
  public connected$ = this.connectionStateSubject.asObservable();

  constructor() {}

  async start(): Promise<void> {
    if (this.hubConnection) return; // already started

    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(this.hubUrl).withAutomaticReconnect().build();

    this.registerEventHandlers();

    try {
      await this.hubConnection.start();
      console.log('✅ Connected to EventHub');
      this.connectionStateSubject.next(true);
    } catch (err) {
      console.error('❌ Error connecting to EventHub', err);
      this.connectionStateSubject.next(false);
    }
  }

  async stop(): Promise<void> {
    if (!this.hubConnection) return;
    await this.hubConnection.stop();
    this.connectionStateSubject.next(false);
    this.hubConnection = undefined;
  }

  async subscribe(topic: string): Promise<void> {
    if (!this.hubConnection) return;
    await this.hubConnection.invoke('Subscribe', topic);
    console.log(`Subscribed to topic: ${topic}`);
  }

  async unsubscribe(topic: string): Promise<void> {
    if (!this.hubConnection) return;
    await this.hubConnection.invoke('Unsubscribe', topic);
    console.log(`Unsubscribed from topic: ${topic}`);
  }

  private registerEventHandlers(): void {
    this.hubConnection?.on('EventReceived', (topic: string, payload: any) => {
      console.log(`📩 Event received [${topic}]`, payload);
      this.eventSubject.next({ topic, payload });
    });
  }

  ngOnDestroy(): void {
    this.stop();
  }
}
