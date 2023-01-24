import { AbstractControl, ValidatorFn } from '@angular/forms';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserService } from '../services/user.service';
import { Helpers } from 'src/app/helpers/helper';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})

export class ValidationCustom {

    static isEmailFormat(ctrl: AbstractControl) {
        const value = ctrl.value;
        if (value == '' || value == null) {
            return null;
        }
        const expressionEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const expressionPattern = /^[a-zA-Z0-9ñÑ.@]+$/;

        if (!expressionEmail.test(String(value)) && expressionPattern.test(String(value))) {
            return { isEmail: true }
        }
        return null;
    }

    static isAvailableEmail(api: UserService) {
        return (ctrl: AbstractControl) => {
            const value = ctrl.value;
            if (!ctrl.value) {
                return of(null);
            } else {
                return api.checkEmail(value)
                    .pipe(
                        map(res => {
                            return res ? null : { notEmail: false };
                        })
                    );
            }
        };
    }

    static isAvailablePhone(api: UserService) {
        return (ctrl: AbstractControl) => {
            const value = ctrl.value;
            if (!ctrl.value) {
                return of(null);
            } else {
                return api.checkPhone(value)
                    .pipe(
                        map(res => {
                            return res ? null : { notPhone: false };
                        })
                    );
            }
        };
    }
    static isAvailableDateBirth(api: UserService) {

        return (ctrl: AbstractControl) => {
            const value = ctrl.value;
            if (!ctrl.value) {
                return of(null);
            } else {
                let dateValue = Date.parse(value);
                return api.getCurrentDate()
                    .pipe(
                        map(res => {
                            let dateRes = Date.parse(res.substring(0, 10));
                            if (dateValue >= dateRes) {
                                return { notDateBirth: true }
                            }
                            if (Helpers.getAgeBirthDate(value) < 18) {
                                return { notAdult: true }
                            }
                        })
                    );
            }
        };
    }

    static isMatchTwoCtrl(controlName: string, checkControlName: string): ValidatorFn {
        return (controls: AbstractControl) => {
            const control = controls.get(controlName);
            const checkControl = controls.get(checkControlName);
            if (checkControl.errors && !checkControl.errors.matching) {
                return null;
            }
            if (control.value !== checkControl.value) {
                controls.get(checkControlName).setErrors({ matching: true });
                return { matching: true };
            } else {
                return null;
            }
        };
    }

    static isValidNumDigitsPhone(ctrl: AbstractControl) {
        const value = ctrl.value;
        if (value == '' || value == null || isNaN(value) || value.includes(" "))
            return null;

        if (String(value).length !== 10)
            return { numDigits: true }

        return null;
    }

    static isValidPassword(ctrl: AbstractControl) {
        const value = ctrl.value;
        if (value == '' || value == null)
            return null;

        if (!value.match(/[\d]/g))
            return { number: true }

        if (!value.match(/[A-Z]/g))
            return { uppercase: true }

        if (!value.match(/[a-z]/g))
            return { lowercase: true }

        if (!value.match(/[.*@%#]/g))
            return { special: true }

        if (value.length < 8)
            return { min: true }

        if (value.length > 15)
            return { max: true }

        if (value.includes(" "))
            return { space: true }

        return null;
    }

    static isAvailableFormatAndSize(file: any, ctrl: AbstractControl) {
        const typeJPG: boolean = file.type === "image/jpeg" ? true : false;
        const typePNG: boolean = file.type === "image/png" ? true : false;
        
        if (!((!typeJPG && typePNG) || (!typePNG && typeJPG))) {
            ctrl.setErrors({ notFormat: true });
            return { notFormat: true };
        }
        if(file.size>1000000){
            ctrl.setErrors({ notSize: true });
            return { notSize: true };
        }
        return null;
    }
}