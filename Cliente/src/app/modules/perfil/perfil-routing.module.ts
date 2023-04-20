import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PerfilPageComponent } from './pages/perfil-page/perfil-page.component';
import { SessionGuard } from 'src/app/core/guards/session.guard';

const routes: Routes = [
  {
    path: 'update',
    canActivate: [SessionGuard],
    component: PerfilPageComponent
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
export class PerfilRoutingModule { }
