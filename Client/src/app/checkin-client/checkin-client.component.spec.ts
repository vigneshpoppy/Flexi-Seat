import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckinClientComponent } from './checkin-client.component';

describe('CheckinClientComponent', () => {
  let component: CheckinClientComponent;
  let fixture: ComponentFixture<CheckinClientComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CheckinClientComponent]
    });
    fixture = TestBed.createComponent(CheckinClientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
