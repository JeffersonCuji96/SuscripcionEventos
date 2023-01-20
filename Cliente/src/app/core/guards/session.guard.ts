import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Helpers } from 'src/app/helpers/helper';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionGuard implements CanActivate {
  constructor(private router: Router, private authService: AuthService, private helper: Helpers) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      return this.checkSession();
  }
  checkSession(): boolean {
    try {
      const authenticity: boolean = this.authService.checkAutentication();
      if (!authenticity) {
        this.router.navigate(['/', 'auth'])
      }
      return authenticity
    } catch (e) {
      this.helper.swalShow("<h4>Acceso no autorizado</h4>", e, "error")
      return false
    }
  }
}
