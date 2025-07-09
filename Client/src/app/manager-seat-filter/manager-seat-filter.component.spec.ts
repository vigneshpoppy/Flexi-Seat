import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagerSeatFilterComponent } from './manager-seat-filter.component';

describe('ManagerSeatFilterComponent', () => {
  let component: ManagerSeatFilterComponent;
  let fixture: ComponentFixture<ManagerSeatFilterComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ManagerSeatFilterComponent]
    });
    fixture = TestBed.createComponent(ManagerSeatFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
