import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CategoriaDto } from 'src/app/core/models/categoriaDto.';
import { EventoDto } from 'src/app/core/models/eventoDTO';
import { Helpers } from 'src/app/helpers/helper';
import { UserService } from 'src/app/modules/user/services/user.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { BuilderService } from '../../services/builder.service';
import { EventoService } from '../../services/evento.service';
import { ValidationEventoCustom } from '../../utils/validation-custom';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public isShowBoxDateTime: boolean = false;
  public lstCategorias: CategoriaDto;
  public btnId="btnRegEvento";

  constructor(
    public serviceBuilder: BuilderService,
    private eventService: EventoService,
    private userService: UserService,
    private router: Router,
    private helper: Helpers) {

  }
  ngOnInit(): void {
    this.serviceBuilder.form.enable();
    this.getCategorias();
  }

  ngOnDestroy() {
    this.serviceBuilder.form.reset();
    this.stop$.next();
    this.stop$.complete();
  }

  getCategorias() {
    return this.eventService.getCategorias().pipe(takeUntil(this.stop$))
      .subscribe(
        res => {
          this.lstCategorias = res;
        });
  }

  get f(): { [key: string]: AbstractControl } {
    return this.serviceBuilder.form.controls;
  }

  onFileChanged(event: any) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      var valid = ValidationCustom.isAvailableFormatAndSize(file, this.f.FileTemporal, "evento");
      if (valid === null) {
        this.helper.convertFileBase64(file).subscribe(base64 => {
          this.serviceBuilder.form.patchValue({
            ImageBase64: base64,
          });
        });
      }
    }
  }

  register() {
    this.serviceBuilder.form.disable();
    this.helper.disableInputElement(this.btnId, true);
    var evento: EventoDto = this.serviceBuilder.form.value;
    this.eventService.register(evento)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.swalShowSuccess(response.Message);
        this.router.navigate(['/']);
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnId, false);
          this.serviceBuilder.form.enable();
        });
  }

  showBoxDateTime(value: any) {
    this.isShowBoxDateTime = value;
    if (value === false) {
      this.f.FechaFin.setValue(null);
      this.f.HoraFin.setValue(null);
      this.serviceBuilder.removeValidators(this.f.HoraFin);
      this.serviceBuilder.removeValidators(this.f.FechaFin);
    } else {
      this.serviceBuilder.addValidRequired(this.f.HoraFin);
      this.serviceBuilder.addValidRequired(this.f.FechaFin);
    }
  }

  availableDateStart() {
    let ctrlFInicio=this.f.FechaInicio;
    let ctrlFFin=this.f.FechaFin;
    this.serviceBuilder.availableInfo(
      ctrlFInicio,
      ValidationEventoCustom.isAvailableDateStart(
        ctrlFFin.value,
        this.stop$,
        this.userService)
    );
    if(this.f.HoraInicio.value!==''){
      this.availableTimeStart();
    }
    if( ctrlFInicio.value!=='' && ctrlFFin.value!==''){
      if(ctrlFFin.status==="INVALID"){
        this.availableDateEnd();
      }
    }
  }

  availableTimeStart() {
    let fechaInicio=this.f.FechaInicio.value;
    let fechaFin=this.f.FechaFin.value;
    let ctrlHInicio= this.f.HoraInicio;
    let horaFin=this.f.HoraFin.value;
    this.serviceBuilder.availableInfo(
      ctrlHInicio,
      ValidationEventoCustom.isAvailableTimeStart(
        fechaInicio,
        horaFin,
        fechaFin, 
        this.stop$,
        this.userService)
    );
    if(fechaInicio!=='' && fechaFin!=='' && ctrlHInicio.value!=='' && horaFin!==''){
      if(this.f.HoraFin.status==="INVALID"){
        this.availableTimeEnd();
      }
    }
  }

  availableDateEnd() {
    let ctrlFInicio=this.f.FechaInicio;
    let ctrlFFin=this.f.FechaFin;
    this.serviceBuilder.availableInfo(
      ctrlFFin,
      ValidationEventoCustom.isAvailableDateEnd(
        ctrlFInicio.value,
        this.stop$,
        this.userService)
    );
    if(this.f.HoraFin.value!==''){
      this.availableTimeEnd();
    }
    if(ctrlFInicio.value!=='' && ctrlFFin.value!==''){
      if(ctrlFInicio.status==="INVALID"){
        this.availableDateStart();
      }
    }
  }

  availableTimeEnd() {
    let fechaInicio=this.f.FechaInicio.value;
    let fechaFin=this.f.FechaFin.value;
    let horaInicio= this.f.HoraInicio.value;
    let ctrlHoraFin=this.f.HoraFin;
    this.serviceBuilder.availableInfo(
      ctrlHoraFin,
      ValidationEventoCustom.isAvailableTimeEnd(
        fechaInicio,
        fechaFin,
        horaInicio,
        this.stop$,
        this.userService)
    );
    if(fechaInicio!=='' && fechaFin!=='' && horaInicio!=='' && ctrlHoraFin.value!==''){
      if(this.f.HoraInicio.status==="INVALID"){
        this.availableTimeStart();
      }
    }
  }
}
