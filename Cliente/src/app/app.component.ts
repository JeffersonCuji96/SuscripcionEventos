import { Component } from '@angular/core';
import { NavigationStart, Router, Event as NavigationEvent } from '@angular/router';
import { HomeService } from './modules/home/services/home.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Eventos';
  event$: any;
  constructor(private router: Router, private homeService: HomeService) {
    this.event$ = this.router.events
      .subscribe(
        (event: NavigationEvent) => {
          if (event instanceof NavigationStart) {

            if (event.url === "/") {
              this.homeService.enableJumbotron(true);
            }
          }
        });
  }

  ngOnDestroy() {
    this.event$.unsubscribe();
  }
}
