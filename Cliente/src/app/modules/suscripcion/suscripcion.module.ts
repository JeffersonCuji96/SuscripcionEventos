import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ListaSuscripcionesComponent } from './pages/lista-suscripciones/lista-suscripciones.component';
import { SuscripcionRoutingModule } from './suscripcion-routing.module';
import { EventoModule } from '../evento/evento.module';

@NgModule({
  declarations: [
    ListaSuscripcionesComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SuscripcionRoutingModule,
    EventoModule
  ],
  providers:[]
})
export class SuscripcionModule { }
