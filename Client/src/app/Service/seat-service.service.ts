import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SeatServiceService {

  constructor(private http:HttpClient ) {}


  getData(): Observable<any> {
    return this.http.get('https://api.example.com/data');
  }


  postData(payload: any): Observable<any> {
    return this.http.post('https://api.example.com/data', payload);
  }
}
