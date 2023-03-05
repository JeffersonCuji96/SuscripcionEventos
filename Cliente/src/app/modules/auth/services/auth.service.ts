import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccessDto } from 'src/app/core/models/accessDto';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthenticationJwt } from 'src/app/core/models/authenticationJwt';
import { CookieService } from 'ngx-cookie-service';
import { TokenValidViewModel } from 'src/app/core/models/view-models/tokenValidViewModel';
import { TokenPasswordViewModel } from 'src/app/core/models/view-models/tokenPasswordViewModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  readonly urlApi = environment.urlHost;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  login(usuario: AccessDto): Observable<AuthenticationJwt> {
    return this.http.post<AuthenticationJwt>(this.urlApi + "api/Access", usuario);
  }
  setCookieService(response: any,check:boolean) {
    if(!check){
      this.cookieService.set("test", response.JwtToken,{path:"/",secure:true});
    }else{
      this.cookieService.set("test", response.JwtToken,{path:"/",secure:true,expires:response.DaysExpireToken});
    }
  }
  setIdUserLocalStorage(id:number){
    localStorage.setItem("client",id.toString())
  }
  setFullNameLocalStorage(fullname:string){
    localStorage.setItem("fullname",fullname)
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
  getFullName():string{
    return localStorage.getItem("fullname");
  }
  checkToken(token:TokenValidViewModel):Observable<boolean>{
    return this.http.post<boolean>(this.urlApi + "api/User/CheckToken", token);
  }
  changePassword(tokenPassViewModel:TokenPasswordViewModel):Observable<any>{
    return this.http.post<any>(this.urlApi + "api/User/ChangePassword", tokenPassViewModel);
  }
  confirmEmail(token:TokenValidViewModel):Observable<boolean>{
    return this.http.post<boolean>(this.urlApi + "api/User/ConfirmEmail",token);
  }
  removeSesion(): void {
    this.cookieService.delete("test","/");
    localStorage.removeItem("client");
    localStorage.removeItem("fullname");
  }
}
