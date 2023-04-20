import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SessionGuard } from 'src/app/core/guards/session.guard';
import { DetailEventoComponent } from './pages/detail-evento/detail-evento.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { ReviewEventosComponent } from './pages/review-eventos/review-eventos.component';

const routes: Routes = [
  {
    path: 'register',
    canActivate: [SessionGuard],
    component: RegisterPageComponent
  },
  {
    path:'revision',
    canActivate: [SessionGuard],
    component:ReviewEventosComponent
  },
  {
    path:'detail/:data',
    canActivate: [SessionGuard],
    component:DetailEventoComponent
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
export class EventoRoutingModule { }
