import { Component, OnInit, OnDestroy } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SimpleModalComponent } from 'ngx-simple-modal';
import { forkJoin, of, Subject } from 'rxjs';
import { AlertModel } from 'src/app/core/models/alert-model';
import { Helpers } from 'src/app/helpers/helper';
import { UserService } from 'src/app/modules/user/services/user.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { PerfilService } from '../../services/perfil.service';
import { PersonaDto } from 'src/app/core/models/personaDto';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { FilePhotoViewModel } from 'src/app/core/models/view-models/filePhotoViewModel';
import { ScriptsService } from 'src/app/app-services/scripts.service';

@Component({
  selector: 'app-change-personalinfo-page',
  templateUrl: './change-personalinfo-page.component.html',
  styleUrls: ['./change-personalinfo-page.component.css']
})
export class ChangePersonalInfoPageComponent extends SimpleModalComponent<AlertModel, null> implements AlertModel, OnInit, OnDestroy {

  Title: string;
  Data: PersonaDto;
  stop$ = new Subject<void>();
  form: FormGroup;
  btnId = "btnUpdateInfo";
  spanFileName = "No se ha seleccionado ninguna foto";
  isChangeImage = false;
  imagePreview = null;
  imageBase64Btoa = null;

  constructor(
    private formBuilder: FormBuilder,
    private perfilService: PerfilService,
    private userService: UserService,
    private authService: AuthService,
    private helper: Helpers,
    private scriptService: ScriptsService
  ) {
    super();
    this.form = this.formBuilder.group({
      Id: ['', Validators.required],
      Nombre: ['', {
        validators: [Validators.compose([
          Validators.required,
          Validators.pattern(/^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$/)
        ])]
      }],
      Apellido: ['', {
        validators: [Validators.compose([
          Validators.required,
          Validators.pattern(/^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$/)
        ])]
      }],
      Telefono: ['', {
        validators: [Validators.compose([
          ValidationCustom.isValidNumDigitsPhone,
          Validators.pattern(/^\d*$/)
        ])]
      }],
      FechaNacimiento: ['', Validators.required],
      FileTemporal: ['']
    });

  }

  ngOnInit(): void {
    this.form.enable();
    this.form.patchValue(this.Data);
    this.loadScript();
    this.imagePreview = this.Data.Foto;
  }

  ngOnDestroy() {
    this.form.reset();
    this.stop$.next();
    this.stop$.complete();
    this.clearVariables();
    this.scriptService.removeScript('img-modal-script');
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }

  loadScript() {
    this.scriptService.loadScript({
      id: 'img-modal-script',
      url: 'assets/js/img-modal.js'
    });
  }

  updatePersonalInfo() {
    this.form.disable();
    this.helper.disableInputElement(this.btnId, true);
    var personDto: PersonaDto = this.form.value;
    const { Foto, ...prevPerfil } = this.Data;
    const { FileTemporal, ...currPerfil } = this.form.value;
    const id = this.authService.getIdUserLocalStorage();
    var filePhoto: FilePhotoViewModel = { Id: id, Photo: this.imageBase64Btoa };
    const isChangePersonalInfo = this.helper.compareTwoObjects(prevPerfil, currPerfil);
    forkJoin([
      this.isChangeImage ? this.perfilService.updatePhoto(filePhoto) : of(null),
      !isChangePersonalInfo ? this.perfilService.updatePerson(personDto, id) : of(null)
    ]).pipe(takeUntil(this.stop$))
      .subscribe(res => {
        (res[0] !== null && res[1] === null) ? this.helper.swalShowSuccess(res[0].Message) : null;
        (res[0] !== null && res[1] !== null) ? this.helper.swalShowSuccess(res[0].Message + "<br/>" + res[1].Message) : null;
        (res[0] === null && res[1] !== null) ? this.helper.swalShowSuccess(res[1].Message) : null;
        this.perfilService.emitChanges(true);
        this.close();
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnId, false);
          this.form.enable();
          this.close();
        });

  }

  onFileChanged(event: any) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.spanFileName = file.name;
      var valid = ValidationCustom.isAvailableFormatAndSize(file, this.f.FileTemporal);
      if (valid === null) {
        this.helper.convertFileBase64(file).subscribe(res => {
          this.imageBase64Btoa = res;
          this.isChangeImage = true;
          this.imagePreview = "data:image/png;base64," + res;
        });
      }
    }
  }

  availablePhone(): void {
    if (this.Data.Telefono !== this.f.Telefono.value) {
      this.availableInfo(
        this.f.Telefono,
        ValidationCustom.isAvailablePhone(this.stop$, this.userService)
      );
    }
  }

  availableDateBirth() {
    if (this.f.FechaNacimiento.value !== this.Data.FechaNacimiento) {
      this.availableInfo(
        this.f.FechaNacimiento,
        ValidationCustom.isAvailableDateBirth(this.stop$, this.userService)
      );
    }
  }

  availableInfo(ctrl: AbstractControl, fnValidator: AsyncValidatorFn): void {
    ctrl.setAsyncValidators(fnValidator);
    ctrl.updateValueAndValidity();
    ctrl.clearAsyncValidators();
  }

  clearVariables() {
    this.imagePreview = null;
    this.isChangeImage = false;
    this.imageBase64Btoa = null;
  }
}
