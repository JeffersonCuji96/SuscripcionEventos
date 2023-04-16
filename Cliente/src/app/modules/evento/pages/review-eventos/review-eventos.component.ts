import { Component, OnDestroy, OnInit } from '@angular/core';
import { EventoService } from '../../services/evento.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { EventoDto } from 'src/app/core/models/eventoDTO';
import { Helpers } from 'src/app/helpers/helper';
import { SimpleModalService } from 'ngx-simple-modal';
import { RegisterPageComponent } from '../register-page/register-page.component';
import { formatDate } from '@angular/common';
declare let $: any;

@Component({
  selector: 'app-review-eventos',
  templateUrl: './review-eventos.component.html',
  styleUrls: ['./review-eventos.component.css']
})
export class ReviewEventosComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public lstEventos: EventoDto;
  public showLoader: boolean = true;

  constructor(
    private eventService: EventoService,
    private authService: AuthService,
    private helper: Helpers,
    private SimpleModalService: SimpleModalService
  ) { }

  ngOnInit(): void {
    this.eventService.issueChanges$.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        res ? this.getByUserventos() : null;
      });
    this.getByUserventos();
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  getByUserventos() {
    const idUsuario = this.authService.getIdUserLocalStorage();
    return this.eventService.getByUserEventos(idUsuario).pipe(takeUntil(this.stop$))
      .subscribe(
        res => {
          this.lstEventos = res;
          this.renderDataTable();
          this.showLoader = false;
        });
  }

  renderDataTable() {
    $(document).ready(function () {
      $('#tblEventos').DataTable({
        retrieve: true,
        columnDefs: [
          {
            targets: [0, 3],
            render: function (data, type, row) {
              if (type === 'filter') {
                return data.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
              }
              return data;
            }
          }],
        language: { url: "/assets/datatables/es-ES.json" }
      });
    });
  }

  showDetailEvent(event: EventoDto) {
    const data = this.formatDateEvent(event);
    this.SimpleModalService.addModal(
      RegisterPageComponent,
      {
        Title: 'Edición del evento',
        Data: data
      }
    );
  }

  formatDateEvent(event: EventoDto) {
    event.FechaInicio = formatDate(event.FechaInicio, 'yyyy-MM-dd', 'en-US');
    if (event.FechaFin !== null) {
      event.FechaFin = formatDate(event.FechaFin, 'yyyy-MM-dd', 'en-US');
    }
    return event;
  }

  removeEvent(id: number) {
    this.helper.confirmDeleteSwal().then((result) => {
      if (result.isConfirmed) {
        this.eventService.removeEvent(id)
          .pipe(takeUntil(this.stop$))
          .subscribe(response => {
            this.helper.swalShowSuccess(response.Message);
            this.getByUserventos();
          },
            error => {
              this.helper.manageErrors(error);
            });
      }
    });
  }
}
