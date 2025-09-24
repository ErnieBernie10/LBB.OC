import { ApplicationRef, createComponent, Injectable, Injector, Type } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DialogService {
  constructor(
    private appRef: ApplicationRef,
    private injector: Injector
  ) {}

  open<T extends object>(component: Type<T>, inputs: Partial<T>): Observable<'confirm' | 'cancel'> {
    const subject = new Subject<'confirm' | 'cancel'>();

    // Dynamically create the component instance
    const componentRef = createComponent(component, {
      environmentInjector: this.appRef.injector,
      elementInjector: this.injector,
    });

    // Apply inputs to the component
    Object.assign(componentRef.instance, inputs);

    // Attach component to Angular's change detection
    this.appRef.attachView(componentRef.hostView);

    // Add to DOM
    const domElem = componentRef.location.nativeElement;
    document.body.appendChild(domElem);

    // Hook into outputs
    (componentRef.instance as any).confirm?.subscribe(() => {
      subject.next('confirm');
      subject.complete();
      this.close(componentRef);
    });

    (componentRef.instance as any).cancel?.subscribe(() => {
      subject.next('cancel');
      subject.complete();
      this.close(componentRef);
    });

    return subject.asObservable();
  }

  private close<T>(componentRef: any) {
    this.appRef.detachView(componentRef.hostView);
    componentRef.destroy();
  }
}
