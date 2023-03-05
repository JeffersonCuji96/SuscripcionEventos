import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PerfilPageComponent } from './pages/perfil-page/perfil-page.component';

const routes: Routes = [
  {
    path: 'update',
    component: PerfilPageComponent
  },
  {
    path: '**',
    redirectTo: '/perfil/update'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PerfilRoutingModule { }
