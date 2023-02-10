import { Router } from '@angular/router';
import * as moment from 'moment';
import { Observable, ReplaySubject } from 'rxjs';
import Swal from 'sweetalert2';

export class Helpers {
    
    disableInputElement(id: any, value: boolean) {
        (<HTMLInputElement>document.getElementById(id)).disabled = value;
    }

    swalShow(title: any, html: any, icon: any) {
        Swal.fire({
            title: title,
            html: html,
            icon: icon
        });
    }
    isEmpty(obj: any): boolean {
        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    }
    verifyErrorsModel(error: any): Array<string> {
        let arrayErrorValidations: Array<string>;
        for (let key in error) {
            if (error.hasOwnProperty(key)) {
                arrayErrorValidations = error[key]
            }
        }
        return arrayErrorValidations;
    }
    parseErrorsHtml(arrayErrorValidations: Array<string>): string {
        var listErrors = "<ul class='text-start'>";
        for (let i = 0; i < arrayErrorValidations.length; i++) {
            listErrors += "<li>" + arrayErrorValidations[i] + "</li>";
        }
        return listErrors.concat("</ul>");;
    }
    verifyStatusError(error: any) {
        switch (error.status) {
            case 400:
                this.swalShow(
                    "<h4>Operación no realizada</h4>",
                    error.error,
                    "warning"
                ); break;
            case 403 || 202:
                this.swalShow(
                    "<h4>Se ha denegado el acceso</h4>",
                    error.error,
                    "warning"
                ); break;
            case 422:
                this.swalShow(
                    "<h4>La solicitud no es válida</h4>",
                    this.parseErrorsHtml(this.verifyErrorsModel(error.error)),
                    "warning"
                ); break;
        }
    }
    manageErrors(error: any) {
        if (!this.isEmpty(error.error)) {
            this.verifyStatusError(error);
        }
    }
    convertFileBase64(file: File): Observable<string> {
        const result = new ReplaySubject<string>(1);
        const reader = new FileReader();
        reader.readAsBinaryString(file);
        reader.onload = (event) => result.next(btoa(event.target.result.toString()));
        return result;
    }
    static getAgeBirthDate(date: string): number {
        var birthDate = moment(date);
        var currentDate = moment();
        return currentDate.diff(birthDate, "years");
    }
    swalShowSuccess(message: string) {
        this.swalShow("<h4>Operación realizada</h4>", message, "success")
    }

}