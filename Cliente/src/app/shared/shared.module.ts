import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FooterComponent } from './components/footer/footer.component';
import { MenuComponent } from './components/menu/menu.component';
import { JumbotronComponent } from './components/jumbotron/jumbotron.component';
import { LoaderComponent } from './components/loader/loader.component';

@NgModule({
  declarations: [
    FooterComponent,
    MenuComponent,
    JumbotronComponent,
    LoaderComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    FooterComponent,
    MenuComponent,
    JumbotronComponent,
    LoaderComponent
  ]
})
export class SharedModule { }
