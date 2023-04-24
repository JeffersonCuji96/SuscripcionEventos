import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListaSuscripcionesComponent } from './pages/lista-suscripciones/lista-suscripciones.component';
import { SessionGuard } from 'src/app/core/guards/session.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [SessionGuard],
    component: ListaSuscripcionesComponent
  },
  {
    path: '**',
    redirectTo: '/error/404'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SuscripcionRoutingModule { }
