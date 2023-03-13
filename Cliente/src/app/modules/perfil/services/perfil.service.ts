import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { PersonaDto } from 'src/app/core/models/personaDto';
import { FilePhotoViewModel } from 'src/app/core/models/view-models/filePhotoViewModel';
import { UserEmailViewModel } from 'src/app/core/models/view-models/userEmailViewModel';
import { UserPasswordViewModel } from 'src/app/core/models/view-models/userPasswordViewModel';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PerfilService {

  readonly urlApi = environment.urlHost;
  private issueChanges = new Subject<boolean>();
  constructor(private http: HttpClient) { }

  issueChanges$ = this.issueChanges.asObservable();
  emitChanges(value: boolean) {
    this.issueChanges.next(value);
  }
  
  updateEmail(userEmail: UserEmailViewModel): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/User/UpdateEmail", userEmail);
  }

  updatePassword(userPass: UserPasswordViewModel): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/User/UpdatePassword", userPass);
  }

  updatePerson(person: PersonaDto,id:number): Observable<any> {
    return this.http.put<any>(this.urlApi + "api/User/UpdatePerson/"+id, person);
  }

  updatePhoto(filePhoto:FilePhotoViewModel){
    return this.http.post<any>(this.urlApi + "api/User/UpdatePhoto", filePhoto);
  }
}
