import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { FrontendLogger } from '../../logging/frontend-logger.service';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const logger = inject(FrontendLogger);
  const token = auth.accessToken();
  const requestToSend = token && !request.url.includes('/api/auth/')
    ? request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      })
    : request;

  return next(requestToSend).pipe(
    catchError((error: unknown) => {
      logger.error('HTTP request failed', {
        method: request.method,
        url: request.url,
        status: getStatus(error),
        statusText: getStatusText(error),
      }, error);
      return throwError(() => error);
    }),
  );
};

function getStatus(error: unknown): number | undefined {
  return isHttpError(error) ? error.status : undefined;
}

function getStatusText(error: unknown): string | undefined {
  return isHttpError(error) ? error.statusText : undefined;
}

function isHttpError(error: unknown): error is { status: number; statusText: string } {
  return typeof error === 'object' && error !== null
    && 'status' in error
    && 'statusText' in error;
}
