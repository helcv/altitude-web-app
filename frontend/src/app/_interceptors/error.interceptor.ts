import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router)
  const toastr = inject(ToastrService)

  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch (error.status) {
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = {state: {error: error.error.detail}};
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          
          default:
            break;
        }
      }
      throw error;
    })
  );
};
