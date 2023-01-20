import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccessDto } from 'src/app/core/models/accessDto';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthenticationJwt } from 'src/app/core/models/authenticationJwt';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  readonly urlApi = environment.urlHost;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  login(usuario: AccessDto): Observable<AuthenticationJwt> {
    return this.http.post<AuthenticationJwt>(this.urlApi + "api/Access", usuario);
  }
  setCookieService(response: any) {
    this.cookieService.set('test', response.JwtToken, response.DaysExpireToken, '/');
  }
  setIdUserLocalStorage(id:number){
    localStorage.setItem("client",id.toString())
  }
  checkAutentication(): boolean {
    return this.cookieService.check('test');
  }
  getToken(): string {
    return this.cookieService.get("test");
  }
  getIdUserLocalStorage(): number {
    var userId=Number(localStorage.getItem("client"));
    if(isNaN(userId)){
      return 0;
    }
    return userId;
  }

}
