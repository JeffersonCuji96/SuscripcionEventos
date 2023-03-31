import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SessionGuard } from 'src/app/core/guards/session.guard';
import { DetailEventoComponent } from './pages/detail-evento/detail-evento.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';

const routes: Routes = [
  {
    path: 'register',
    canActivate: [SessionGuard],
    component: RegisterPageComponent
  },
  {
    path:'detail/:data',
    component:DetailEventoComponent
  },
  {
    path: '**',
    redirectTo: '/evento/register'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventoRoutingModule { }
