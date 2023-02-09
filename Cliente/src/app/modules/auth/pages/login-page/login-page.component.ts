import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccessDto } from 'src/app/core/models/accessDto';
import { AuthService } from '../../services/auth.service';
import { Helpers } from 'src/app/helpers/helper';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})

export class LoginPageComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public form: FormGroup;
  public isChecked = false;
  public isEnableLink=false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private helper: Helpers) {

    this.form = this.formBuilder.group({
      Email: ['', Validators.required],
      Clave: ['', Validators.required]
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  login(): void {
    const btnId="btnLogin";
    this.form.disable();
    this.helper.disableInputElement(btnId, true);
    this.isEnableLink=true;
    var usuario: AccessDto = this.form.value;
    this.authService.login(usuario)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.authService.setCookieService(response);
        this.authService.setIdUserLocalStorage(response.IdUsuario);
        this.router.navigate(['/']);
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(btnId, false);
          this.isEnableLink=false;
          this.form.enable();
        });

  }

}
