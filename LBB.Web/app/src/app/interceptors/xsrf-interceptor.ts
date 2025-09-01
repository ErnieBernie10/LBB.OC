import { HttpInterceptorFn } from '@angular/common/http';

export const xsrfInterceptor: HttpInterceptorFn = (req, next) => {
  // Only add CSRF token for non-GET/HEAD/OPTIONS
  if (['GET', 'HEAD', 'OPTIONS'].includes(req.method.toUpperCase())) {
    return next(req);
  }

  // Find the Orchard antiforgery cookie (__orchantiforgery_<tenantId>)
  const cookie = document.cookie.split('; ').find((row) => row.toLowerCase().startsWith('__orchantiforgery_'));

  if (cookie) {
    const [cookieName, cookieValue] = cookie.split('=');
    const cloned = req.clone({
      setHeaders: { [cookieName]: decodeURIComponent(cookieValue) },
    });
    return next(cloned);
  }

  return next(req);
};
