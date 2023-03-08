import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserEmailViewModel } from 'src/app/core/models/view-models/userEmailViewModel';
import { UserPasswordViewModel } from 'src/app/core/models/view-models/userPasswordViewModel';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PerfilService {

  readonly urlApi = environment.urlHost;
  constructor(private http: HttpClient) { }
  
  updateEmail(userEmail: UserEmailViewModel): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/User/UpdateEmail", userEmail);
  }

  updatePassword(userPass: UserPasswordViewModel): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/User/UpdatePassword", userPass);
  }
}
