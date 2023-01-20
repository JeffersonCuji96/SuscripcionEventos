import { HttpInterceptor, HttpRequest, HttpEvent, HttpHandler } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from "src/app/modules/auth/services/auth.service";

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {

    constructor(private authService: AuthService, private router: Router) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const re = /Access/gi;
        if (req.url.search(re) === -1) {
            req = req.clone({
                withCredentials: false,
                setHeaders: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.authService.getToken()}`
                }
            });
        }
        return next.handle(req).pipe(
            catchError(e => {
                return throwError(e)
            }));
    }

}