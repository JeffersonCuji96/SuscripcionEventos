import { Directive, HostListener } from "@angular/core";
import { NgControl } from "@angular/forms";

@Directive({
    selector: '[formatTime]'
})
export class DirectiveFormatTime {

    constructor(private ngControl: NgControl) { }
    @HostListener('blur') format() {
        if(!this.ngControl.errors) {
            let time=this.ngControl.value.substring(0, 5);
            this.ngControl.control.setValue(time.concat(":00"));
        }
    }
}