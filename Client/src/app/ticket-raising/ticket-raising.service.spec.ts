import { TestBed } from '@angular/core/testing';

import { TicketRaisingService } from './ticket-raising.service';

describe('TicketRaisingService', () => {
  let service: TicketRaisingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TicketRaisingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
