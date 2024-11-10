import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
  const toastr = inject(ToastrService);
  const router = inject(Router)

  if (localStorage.getItem('token') !== null) {
    return true;
  } else {
    toastr.warning('Please log in to access this page.');
    router.navigate(['/login']);
    return false;
  }
};