import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAdminComponent } from './employee-admin.component';

describe('EmployeeAdminComponent', () => {
  let component: EmployeeAdminComponent;
  let fixture: ComponentFixture<EmployeeAdminComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EmployeeAdminComponent]
    });
    fixture = TestBed.createComponent(EmployeeAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
