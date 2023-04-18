import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { SuscripcionDto } from 'src/app/core/models/suscripcionDto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SuscripcionService {

  constructor(private http: HttpClient) { }
  readonly urlApi = environment.urlHost;
  
  private issueChanges = new BehaviorSubject<number>(0);
  issueChanges$ = this.issueChanges.asObservable();
  emitChanges(value: number) {
    this.issueChanges.next(value);
  }

  suscribe(suscripcion: SuscripcionDto): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/Suscripcion", suscripcion);
  }

  checkSuscribeUser(suscripcion:any):Observable<number>{
    return this.http.post<number>(this.urlApi + "api/Suscripcion/CheckSuscribe", suscripcion);
  }

  unsuscribe(suscripcion: SuscripcionDto,id:number): Observable<any> {
    return this.http.put<any>(this.urlApi + "api/Suscripcion/"+id,suscripcion);
  }
}
