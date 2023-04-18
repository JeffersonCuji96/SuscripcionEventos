import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaSuscripcionesComponent } from './lista-suscripciones.component';

describe('ListaSuscripcionesComponent', () => {
  let component: ListaSuscripcionesComponent;
  let fixture: ComponentFixture<ListaSuscripcionesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListaSuscripcionesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListaSuscripcionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
