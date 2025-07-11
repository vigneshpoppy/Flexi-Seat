import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor(private http:HttpClient ) {}
  
    localBaseUrl="http://localhost:39752/api/Users/"
    getRoleAllData(): Observable<any> {
        return this.http.get(this.localBaseUrl+"All");
      }
  

    getRoleByID(id:string): Observable<any> {
      return this.http.get(`${this.localBaseUrl}${id}`);
    }
  
  
  
   
  }
