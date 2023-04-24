import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  public fullName:string;
  maxWidth = 992;
  @ViewChild('menuDesplegable') menuDesplegable!: ElementRef;

  constructor(
    private authService:AuthService
    ) {
      this.fullName=authService.getFullName();
     }

  ngOnInit(): void {}

  logout() {
    this.closeToggler();
    this.authService.removeSesion();
    window.location.href="/auth/login";
  }

  closeToggler(): void {
    const windowWidth = window.innerWidth;
    if (windowWidth < this.maxWidth) {
      this.menuDesplegable.nativeElement.click();
    }
  }
}
