import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Helpers } from 'src/app/helpers/helper';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {

  message = "CONFIRMANDO LA CUENTA";
  constructor(
    private authService: AuthService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private helper: Helpers) { }

  ngOnInit(): void {
    this.actRoute.params.subscribe(
      (params: Params) => {
        this.authService.confirmEmail({Token:params.token}).subscribe(
          (res: any) => {
            if (!res) {
              this.router.navigate(['/error/401']);
            } else {
              this.helper.swalShowSuccess("Su cuenta ha sido verificada!! Ya puede iniciar sesión");
              this.router.navigate(['/auth/login']);
            }
          },
          error => {
            this.helper.manageErrors(error);
            this.router.navigate(['/error/401']);
          });
      }
    );
  }

}
