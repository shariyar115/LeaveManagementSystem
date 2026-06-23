import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

/** Surfaces backend error payloads ({ error: "..." }) as toast notifications. */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const detail =
        err.error?.error ??
        (typeof err.error === 'string' ? err.error : null) ??
        (err.status === 0
          ? 'Cannot reach the API. Make sure the backend is running.'
          : 'An unexpected error occurred.');

      messageService.add({ severity: 'error', summary: 'Error', detail, life: 5000 });
      return throwError(() => err);
    }),
  );
};
