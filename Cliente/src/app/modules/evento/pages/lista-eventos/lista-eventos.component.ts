import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { HomeService } from 'src/app/modules/home/services/home.service';
import { EventoService } from '../../services/evento.service';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';

@Component({
  selector: 'app-lista-eventos',
  templateUrl: './lista-eventos.component.html',
  styleUrls: ['./lista-eventos.component.css']
})
export class ListaEventosComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public lstEventos: any = [];
  private dataCountInit: number = 6;
  public lstCategorias: CategoriaDto;
  public filterSelected: boolean;
  public showLoader: boolean = true;
  public lengthDataLimit: number = 0;
  public idCategoriaFilter: number = 0;

  constructor(
    private eventService: EventoService,
    private homeService: HomeService,
    private router: Router,
    private suscriptionService: SuscripcionService
  ) { }

  ngOnInit(): void {
    window.scroll(0, 0);
    this.getCategorias();
    this.getEventos(true);
    this.filterSelected = false;
  }

  /*
    *Antes de realizar la petición para obtener los eventos:
       -Se usa una bandera fetchData para evitar peticiones infinitas, debido al uso de la directiva para cargar los datos en 
        intervalos de 6 cada vez que coincide el 'Codigo' de cada objeto del array con la longitud del listado.
       -Se verifica si la longitud del listado es distinto al limite establecido o el limite y la longitud de la lista es cero,
        eso para evitar que una vez cargada la información y llegue el scroll al final de la página realice peticiones innecesarias.
    *Se usa el método reduce para agregar una nueva propiedad('Codigo') a cada objeto del array y a la vez se filtra los datos para 
     obtener solo la cantidad del intervalo establecido
    *No se utiliza el Id por defecto del usuario para obtener la información debido a que en caso de que el Id puede no ser secuencial(1,5,10,11...)
     como sucede cuando se aplica algún filtro(categoría), por eso se usa el indice ya que de no hacerlo la longitud del listado no coincidiría con
     el 'Id' del evento pero sí con el índice (1,2,3,4,5,6...)
    *Cuando se selecciona un evento se despliega el detalle del mismo y en caso de que el usuario decida suscribirse o desuscribirse del evento,
     se debe eliminar manualmente el componente debido al comportamiento por defecto de RouteReuseStrategy. Después se emite un valor que sería la 
     cantidad de eventos que se cargarán en la lista y así reflejar el número de suscriptores actualizado. El comportamiento da como resultado que
     cuando el usuario retroceda a la página de inicio, sea ubicado en la última sección donde seleccionó el evento. Para ello se hace uso de un observable 
     para usar el último valor emitido(límite) antes de cargar el listado de eventos.
  */
  getEventos(fetchData: boolean) {
    if (fetchData) {
      if (this.lengthDataLimit !== this.lstEventos.length || (this.lengthDataLimit === 0 && this.lstEventos.length === 0)) {
        let endLimit = this.lstEventos.length + this.dataCountInit + 1;
        this.suscriptionService.issueChanges$.pipe(
          takeUntil(this.stop$)).subscribe(res => {
            res !== 0 ? endLimit = res : null;
          });
        this.eventService.getEventosSuscripciones(this.idCategoriaFilter)
          .pipe(
            takeUntil(this.stop$),
            tap(t => this.lengthDataLimit = t.length),
            map(x =>
              x.reduce((a: any, b: any, index: number) => index + 1 > this.lstEventos.length && index + 1 < endLimit ? [...a, { ...b, Codigo: index + 1 }] : a, [])
            )).subscribe(
              (res: any) => {
                this.suscriptionService.emitChanges(0);
                this.lstEventos = this.lstEventos.concat(res);
                this.showLoader = false;
              });
      }
    }
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  getCategorias() {
    this.eventService.getCategorias().pipe(takeUntil(this.stop$))
      .subscribe(
        res => {
          this.lstCategorias = res;
        });
  }

  onChange(event: any) {
    this.clearBeforeLoad();
    const idCategoria = Number(event.target.value);
    idCategoria === 0 ? this.filterSelected = false : this.filterSelected = true;
    this.idCategoriaFilter = idCategoria;
    this.getEventos(true);
  }

  clearBeforeLoad() {
    this.dataCountInit = 6;
    this.lstEventos = [];
  }

  removeFilter() {
    this.idCategoriaFilter = 0;
    this.clearBeforeLoad();
    this.filterSelected = false;
    this.resetSelect();
    this.getEventos(true);
  }

  resetSelect() {
    (<HTMLInputElement>document.getElementById('select-filter')).value = '';
  }

  showDetailEvent(item: any, loadLimit: number) {
    item.Limit = loadLimit + 1;
    this.homeService.enableJumbotron(false);
    this.router.navigate(['/evento/detail', btoa(JSON.stringify(item))], { skipLocationChange: true });
  }
}
