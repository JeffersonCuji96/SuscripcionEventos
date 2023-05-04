import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { Helpers } from './helpers/helper';
import { CookieService } from 'ngx-cookie-service';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpConfigInterceptor } from './core/interceptors/http-config.interceptor';
import { ScriptsService } from './app-services/scripts.service';
import { RouteReuseStrategy } from '@angular/router';
import { CustomRouteReuseStrategy } from './app-services/CustomRouteReuseStrategy';
import { NgxScrollPositionRestorationModule } from 'ngx-scroll-position-restoration';
import { HubService } from './app-services/hub.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    NgxScrollPositionRestorationModule.forRoot() //Guarda la posicion del scroll al regresar a la página anterior
  ],
  providers: [CookieService,Helpers,ScriptsService,HubService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpConfigInterceptor,
      multi: true
    },
    {
      provide: RouteReuseStrategy,
      useClass: CustomRouteReuseStrategy
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
