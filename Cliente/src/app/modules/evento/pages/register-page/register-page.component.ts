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
import { SimpleModalComponent } from 'ngx-simple-modal';
import { AlertModel } from 'src/app/core/models/alert-model';
import { ScriptsService } from 'src/app/app-services/scripts.service';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent extends SimpleModalComponent<AlertModel, null> implements AlertModel, OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public isShowBoxDateTime: boolean = false;
  public lstCategorias: CategoriaDto;
  public btnIdRegister = "btnRegEvento";
  public btnIdUpdate = "btnUpdateEvento";
  Title: string = "Crear Evento";
  Data: any = null;
  spanFileName = "No se ha seleccionado ninguna foto";
  isChangeImage = false;
  imagePreview = null;
  status: number = 0;

  constructor(
    public serviceBuilder: BuilderService,
    private eventService: EventoService,
    private userService: UserService,
    private router: Router,
    private helper: Helpers,
    private scriptService: ScriptsService) {
    super();
  }
  ngOnInit(): void {
    this.serviceBuilder.form.enable();
    this.getCategorias();
    this.setValueForm();
  }

  ngOnDestroy() {
    this.serviceBuilder.form.reset();
    this.stop$.next();
    this.stop$.complete();
    this.scriptService.removeScript('img-modalevent-script');
    this.clearVariables();
  }

  setValueForm() {
    if (this.Data !== null) {
      this.loadScript();
      this.status = this.Data.IdEstado;
      this.Data.FileTemporal = null;
      this.imagePreview = this.Data.ImageBase64;
      this.Data.FechaFin !== null ? this.showBoxDateTime(true) : this.showBoxDateTime(false);
      this.serviceBuilder.form.patchValue(this.Data);
      this.status === 4 ? this.serviceBuilder.form.disable():null;
    }
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
          this.imagePreview = "data:image/png;base64," + base64;
          this.isChangeImage = true;
          this.serviceBuilder.form.patchValue({
            ImageBase64: base64,
          });
        });
      }
    }
  }

  register() {
    if (this.Data === null) {
      this.serviceBuilder.form.disable();
      this.helper.disableInputElement(this.btnIdRegister, true);
      var evento: EventoDto = this.serviceBuilder.form.value;
      this.eventService.register(evento)
        .pipe(takeUntil(this.stop$))
        .subscribe(response => {
          this.helper.swalShowSuccess(response.Message);
          this.router.navigate(['/']);
        },
          error => {
            this.helper.manageErrors(error);
            this.helper.disableInputElement(this.btnIdRegister, false);
            this.serviceBuilder.form.enable();
          });
    } else {
      this.updateEvent();
    }
  }

  loadScript() {
    this.scriptService.loadScript({
      id: 'img-modalevent-script',
      url: 'assets/js/img-modal.js'
    });
  }

  updateEvent() {
    const prevEvento = this.formatDataEvento()[0];
    const currEvento = this.formatDataEvento()[1];
    const isChangeEvento = this.helper.compareTwoObjects(prevEvento, currEvento);
    if (isChangeEvento === false) {
      this.isChangeImage === false ? prevEvento.ImageBase64 = null : null;
      this.serviceBuilder.form.disable();
      this.helper.disableInputElement(this.btnIdUpdate, true);
      this.eventService.updateEvent(prevEvento, currEvento.Id, this.isChangeImage)
        .pipe(takeUntil(this.stop$))
        .subscribe(response => {
          this.helper.swalShowSuccess(response.Message);
          this.eventService.emitChanges(true);
          this.close();
        },
          error => {
            this.helper.manageErrors(error);
            this.helper.disableInputElement(this.btnIdUpdate, false);
            this.serviceBuilder.form.enable();
            this.close();
          });
    } else {
      this.close();
    }
  }

  formatDataEvento() {
    const prevEvento = { Id: this.Data.Id, ...this.serviceBuilder.form.value };
    const currEvento = { ...this.Data };
    delete currEvento.Categoria;
    delete currEvento.IdEstado;
    var dataEvento: [EventoDto, EventoDto] = [prevEvento, currEvento];
    return dataEvento;
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
    let ctrlFInicio = this.f.FechaInicio;
    let ctrlFFin = this.f.FechaFin;
    this.serviceBuilder.availableInfo(
      ctrlFInicio,
      ValidationEventoCustom.isAvailableDateStart(
        ctrlFFin.value,
        this.stop$,
        this.userService)
    );
    if (this.f.HoraInicio.value !== '') {
      this.availableTimeStart();
    }
    if (ctrlFInicio.value !== '' && ctrlFFin.value !== '') {
      if (ctrlFFin.status === "INVALID") {
        this.availableDateEnd();
      }
    }
  }

  availableTimeStart() {
    let fechaInicio = this.f.FechaInicio.value;
    let fechaFin = this.f.FechaFin.value;
    let ctrlHInicio = this.f.HoraInicio;
    let horaFin = this.f.HoraFin.value;
    this.serviceBuilder.availableInfo(
      ctrlHInicio,
      ValidationEventoCustom.isAvailableTimeStart(
        fechaInicio,
        horaFin,
        fechaFin,
        this.stop$,
        this.userService)
    );
    if (fechaInicio !== '' && fechaFin !== '' && ctrlHInicio.value !== '' && horaFin !== '') {
      if (this.f.HoraFin.status === "INVALID") {
        this.availableTimeEnd();
      }
    }
  }

  availableDateEnd() {
    let ctrlFInicio = this.f.FechaInicio;
    let ctrlFFin = this.f.FechaFin;
    this.serviceBuilder.availableInfo(
      ctrlFFin,
      ValidationEventoCustom.isAvailableDateEnd(
        ctrlFInicio.value,
        this.stop$,
        this.userService)
    );
    if (this.f.HoraFin.value !== '') {
      this.availableTimeEnd();
    }
    if (ctrlFInicio.value !== '' && ctrlFFin.value !== '') {
      if (ctrlFInicio.status === "INVALID") {
        this.availableDateStart();
      }
    }
  }

  availableTimeEnd() {
    let fechaInicio = this.f.FechaInicio.value;
    let fechaFin = this.f.FechaFin.value;
    let horaInicio = this.f.HoraInicio.value;
    let ctrlHoraFin = this.f.HoraFin;
    this.serviceBuilder.availableInfo(
      ctrlHoraFin,
      ValidationEventoCustom.isAvailableTimeEnd(
        fechaInicio,
        fechaFin,
        horaInicio,
        this.stop$,
        this.userService)
    );
    if (fechaInicio !== '' && fechaFin !== '' && horaInicio !== '' && ctrlHoraFin.value !== '') {
      if (this.f.HoraInicio.status === "INVALID") {
        this.availableTimeStart();
      }
    }
  }

  clearVariables() {
    this.imagePreview = null;
    this.isChangeImage = false;
    this.Data = null;
  }
}
