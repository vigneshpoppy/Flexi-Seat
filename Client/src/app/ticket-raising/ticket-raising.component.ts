import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TicketRaisingService } from './ticket-raising.service';
 
@Component({
  selector: 'app-ticket-raising',
  templateUrl: './ticket-raising.component.html',
  styleUrls: ['./ticket-raising.component.css']
})
export class TicketRaisingComponent {
  ticketForm: FormGroup;
  ticketRaised = false;
 
  constructor(private fb: FormBuilder, private ticketService: TicketRaisingService) {
    this.ticketForm = this.fb.group({
      message: ['', [Validators.required, Validators.minLength(10)]]
    });
  }
 
  generateTicketId(): string {
  const prefix = 'TCKT';
  const date = new Date().toISOString().slice(0, 10).replace(/-/g, '');
  const random = Math.random().toString(36).substring(2, 8).toUpperCase();
  return `${prefix}-${date}-${random}`;
}
 
 
  raiseTicket(): void {
  if (this.ticketForm.valid) {
    const body = this.ticketForm.value.message;
    const id = this.generateTicketId();
    const adid = localStorage.getItem('userid')?.toUpperCase();
 
    const ticket = { id, body, adid };
 
    this.ticketService.raiseTicket(ticket).subscribe({
      next: () => {
        this.ticketRaised = true;
        this.ticketForm.reset();
      },
      error: err => console.error('Error:', err)
    });
  }
}
 
 
  get messageControl() {
    return this.ticketForm.get('message');
  }
}