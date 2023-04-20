import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  public fullName:string;
  constructor(
    private authService:AuthService
    ) {
      this.fullName=authService.getFullName();
     }

  ngOnInit(): void {
  }
  logout() {
    this.authService.removeSesion();
    window.location.href="/auth/login";
  }

}
