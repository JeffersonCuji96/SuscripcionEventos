import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path:'evento',
    loadChildren:()=>import(`../evento/evento.module`).then(m=>m.EventoModule)
  },
  {
    path:'perfil',
    loadChildren:()=>import(`../perfil/perfil.module`).then(m=>m.PerfilModule)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
