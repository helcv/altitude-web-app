import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../_services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const toastr = inject(ToastrService);
  const router = inject(Router)
  const authService = inject(AuthService)

  const isAdmin = authService.getRoleFromToken() === 'Admin' ? true : false;
  if(isAdmin)
    return true
  else {
    toastr.warning('Not allowed.');
    router.navigate(['/']);
    return false
  }
  
};
