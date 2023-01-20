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
            case 422:
                this.swalShow(
                    "<h4>La solicitud no es válida</h4>",
                    this.parseErrorsHtml(this.verifyErrorsModel(error.error)),
                    "warning"
                ); break;
            case 403:
                this.swalShow(
                    "<h4>Se ha denegado el acceso</h4>",
                    error.error,
                    "warning"
                ); break;
            case 0:
                this.swalShow(
                    "<h4>Operación no realizada</h4>",
                    "La conexión ha fallado!. Revise su acceso a internet o intentelo más tarde",
                    "error"
                ); break;
            default:
                this.swalShow(
                    "<h4>Operación no realizada</h4>",
                    "Internal server error",
                    "error"
                ); break;
        }
    }
    manageErrors(error: any) {
        if (!this.isEmpty(error.error)) {
            this.verifyStatusError(error);
        }
    }
}