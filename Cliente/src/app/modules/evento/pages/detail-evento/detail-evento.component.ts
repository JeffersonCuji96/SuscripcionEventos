import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Location } from '@angular/common';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';
import { Subject } from 'rxjs';
import { SuscripcionDto } from 'src/app/core/models/suscripcionDto';
import { Helpers } from 'src/app/helpers/helper';
import { takeUntil } from 'rxjs/operators';
import { CustomRouteReuseStrategy } from 'src/app/app-services/CustomRouteReuseStrategy';

@Component({
  selector: 'app-detail-evento',
  templateUrl: './detail-evento.component.html',
  styleUrls: ['./detail-evento.component.css']
})
export class DetailEventoComponent implements OnInit, OnDestroy {

  public objEvento: any;
  public isOrganizer: boolean = true;
  public btnIdSuscribe: string = "btnSuscribe";
  public btnIdUnsuscribe: string = "btnUnsuscribe";
  private stop$ = new Subject<void>();
  public isSuscribe: number = 0;
  private objSuscripcion: SuscripcionDto;

  constructor(
    private actRouted: ActivatedRoute,
    private location: Location,
    private authService: AuthService,
    private suscriptionService: SuscripcionService,
    private helper: Helpers,
    private router:Router
  ) { }

  ngOnInit(): void {
    history.pushState(null, '');
    window.scroll(0, 0);
    this.actRouted.params.subscribe(
      (params: Params) => {
        this.objEvento = JSON.parse(atob(params.data));
      }
    );
    this.setValueData();
    this.checkUserOrganizer();
    if(this.isOrganizer === true){
      this.checkSuscribeUser();
    }
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  backPage() {
    this.location.back()
  }

  checkUserOrganizer() {
    if (this.objSuscripcion.IdUsuario === this.objEvento.Organizador.Id) {
      this.isOrganizer = false;
    }
  }

  suscribe() {
    this.helper.disableInputElement(this.btnIdSuscribe, true);
    this.suscriptionService.suscribe(this.objSuscripcion)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.swalShowSuccess(response.Message);
        this.isSuscribe = 1;
        this.objSuscripcion.Id=response.Id;
        this.deleteStoredRoute("/");
        this.suscriptionService.emitChanges(this.objEvento.Limit);
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnIdSuscribe, false);
        });
  }

  checkSuscribeUser() {
    this.suscriptionService.checkSuscribeUser(this.objSuscripcion)
      .pipe(takeUntil(this.stop$))
      .subscribe((response:any) => {
        this.objSuscripcion.Id=response.Item1;
        this.isSuscribe=response.Item2;
      },
        error => {
          this.helper.manageErrors(error);
        });
  }

  setValueData() {
    this.objSuscripcion = {
      Id:0,
      IdEvento: this.objEvento.Id,
      IdUsuario: this.authService.getIdUserLocalStorage(),
      IdEstado: 1
    };
  }

  unsuscribe() {
    this.helper.disableInputElement(this.btnIdUnsuscribe, true);
    this.suscriptionService.unsuscribe(this.objSuscripcion,this.objSuscripcion.Id)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.swalShowSuccess(response.Message);
        this.isSuscribe=0;
        this.deleteStoredRoute("/");
        this.suscriptionService.emitChanges(this.objEvento.Limit);
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnIdUnsuscribe, false);
        });
  }

  deleteStoredRoute(url:string):void{
    const strategy = this.router.routeReuseStrategy as CustomRouteReuseStrategy;
    strategy.deleteStoreRoute(url);
  }
}
