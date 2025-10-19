import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map } from 'rxjs/operators';

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
  private startPromise?: Promise<void>;
  private subscribedTopics = new Set<string>();

  // Stream of all incoming events
  private eventSubject = new Subject<HubEvent>();
  public events$ = this.eventSubject.asObservable();

  // Optional: expose connection state
  private connectionStateSubject = new BehaviorSubject<boolean>(false);
  public connected$ = this.connectionStateSubject.asObservable();

  constructor() {
    // Automatically start the connection when the service is instantiated
    this.ensureStarted();
  }

  /**
   * Subscribe to a topic and return an observable filtered to that topic's events.
   * Similar to sessionService.getSession() pattern.
   * Automatically ensures connection is started before subscribing.
   */
  public subscribeToTopic<T = any, R = any>(topic: string, identifier: R): Observable<T> {
    const t = this.getTopic(topic, identifier);
    this.ensureStarted().then(() => {
      if (!this.subscribedTopics.has(t)) {
        this.subscribe(t);
        this.subscribedTopics.add(t);
      }
    });

    return this.events$.pipe(
      filter((event) => event.topic === t),
      map((event) => event.payload as T)
    );
  }

  private getTopic(topic: string, identifier: any): string {
    if (typeof identifier === 'string') {
      return `${topic}.${identifier}`;
    } else if (typeof identifier === 'number') {
      return `${topic}.${identifier}`;
    } else {
      return topic;
    }
  }

  private async ensureStarted(): Promise<void> {
    if (this.startPromise) {
      return this.startPromise;
    }
    this.startPromise = this.start();
    return this.startPromise;
  }

  private async start(): Promise<void> {
    if (this.hubConnection) return; // already started

    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(this.hubUrl).withAutomaticReconnect().build();

    // Register event handlers BEFORE starting and only ONCE
    this.hubConnection.on('EventReceived', (topic: string, payload: any) => {
      console.log(`📩 Event received [${topic}]`, payload);
      this.eventSubject.next({ topic, payload });
    });

    // Handle reconnection events
    this.hubConnection.onreconnecting(() => {
      console.log('🔄 Reconnecting to EventHub...');
      this.connectionStateSubject.next(false);
    });

    this.hubConnection.onreconnected(async () => {
      console.log('✅ Reconnected to EventHub');
      this.connectionStateSubject.next(true);
      // Re-subscribe to all topics after reconnection
      for (const topic of this.subscribedTopics) {
        await this.subscribe(topic);
      }
    });

    this.hubConnection.onclose(() => {
      console.log('❌ Connection closed');
      this.connectionStateSubject.next(false);
    });

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

    // Remove all event handlers before stopping
    this.hubConnection.off('EventReceived');

    await this.hubConnection.stop();
    this.connectionStateSubject.next(false);
    this.hubConnection = undefined;
    this.subscribedTopics.clear();
    this.startPromise = undefined;
  }

  private async subscribe(topic: string): Promise<void> {
    if (!this.hubConnection) return;
    await this.hubConnection.invoke('Subscribe', topic);
    console.log(`Subscribed to topic: ${topic}`);
  }

  async unsubscribe(topic: string): Promise<void> {
    if (!this.hubConnection) return;
    await this.hubConnection.invoke('Unsubscribe', topic);
    this.subscribedTopics.delete(topic);
    console.log(`Unsubscribed from topic: ${topic}`);
  }

  ngOnDestroy(): void {
    this.stop();
  }
}
