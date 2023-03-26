import { NgModule } from '@angular/core';
import { SanitizerPipe } from './helpers/pipes/sanitize-url';
import { UpdateRowsPipe } from './helpers/pipes/update-rows';
@NgModule({
    declarations: [
        SanitizerPipe,
        UpdateRowsPipe
    ],
    imports: [],
    exports: [
        SanitizerPipe,
        UpdateRowsPipe
    ]
})
export class PipeModule { }