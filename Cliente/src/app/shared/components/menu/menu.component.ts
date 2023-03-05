import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  public fullName:string;
  constructor(
    private authService:AuthService,
    private router:Router
    ) {
      this.fullName=authService.getFullName();
     }

  ngOnInit(): void {
  }
  logout() {
    this.authService.removeSesion();
    this.router.navigate(['/', 'auth']);
  }

}
