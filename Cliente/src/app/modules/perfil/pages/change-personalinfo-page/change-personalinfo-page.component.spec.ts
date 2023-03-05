import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangePersonalinfoPageComponent } from './change-personalinfo-page.component';

describe('ChangePersonalinfoPageComponent', () => {
  let component: ChangePersonalinfoPageComponent;
  let fixture: ComponentFixture<ChangePersonalinfoPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangePersonalinfoPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangePersonalinfoPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
