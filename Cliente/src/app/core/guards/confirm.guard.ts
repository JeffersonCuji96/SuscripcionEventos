import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmGuard implements CanActivate {

  constructor(private router: Router) { }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const token = route.url[1].path;
    const test = /^[A-Za-z0-9]+$/.test(token);
    if (token.length === 64 && test === true) {
      return true;
    }
    this.router.navigate(['/error/401']);
    return false;
  }

}
