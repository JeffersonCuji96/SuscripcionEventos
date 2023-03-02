import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { EventoDto } from 'src/app/core/models/eventoDTO';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class EventoService {

  readonly urlApi = environment.urlHost;
  constructor(private http: HttpClient) { }

  getCategorias():Observable<CategoriaDto>{
    return this.http.get<CategoriaDto>(this.urlApi+"api/Evento/GetCategorias");
  }

  register(evento: EventoDto): Observable<any> {
    return this.http.post<any>(this.urlApi + "api/Evento", evento);
  }
}
