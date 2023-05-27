import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable  } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { EventoDto } from 'src/app/core/models/eventoDTO';
import { SuscripcionDto } from 'src/app/core/models/suscripcionDto';
import { EventNotificationViewModel } from 'src/app/core/models/view-models/eventNotificationViewModel';
import { NotificationViewModel } from 'src/app/core/models/view-models/notificationViewModel';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SuscripcionService {

  constructor(private http: HttpClient) { }
  readonly urlApi = environment.urlHost;
  suscriptionChange = new EventEmitter<boolean>();
  notifiedSuscriptionChange = new EventEmitter<boolean>();
  
  changeSuscription = new BehaviorSubject<number>(0);
  changeSuscription$ = this.changeSuscription.asObservable();
  changeDesuscription = new BehaviorSubject<number>(0);
  changeDesuscription$ = this.changeDesuscription.asObservable();

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

  getNotificationsMongoDb(id:number):Observable<NotificationViewModel>{
    return this.http.get<NotificationViewModel>(this.urlApi + "api/Suscripcion/GetNotificationsMongoDb/"+id);
  }

  removeNotification(id:string){
    return this.http.delete<any>(this.urlApi+"api/Suscripcion/RemoveNotification/"+id);
  }

  getNotificationEvent(eventNotification:EventNotificationViewModel):Observable<any>{
    return this.http.post<any>(this.urlApi + "api/Suscripcion/GetNotificationEvent", eventNotification);
  }
}
