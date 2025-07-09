import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeatAdminComponent } from './seat-admin.component';

describe('SeatAdminComponent', () => {
  let component: SeatAdminComponent;
  let fixture: ComponentFixture<SeatAdminComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SeatAdminComponent]
    });
    fixture = TestBed.createComponent(SeatAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
