import { HttpClient } from '@angular/common/http';
import { Injectable,EventEmitter } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { EventoDto } from 'src/app/core/models/eventoDTO';
import { OrganizerCheckViewModel } from 'src/app/core/models/view-models/organizerCheckViewModel';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventoService {

  constructor(private http: HttpClient) { }
  
  readonly urlApi = environment.urlHost;
  notifiedEventChange = new EventEmitter<boolean>();
  processedEventChange = new EventEmitter<boolean>();
  notifiedJoinEvent = new EventEmitter<boolean>();
  private issueChanges = new Subject<boolean>();
  issueChanges$ = this.issueChanges.asObservable();
  emitChanges(value: boolean) {
    this.issueChanges.next(value);
  }

  getCategorias(): Observable<CategoriaDto> {
    return this.http.get<CategoriaDto>(this.urlApi + "api/Evento/GetCategorias");
  }

  getEventosSuscripciones(idCategoria: number): Observable<any> {
    return this.http.get<EventoDto>(this.urlApi + "api/Evento/GetEventosSuscripciones/" + idCategoria);
  }

  getByUserEventos(idUsuario: number): Observable<EventoDto> {
    return this.http.get<EventoDto>(this.urlApi + "api/Evento/GetByUserEventos/" + idUsuario);
  }

  register(evento: EventoDto): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/Evento", evento);
  }

  removeEvent(id:number):Observable<any>{
    return this.http.delete<any>(this.urlApi+"api/Evento/RemoveEvent/"+id);
  }

  updateEvent(event:any,id:number,checkImage:boolean){
    return this.http.put<any>(this.urlApi+"api/Evento/UpdateEvent/"+id+"/"+checkImage,event);
  }

  checkEventOrganizer(organizerCheckViewModel: OrganizerCheckViewModel): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/Evento/CheckEventOrganizer", organizerCheckViewModel);
  }

  getStatusEvent(id: number): Observable<number> {
    return this.http.get<number>(this.urlApi + "api/Evento/GetStatusEvent/" + id);
  }
}
