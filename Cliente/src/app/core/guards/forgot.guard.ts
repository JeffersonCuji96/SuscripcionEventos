import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ForgotGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const token = route.url[1].path;
    const test = /^[A-Za-z0-9]+$/.test(token);
    if (token.length === 64 && test === true) {
      this.authService.checkToken({Token:token}).subscribe(
        (res: any) => {
          if (!res) {
            this.router.navigate(['/error/recovery-error']);
            return false;
          }
        }
      );
      return true;
    }
    this.router.navigate(['/error/401'])
    return false;
  }
}
