import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class BuilderService {

  constructor(private fb: FormBuilder) { }

  form = this.fb.group({
    Titulo: ['', {
      validators: [Validators.compose([
        Validators.required,
        Validators.maxLength(100)
      ])],
    }],
    FechaInicio: ['', Validators.required],
    HoraInicio: ['',Validators.required],
    FechaFin: [null],
    HoraFin: [null],
    Ubicacion: ['', {
      validators: [Validators.compose([
        Validators.required,
        Validators.maxLength(500)
      ])],
    }],
    InformacionAdicional: ['', Validators.maxLength(500)],
    FileTemporal: [''],
    ImageBase64: [''],
    IdCategoria: ['', Validators.required],
    IdUsuario: ['13', Validators.required]
  });

  availableInfo(ctrl: AbstractControl, fnValidator: AsyncValidatorFn): void {
    ctrl.setAsyncValidators(fnValidator);
    ctrl.updateValueAndValidity();
    ctrl.clearAsyncValidators();
  }

  addValidRequired(ctrl: AbstractControl): void {
    ctrl.setValidators(Validators.required);
    ctrl.updateValueAndValidity();
  }

  removeValidators(ctrl: AbstractControl): void {
    ctrl.setValidators(null);
    ctrl.setErrors(null);
  }
}