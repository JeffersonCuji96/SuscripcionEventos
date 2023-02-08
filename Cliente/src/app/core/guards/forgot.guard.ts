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
    if (token === '' || token === null || !token.match(/[a-zA-Z0-9]+$/)) {
      this.router.navigate(['/auth/login'])
      return false;
    }
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
}
