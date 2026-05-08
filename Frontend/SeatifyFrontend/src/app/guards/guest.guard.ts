import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const guestGuard: CanActivateFn = (): boolean | UrlTree => {
  const authService = inject(AuthService)
  const router = inject(Router)

  if (!authService.isLoggedIn()) {
    return true
  }

  return router.createUrlTree(['/dashboard'])
};