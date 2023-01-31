import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgotPageComponent } from './forgot-page.component';

describe('ForgotPageComponent', () => {
  let component: ForgotPageComponent;
  let fixture: ComponentFixture<ForgotPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ForgotPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgotPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
