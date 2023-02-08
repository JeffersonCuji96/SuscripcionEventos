import { TestBed } from '@angular/core/testing';

import { ForgotGuard } from './forgot.guard';

describe('ForgotGuard', () => {
  let guard: ForgotGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ForgotGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
