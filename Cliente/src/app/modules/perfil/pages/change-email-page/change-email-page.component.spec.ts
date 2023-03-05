import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeEmailPageComponent } from './change-email-page.component';

describe('ChangeEmailPageComponent', () => {
  let component: ChangeEmailPageComponent;
  let fixture: ComponentFixture<ChangeEmailPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeEmailPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeEmailPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
