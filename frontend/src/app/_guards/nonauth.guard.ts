import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

export const nonauthGuard: CanActivateFn = (route, state) => {
  const toastr = inject(ToastrService);
  const router = inject(Router)

  if (localStorage.getItem('token') === null) {
    return true;
  } else {
    toastr.warning('Not allowed.');
    router.navigate(['/profile']);
    return false;
  }
};
