import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailEventoComponent } from './detail-evento.component';

describe('DetailEventoComponent', () => {
  let component: DetailEventoComponent;
  let fixture: ComponentFixture<DetailEventoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailEventoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailEventoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
