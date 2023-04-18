import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ListaSuscripcionesComponent } from './pages/lista-suscripciones/lista-suscripciones.component';

@NgModule({
  declarations: [
    ListaSuscripcionesComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers:[]
})
export class SuscripcionModule { }
