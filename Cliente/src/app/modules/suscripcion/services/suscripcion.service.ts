import { HttpClient } from '@angular/common/http';
import { Injectable  } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { EventoDto } from 'src/app/core/models/eventoDTO';
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

  getByUserSuscriptions(idUsuario: number): Observable<EventoDto> {
    return this.http.get<EventoDto>(this.urlApi + "api/Suscripcion/GetByUserSuscriptions/" + idUsuario);
  }
}
