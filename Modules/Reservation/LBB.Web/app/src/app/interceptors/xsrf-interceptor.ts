import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const xsrfInterceptor: HttpInterceptorFn = (req, next) => {
  const csrfTokenService = inject(AuthService);
  // Only add CSRF token for non-GET/HEAD/OPTIONS
  if (['GET', 'HEAD', 'OPTIONS'].includes(req.method.toUpperCase())) {
    return next(req);
  }

  const cloned = req.clone({
    setHeaders: {
      RequestVerificationToken: csrfTokenService.getXsrfToken(),
    },
    withCredentials: true,
  });
  return next(cloned);
};
