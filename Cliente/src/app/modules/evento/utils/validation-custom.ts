import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { map, takeUntil } from 'rxjs/operators';
import { UserService } from '../../user/services/user.service';
import * as moment from 'moment';
import { Observable, of } from 'rxjs';

@Injectable({
    providedIn: 'root'
})

export class ValidationEventoCustom {

    static isAvailableDateStart(fechaFin: any, stop: any, userService: UserService) {
        return (ctrl: AbstractControl) => {
            const fechaInicio = ctrl.value;
            if (!ctrl.value) {
                return of(null);
            } else {
                return userService.getCurrentDate()
                    .pipe(
                        takeUntil(stop),
                        map(res => {
                            let currentDate = res.substring(0, 10);
                            if (fechaInicio < currentDate) {
                                return { notDateStart: true }
                            }
                            if (fechaFin !== '' && fechaInicio > fechaFin) {
                                return { notDateStartEnd: true }
                            }
                        })
                    );
            }
        };
    }

    static isAvailableTimeStart(fechaInicio: any, horaFin: any, fechaFin: any, stop: any, userService: UserService) {
        return (ctrl: AbstractControl) => {
            const horaInicio = ctrl.value;
            if (fechaInicio !== '' && horaInicio !== '') {
                return userService.getCurrentDate()
                    .pipe(
                        takeUntil(stop),
                        map(res => {
                            let currentDate = res.substring(0, 10);
                            let dateTimeFActual = new Date(res);
                            let horaActual = moment(dateTimeFActual).format('HH:mm').concat(':00');
                            dateTimeFActual.setMinutes(dateTimeFActual.getMinutes() + 30);
                            let horaMinimaHActual = moment(dateTimeFActual).format('HH:mm');
                            if (currentDate === fechaInicio) {
                                if (horaInicio === horaActual) {
                                    return { notTimeStartActual: true };
                                }
                                if (horaInicio < horaActual) {
                                    return { notTimeStartMinor: true };
                                }
                                if (horaInicio < horaMinimaHActual) {
                                    return { notTimeStartMinimum: true };
                                }
                            }
                            if (fechaFin === fechaInicio && horaFin !== '') {
                                let dateTimeHInicio = new Date(currentDate.concat(" " + horaFin + ""));
                                dateTimeHInicio.setMinutes(dateTimeHInicio.getMinutes() - 30);
                                let horaMinimaHInicio = moment(dateTimeHInicio).format('HH:mm');
                                if (horaInicio >= horaFin) {
                                    return { notTimeStartEnd: true };
                                }
                                if (horaInicio>horaMinimaHInicio){
                                    return { notTimeStartEndMinimum: true };
                                }
                            }
                        })
                    )
            }
            return of(null);
        };
    }

    static isAvailableDateEnd(fechaInicio: any, stop: any, userService: UserService) {
        return (ctrl: AbstractControl) => {
            const fechaFin = ctrl.value;
            if (fechaFin !== '') {
                return userService.getCurrentDate()
                    .pipe(
                        takeUntil(stop),
                        map(res => {
                            let currentDate = res.substring(0, 10);
                            if (fechaFin < currentDate) {
                                return { notDateCurrent: true }
                            }
                            if (fechaInicio !== '') {
                                if (fechaFin < fechaInicio) {
                                    return { notDateEnd: true };
                                }
                            }
                        })
                    );
            }
            return of(null);
        };
    }

    static isAvailableTimeEnd(fechaInicio: any, fechaFin: any, horaInicio: any, stop: any, userService: UserService) {
        return (ctrl: AbstractControl) => {
            const horaFin = ctrl.value;
            if (horaFin !== '') {
                return userService.getCurrentDate()
                    .pipe(
                        takeUntil(stop),
                        map(res => {
                            let currentDate = moment(res).format('YYYY-MM-DD');
                            let dateTime = new Date(res);
                            let horaActual = moment(dateTime).format('HH:mm:ss');
                            if (fechaFin === currentDate && horaFin <= horaActual) {
                                return { notTimeEndActual: true };
                            }
                            if (horaInicio !== '') {
                                let dateTime = new Date(currentDate.concat(" " + horaInicio + ""));
                                dateTime.setMinutes(dateTime.getMinutes() + 30);
                                const horaMinima = moment(dateTime).format('HH:mm');
                                if (fechaInicio === fechaFin) {
                                    if (horaFin === horaInicio) {
                                        return { notTimeEndStart: true };
                                    }
                                    if (horaFin < horaInicio) {
                                        return { notTimeEndMinor: true };
                                    }
                                    if (horaFin < horaMinima) {
                                        return { notTimeEndMinimum: true };
                                    }
                                }
                            }
                        })
                    )
            }
            return of(null);
        };
    }
}