import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { individualSeatReservation } from '../Models/reservation';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor(private http:HttpClient ) {}
  
    localBaseUrl="http://localhost:39752/api/Reservations/"
  

    getSeatsbyZone(zoneId:string,date:string): Observable<any> {
      return this.http.get(`${this.localBaseUrl}zone/${zoneId}/seats?date=${date}`);
    }
  
  individualReservation(payload:individualSeatReservation){
    console.log("post call")
    console.log(payload)
     return this.http.post(`${this.localBaseUrl}Create`, payload);
  }

   bulkReservation(payload:individualSeatReservation[]){
    console.log("post call")
    console.log(payload)
     return this.http.post(`${this.localBaseUrl}BulkCreate`, payload);
  }
  
   
  }
