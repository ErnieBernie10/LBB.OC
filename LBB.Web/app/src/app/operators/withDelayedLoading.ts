import { distinctUntilChanged, map, Observable, of, switchMap, timer } from 'rxjs';

/**
 * Creates an Observable that delays showing loading state until a specified delay has passed.
 * @param source$ - Source Observable of loading state
 * @param delay - Delay in milliseconds before showing loading state (default: 200)
 * @returns Observable<boolean> that represents the delayed loading state
 */
export function withDelayedLoading(source$: Observable<boolean>, delay = 200): Observable<boolean> {
  return source$.pipe(
    distinctUntilChanged(),
    switchMap((isLoading) => {
      if (!isLoading) {
        return of(false);
      }
      return timer(delay).pipe(map(() => isLoading));
    })
  );
}
