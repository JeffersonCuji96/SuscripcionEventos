import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { UserService } from '../../user/services/user.service';
import { ValidationEventoCustom } from '../utils/validation-custom';

@Injectable({
  providedIn: 'root'
})
export class BuilderService {

  constructor(private fb: FormBuilder,private userService:UserService) { }

  form = this.fb.group({
    Titulo: ['', {
      validators: [Validators.compose([
        Validators.required,
        Validators.maxLength(100)
      ])],
    }],
    FechaInicio: ['', {
      validators: [Validators.compose([
        Validators.required
      ])],
    }],
    HoraInicio: ['', {
      validators: [Validators.compose([
        Validators.required
      ])],
    }],
    FechaFin: [''],
    HoraFin: [''],
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
    IdUsuario: ['', Validators.required]
  },
    {
      validators: [Validators.compose([
        ValidationEventoCustom.isAvailableTimeStart("FechaInicio", "HoraInicio",this.userService),
        ValidationEventoCustom.isAvailableDateEnd("FechaInicio", "FechaFin"),
        ValidationEventoCustom.isAvailableTimeEnd("FechaInicio", "FechaFin","HoraInicio","HoraFin")
      ])]
    },
  );

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