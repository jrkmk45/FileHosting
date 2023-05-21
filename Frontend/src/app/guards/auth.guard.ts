import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { SnackbarService } from '../services/snackbar.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService,  
    private router: Router,
    private snackbar: SnackbarService ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot) {
    if (this.authService.isUserLoggedIn()) {
      return true;
    }
    else {
      this.router.navigate(['/login'])
      this.snackbar.showMessage("Увійдіть в обліковий запис!");
      return false;
    }
  }
  
}
