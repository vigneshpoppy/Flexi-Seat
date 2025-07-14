import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PostZone } from '../Models/zone';

@Injectable({
  providedIn: 'root'
})
export class ZoneServiceService {

constructor(private http:HttpClient ) {}

localBaseUrl="http://localhost:39752/api/Zones/"
  getZoneAllData(): Observable<any> {
    return this.http.get(this.localBaseUrl+"All");
  }


  getZoneByID(id:string): Observable<any> {
    return this.http.get(`${this.localBaseUrl}GetZoneById/${id}`);
  }



  postData(payload: PostZone): Observable<any> {
    return this.http.post(`${this.localBaseUrl}CreateZone`, payload);
  }

  
updateZone(id: string, payload: PostZone): Observable<any> {
  return this.http.put(`${this.localBaseUrl}UpdateZoneById/${id}`, payload);
}


deleteZone(id: string): Observable<any> {
  return this.http.delete(`${this.localBaseUrl}DeleteZoneById/${id}`);
}

}
