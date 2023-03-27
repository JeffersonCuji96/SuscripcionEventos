import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { map, takeUntil, tap } from 'rxjs/operators';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { EventoService } from '../../services/evento.service';

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
  public showLoader:boolean=true;

  constructor(
    private eventService: EventoService
  ) { }

  ngOnInit(): void {
    window.scroll(0, 0);
    this.getEventos(true, 0);
    this.getCategorias();
    this.filterSelected=false;
  }

  getEventos(fetchData: boolean, idCategoria: number) {
    if (fetchData) {
      let endLimit = this.lstEventos.length + this.dataCountInit + 1;
      this.eventService.getEventosSuscripciones(idCategoria)
        .pipe(
          takeUntil(this.stop$),
          map(x => x.filter((evt: any, index: number) =>
            index + 1 > this.lstEventos.length && index + 1 < endLimit)
          )).subscribe(
            (res: any) => {
              this.lstEventos = this.lstEventos.concat(res);
              this.showLoader=false;
            });
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
    this.getEventos(true, idCategoria);
  }

  clearBeforeLoad() {
    this.dataCountInit = 6;
    this.lstEventos = [];
  }

  removeFilter() {
    this.clearBeforeLoad();
    this.filterSelected = false;
    this.resetSelect();
    this.getEventos(true, 0);
  }

  resetSelect(){
    (<HTMLInputElement>document.getElementById('select-filter')).value='';
  }
}
