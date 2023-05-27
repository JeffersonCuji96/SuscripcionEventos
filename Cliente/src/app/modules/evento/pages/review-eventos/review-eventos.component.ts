import { Component, Input, OnDestroy, OnInit, NgZone } from '@angular/core';
import { EventoService } from '../../services/evento.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Helpers } from 'src/app/helpers/helper';
import { SimpleModalService } from 'ngx-simple-modal';
import { RegisterPageComponent } from '../register-page/register-page.component';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';
declare let $: any;

@Component({
  selector: 'app-review-eventos',
  templateUrl: './review-eventos.component.html',
  styleUrls: ['./review-eventos.component.css']
})
export class ReviewEventosComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public lstEventos: any;
  public showLoader: boolean = true;
  @Input() Title: any = "EVENTOS ORGANIZADOS";
  @Input() Suscription = false;

  constructor(
    private eventService: EventoService,
    private authService: AuthService,
    private helper: Helpers,
    private SimpleModalService: SimpleModalService,
    private router: Router,
    private suscriptionService: SuscripcionService,
    private ngZone: NgZone,
  ) { }

  ngOnInit(): void {
    this.checkEventEmitChanges();
    this.notifiedSuscriptionChange();
    this.notifiedEventChange();
    this.processedEventChange();
    if (this.Suscription === false) {
      this.getByUserventos();
    } else {
      this.getByUserSuscriptions();
    }
  }

  checkEventEmitChanges() {
    this.eventService.issueChanges$.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        res ? this.getByUserventos() : null;
      });
  }

  notifiedSuscriptionChange() {
    this.suscriptionService.suscriptionChange.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        if (res && this.Suscription === true) {
          this.ngZone.run(() => this.getByUserSuscriptions());
        }
      });
  }

  notifiedEventChange() {
    this.eventService.notifiedEventChange.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        if (res && this.Suscription === false) {
          this.ngZone.run(() => this.getByUserventos());
        }
      });
  }

  processedEventChange() {
    this.eventService.processedEventChange.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        this.ngZone.run(()=>{
          if(res && this.Suscription === false){
            this.getByUserventos();
          } 
          if(res && this.Suscription === true){
            this.getByUserSuscriptions();
          } 
        });
      });
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

  getByUserSuscriptions() {
    const idUsuario = this.authService.getIdUserLocalStorage();
    return this.suscriptionService.getByUserSuscriptions(idUsuario).pipe(takeUntil(this.stop$))
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
        scrollX: true,
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

  showModalDetailEvent(event: any) {
    this.SimpleModalService.addModal(
      RegisterPageComponent,
      {
        Title: event.IdEstado === 4 ? 'Información del evento' : 'Edición del evento',
        Data: this.formatDateEvent(event)
      }
    );
  }

  formatDateEvent(event: any) {
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
            this.eventService.notifiedEventChange.emit(true);
            this.getByUserventos();
          },
            error => {
              this.helper.manageErrors(error);
            });
      }
    });
  }

  showPageDetailEvent(event: any) {
    event.Limit = 0;
    this.router.navigate(['/evento/detail', btoa(JSON.stringify(event))], { skipLocationChange: true });
  }
}
