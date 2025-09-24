import { ApplicationRef, createComponent, Injectable, Injector, Type } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DialogService {
  constructor(
    private appRef: ApplicationRef,
    private injector: Injector
  ) {}

  open<T extends object>(
    component: Type<T>,
    inputs: Partial<T>,
    action$?: Observable<unknown> // optional observable to run on confirm
  ): Observable<'confirm' | 'cancel'> {
    const subject = new Subject<'confirm' | 'cancel'>();

    const componentRef = createComponent(component, {
      environmentInjector: this.appRef.injector,
      elementInjector: this.injector,
    });

    Object.assign(componentRef.instance, inputs);

    this.appRef.attachView(componentRef.hostView);
    document.body.appendChild(componentRef.location.nativeElement);

    // Handle confirm
    (componentRef.instance as any).confirm?.subscribe(() => {
      if (action$) {
        // set loading flag if the component has it
        if ('loading' in componentRef.instance) {
          (componentRef.instance as any).loading = true;
        }

        action$.subscribe({
          next: () => {
            subject.next('confirm');
          },
          error: (err) => {
            subject.error(err);
          },
          complete: () => {
            subject.complete();
            this.close(componentRef);
          },
        });
      } else {
        subject.next('confirm');
        subject.complete();
        this.close(componentRef);
      }
    });

    // Handle cancel
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
