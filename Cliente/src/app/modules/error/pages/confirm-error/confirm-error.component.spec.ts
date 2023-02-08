import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmErrorComponent } from './confirm-error.component';

describe('ConfirmErrorComponent', () => {
  let component: ConfirmErrorComponent;
  let fixture: ComponentFixture<ConfirmErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmErrorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
