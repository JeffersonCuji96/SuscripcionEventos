import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, Validators } from '@angular/forms';
import { ValidationCustom } from '../utils/validation-custom';

@Injectable({
  providedIn: 'root'
})
export class BuilderService {
 
    constructor(private fb: FormBuilder) {}

    form = this.fb.group({
        Email: ['', {
          validators: [Validators.compose([
            Validators.required,
            ValidationCustom.isEmailFormat,
            Validators.pattern(/^[a-zA-Z0-9ñÑ.@]+$/)
          ])]
        }],
        Clave: ['', {
          validators: [Validators.compose([
            Validators.required,
            ValidationCustom.isValidPassword
          ])]
        }],
        ConfirmClave: ['', Validators.required],
        FileTemporal:[''],
        ImageBase64: [''],
        Persona: this.fb.group({
          Nombre: ['', {
            validators: [Validators.compose([
              Validators.required,
              Validators.pattern(/^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$/)
            ])]
          }],
          Apellido: ['', {
            validators: [Validators.compose([
              Validators.required,
              Validators.pattern(/^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$/)
            ])]
          }],
          Telefono: ['',{
            validators: [Validators.compose([
              ValidationCustom.isValidNumDigitsPhone,  
              Validators.pattern(/^\d*$/)
            ])]
          }],
          FechaNacimiento: ['',Validators.required],
        })
      },
        {
          validators: [ValidationCustom.isMatchTwoCtrl("Clave", "ConfirmClave")],
        }
      );


    availableInfo(ctrl: AbstractControl,fnValidator:AsyncValidatorFn): void {
        ctrl.setAsyncValidators(fnValidator);
        ctrl.updateValueAndValidity();
        ctrl.clearAsyncValidators();
    } 
}