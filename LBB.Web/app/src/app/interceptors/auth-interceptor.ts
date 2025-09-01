import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, finalize, of, shareReplay, switchMap } from 'rxjs';

// Track a single in-flight refresh operation and share its result with all waiting requests.
let refreshInFlight$: import('rxjs').Observable<void> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const withAuth = (r: typeof req, token: string) =>
    r.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });

  const isTokenExpiringSoon = (thresholdMs = 60_000) => {
    // If we have no token yet, don't attempt a refresh here.
    if (!authService.token) return false;
    if (!authService.expiration) return false;
    return Date.now() + thresholdMs >= authService.expiration.getTime();
  };

  // Do not trigger refresh for the refresh endpoint itself to avoid loops
  const isRefreshRequest = req.url.includes('/refresh');

  // If this is the refresh request, just attach the current token (if any) and forward
  if (isRefreshRequest) {
    const forwarded = authService.token ? withAuth(req, authService.token) : req;
    return next(forwarded);
  }

  // If token is not expiring soon, attach it and continue
  if (!isTokenExpiringSoon()) {
    return next(withAuth(req, authService.token));
  }

  // Token is expiring soon: ensure a single refresh request and queue others until it completes
  if (!refreshInFlight$) {
    refreshInFlight$ = authService.refreshToken().pipe(
      // We only need completion; errors are handled below
      switchMap(() => of(void 0)),
      shareReplay(1),
      finalize(() => {
        refreshInFlight$ = null;
      })
    );
  }

  return refreshInFlight$.pipe(
    // After refresh, attach the new token and continue the original request
    switchMap(() => next(withAuth(req, authService.token))),
    // If refresh fails, forward the original request without modifying it (or you can handle logout)
    catchError(() => next(req))
  );
};
