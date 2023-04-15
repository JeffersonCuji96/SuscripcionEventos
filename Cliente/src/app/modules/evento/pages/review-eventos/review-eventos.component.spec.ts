import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewEventosComponent } from './review-eventos.component';

describe('ReviewEventosComponent', () => {
  let component: ReviewEventosComponent;
  let fixture: ComponentFixture<ReviewEventosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReviewEventosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReviewEventosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
