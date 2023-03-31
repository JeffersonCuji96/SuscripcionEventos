import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListaEventosComponent } from '../evento/pages/lista-eventos/lista-eventos.component';

const routes: Routes = [
  {
    path:'',
    component:ListaEventosComponent,
    data:{
      //Se usa esta bandera para indicar la reutilización en esta ruta
      reuseRoute:true
    }
  },
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
