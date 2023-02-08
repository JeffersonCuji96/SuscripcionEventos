import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecoveryErrorComponent } from './recovery-error.component';

describe('RecoveryErrorComponent', () => {
  let component: RecoveryErrorComponent;
  let fixture: ComponentFixture<RecoveryErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RecoveryErrorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecoveryErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
