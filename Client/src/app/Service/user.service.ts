import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http:HttpClient ) {}
  
  
    localBaseUrl=`${environment.apiUrl}/api/Users/`
      // getSeatAllData(): Observable<any> {
      //   return this.http.get(this.localBaseUrl+"GetAll");
      // }
    
    
      getTeamMembersByManagerID(id:string): Observable<any> {
        return this.http.get(`${this.localBaseUrl}manager/${id}`);
      }
    
    
    
    //   postseatData(payload: Seatpostpayload): Observable<any> {
    //     return this.http.post(`${this.localBaseUrl}Create`, payload);
    //   }
    
      
    // updateSeat(id: string, payload: SeatPatchpayload): Observable<any> {
    //   return this.http.patch(`${this.localBaseUrl}Update/${id}`, payload);
    // }
    
    
    deleteSeat(id: string): Observable<any> {
      return this.http.delete(`${this.localBaseUrl}${id}`);
    }
}
