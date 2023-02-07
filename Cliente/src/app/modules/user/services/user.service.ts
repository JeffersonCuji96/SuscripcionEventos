import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UsuarioDto } from 'src/app/core/models/usuarioDto';
import { UserEmailViewModel } from 'src/app/core/models/view-models/userEmailViewModel';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  readonly urlApi = environment.urlHost;
  constructor(private http: HttpClient) { }

  checkEmail(email: string): Observable<boolean> {
    return this.http.get<boolean>(this.urlApi+"api/User/CheckEmail/"+email);
  }
  getCurrentDate():Observable<any>{
    return this.http.get<any>(this.urlApi+"api/User/GetCurrentDate");
  }
  checkPhone(phone: string): Observable<boolean> {
    return this.http.get<boolean>(this.urlApi+"api/User/CheckPhone/"+phone);
  }
  register(user: UsuarioDto): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/User", user);
  }
  recoveryAccess(userEmail:UserEmailViewModel){
    return this.http.post<any>(this.urlApi + "api/User/RecoveryAccess", userEmail);
  }
}
