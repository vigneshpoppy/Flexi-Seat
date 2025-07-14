import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
 
export interface Ticket {
  id: string;
  body: string;
  adid: string |undefined;
}
 
@Injectable({
  providedIn: 'root'
})
export class TicketRaisingService {
  private apiUrl = 'http://localhost:5000/api/tickets';
 
  constructor(private http: HttpClient) {}
 
  raiseTicket(ticket: Ticket): Observable<any> {
  return this.http.post(this.apiUrl, ticket);
}
}