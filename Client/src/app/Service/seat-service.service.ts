import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SeatPatchpayload, Seatpostpayload } from '../Models/seat';

@Injectable({
  providedIn: 'root'
})
export class SeatServiceService {

  constructor(private http:HttpClient ) {}


  localBaseUrl="http://localhost:39752/api/Seats/"
    getSeatAllData(): Observable<any> {
      return this.http.get(this.localBaseUrl+"GetAll");
    }
  
  
    getseatByID(id:string): Observable<any> {
      return this.http.get(`${this.localBaseUrl}${id}`);
    }
  
  
  
    postseatData(payload: Seatpostpayload): Observable<any> {
      return this.http.post(`${this.localBaseUrl}Create`, payload);
    }
  
    
  updateSeat(id: string, payload: SeatPatchpayload): Observable<any> {
    return this.http.patch(`${this.localBaseUrl}Update/${id}`, payload);
  }
  
  
  deleteSeat(id: string): Observable<any> {
    return this.http.delete(`${this.localBaseUrl}${id}`);
  }
}
