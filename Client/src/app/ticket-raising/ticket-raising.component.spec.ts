import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketRaisingComponent } from './ticket-raising.component';

describe('TicketRaisingComponent', () => {
  let component: TicketRaisingComponent;
  let fixture: ComponentFixture<TicketRaisingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TicketRaisingComponent]
    });
    fixture = TestBed.createComponent(TicketRaisingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
