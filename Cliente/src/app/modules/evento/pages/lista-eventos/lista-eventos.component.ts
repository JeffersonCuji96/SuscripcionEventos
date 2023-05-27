import { Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { EventoService } from '../../services/evento.service';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';
import { Helpers } from 'src/app/helpers/helper';

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
  public linkClicked: boolean = false;

  constructor(
    private eventService: EventoService,
    private router: Router,
    private suscriptionService: SuscripcionService,
    private helpers: Helpers
  ) { }

  ngOnInit(): void {
    this.updateCountDesuscription();
    this.updateCountSuscription();
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
     se emite un valor(Id), al detectar el cambio del observable se modifica el objeto evento del listado con la cantidad de suscriptores 
     que corresponda, esto es necesario debido al comportamiento de routestrategy aplicada en este componente. Si en caso el usuario 
     retrocede a la página anterior(Inicio), es ubicado en la última sección donde seleccionó el evento.
  */
  getEventos(fetchData: boolean) {
    if (fetchData) {
      if (this.lengthDataLimit !== this.lstEventos.length || (this.lengthDataLimit === 0 && this.lstEventos.length === 0)) {
        let endLimit = this.lstEventos.length + this.dataCountInit + 1;
        this.eventService.getEventosSuscripciones(this.idCategoriaFilter).pipe(
          takeUntil(this.stop$),
          tap(t => this.lengthDataLimit = t.length),
          map(x =>
            x.reduce((a: any, b: any, index: number) => index + 1 > this.lstEventos.length && index + 1 < endLimit ? [...a, { ...b, Codigo: index + 1 }] : a, [])
          )).subscribe((res: any) => {
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

  updateCountSuscription() {
    this.suscriptionService.changeSuscription$.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        if (res !== 0) {
          const objeto = this.lstEventos.find((i: any) => i.Id === res);
          objeto.Suscriptores += 1;
        }
      });
  }
  updateCountDesuscription() {
    this.suscriptionService.changeDesuscription$.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        if (res !== 0) {
          const objeto = this.lstEventos.find((i: any) => i.Id === res);
          objeto.Suscriptores -= 1;
        }
      });
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
    this.linkClicked = true;
    this.eventService.getStatusEvent(item.Id)
      .pipe(takeUntil(this.stop$))
      .subscribe(
        status => {
          this.linkClicked = false;
          if (status === 2) {
            this.helpers.swalShow(
              "<h4>Operación no realizada</h4>",
              "No de se puede mostrar el detalle del evento debido a que ha sido eliminado",
              "warning");
          } else {
            item.IdEstado = status;
            //item.Limit = loadLimit + 1;
            //item.Listado=JSON.stringify(this.lstEventos);
            this.router.navigate(['/evento/detail', btoa(JSON.stringify(item))], { skipLocationChange: true });
          }
        });
  }
}
