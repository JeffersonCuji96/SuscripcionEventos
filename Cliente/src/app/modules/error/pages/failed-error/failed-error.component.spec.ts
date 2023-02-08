import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FailedErrorComponent } from './failed-error.component';

describe('FailedErrorComponent', () => {
  let component: FailedErrorComponent;
  let fixture: ComponentFixture<FailedErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FailedErrorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FailedErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
