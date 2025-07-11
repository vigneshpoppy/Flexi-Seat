import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthServiceService } from '../auth-service.service';
 // Ensure this exi

@Injectable({
  providedIn: 'root',
})
export class  authGuard implements CanActivate {
  constructor(private authService: AuthServiceService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const expectedRoles = route.data['roles'] as string[]; // e.g., ['admin']
    const userRole = this.authService.getRoles();

    if (!this.authService.isAuthenticated()) {
      return this.router.createUrlTree(['/login']);
    }

    if (expectedRoles && !expectedRoles.includes(userRole || '')) {
      return this.router.createUrlTree(['/unauthorized']); // Optional: Create unauthorized page
    }

    return true;
  }
}