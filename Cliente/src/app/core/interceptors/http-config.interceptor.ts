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
                switch (e.status) {
                    case 0:
                        this.router.navigate(['/error/failed-error']);
                        break;
                    case 201:
                        this.router.navigate(['/error/confirm-error']);
                        break;
                    case 401:
                        this.router.navigate(['/error/401']);
                        break;
                    case 404:
                        this.router.navigate(['/error/404']);
                        break;
                    case 500:
                        this.router.navigate(['/error/500']);
                        break;
                }
                return throwError(e);
            }));
    }

}