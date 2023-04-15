import { Component, OnInit } from '@angular/core';
import { EventoService } from '../../services/evento.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { EventoDto } from 'src/app/core/models/eventoDTO';
declare let $: any;

@Component({
  selector: 'app-review-eventos',
  templateUrl: './review-eventos.component.html',
  styleUrls: ['./review-eventos.component.css']
})
export class ReviewEventosComponent implements OnInit {

  private stop$ = new Subject<void>();
  public lstEventos: EventoDto;
  public table: any;

  constructor(
    private eventService: EventoService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.getByUserventos();
  }

  getByUserventos() {
    const idUsuario = this.authService.getIdUserLocalStorage();
    return this.eventService.getByUserEventos(idUsuario).pipe(takeUntil(this.stop$))
      .subscribe(
        res => {
          this.lstEventos = res;
          this.renderDataTable();
        });
  }

  renderDataTable() {
    $(document).ready(function () {
      this.table = $('#tblEventos').DataTable({
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

  showDetailEvent(idEvent: number) {
    console.log("detail " + idEvent);
  }

  removeEvent(idEvent: number) {
    console.log("remove " + idEvent);
  }

}
