import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
    return this.http.get<any>(this.urlApi+"/api/User/GetCurrentDate");
  }
  checkPhone(phone: string): Observable<boolean> {
    return this.http.get<boolean>(this.urlApi+"api/User/CheckPhone/"+phone);
  }
}
