import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserService } from '../../user/services/user.service';
import * as moment from 'moment';

@Injectable({
    providedIn: 'root'
})

export class ValidationEventoCustom {

    static isAvailableDateStart(api: UserService) {
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
                            if (dateValue < dateRes) {
                                return { notDateStart: true }
                            }
                        })
                    );
            }
        };
    }

    static isAvailableTimeStart(ctrlFInicio: string, ctrlHInicio: string, userService: UserService) {
        return (controls: AbstractControl) => {
            const fechaInicio = controls.get(ctrlFInicio).value;
            const horaInicio = controls.get(ctrlHInicio).value;
            if (fechaInicio !== '' && horaInicio !== '') {
                return userService.getCurrentDate()
                    .subscribe(res => {
                        let currentDate = res.substring(0, 10);
                        if (currentDate === fechaInicio) {
                            let dateTime = new Date(res);
                            let horaActual = moment(dateTime).format('HH:mm');
                            dateTime.setMinutes(dateTime.getMinutes() + 30);
                            let horaMinima = moment(dateTime).format('HH:mm');
                            if (horaInicio === '00:00') {
                                controls.get(ctrlHInicio).setErrors({ notTimeStartPast: true });
                                return { notTimeStartPast: true };
                            }
                            if (horaInicio === horaActual) {
                                controls.get(ctrlHInicio).setErrors({ notTimeStartActual: true });
                                return { notTimeStartActual: true };
                            }
                            if (horaInicio < horaActual) {
                                controls.get(ctrlHInicio).setErrors({ notTimeStartMinor: true });
                                return { notTimeStartMinor: true };
                            }
                            if (horaInicio < horaMinima) {
                                controls.get(ctrlHInicio).setErrors({ notTimeStartMinimum: true });
                                return { notTimeStartMinimum: true };
                            }
                        }
                    });
            } else {
                return null;
            }
        };
    }
    static isAvailableDateEnd(ctrlFInicio: string, ctrlFFin: string) {
        return (controls: AbstractControl) => {
            const fechaInicio = controls.get(ctrlFInicio).value;
            const fechaFin = controls.get(ctrlFFin).value;
            if (fechaInicio !== '' && fechaFin !== '') {
                if (fechaFin < fechaInicio) {
                    controls.get(ctrlFFin).setErrors({ notDateEnd: true });
                    return { notDateEnd: true };
                }
            } else {
                return null;
            }
        };
    }

    static isAvailableTimeEnd(ctrlFInicio: string, ctrlFFin: string,ctrlHInicio:string,ctrlHFin:string) {
        return (controls: AbstractControl) => {
            const fechaInicio = controls.get(ctrlFInicio).value;
            const fechaFin = controls.get(ctrlFFin).value;
            const horaInicio = controls.get(ctrlHInicio).value;
            const horaFin = controls.get(ctrlHFin).value;

            if (horaInicio !== '' && horaFin !== '') {
                var date=moment(Date.now()).format('YYYY-MM-DD');
                var dateTime=new Date(date.concat(" "+horaInicio+':00'+""));
                dateTime.setMinutes(dateTime.getMinutes() + 30);
                const horaMinima=moment(dateTime).format('HH:mm');
                if (horaFin === horaInicio) {
                    controls.get(ctrlHFin).setErrors({ notTimeEndStart: true });
                    return { notTimeEndStart: true };
                }
                if (horaFin < horaInicio && fechaInicio === fechaFin) {
                    controls.get(ctrlHFin).setErrors({ notTimeEndMinor: true });
                    return { notTimeEndMinor: true };
                }
                if (horaFin < horaMinima && fechaInicio === fechaFin) {
                    controls.get(ctrlHFin).setErrors({ notTimeEndMinimum: true });
                    return { notTimeEndMinimum: true };
                }
            } else {
                return null;
            }
        };
    }
}