import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Role } from '../Models/role';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrgpoolService {


constructor(private http:HttpClient ) {}
localBaseUrl=`${environment.apiUrl}/api/OrgSeatPools/`
  getOrgAllData(): Observable<any> {
    return this.http.get(this.localBaseUrl+"ALl");
  }


  getOrgByID(id:string): Observable<any> {
    return this.http.get(`${this.localBaseUrl}${id}`);
  }



  postOrgData(payload: Role): Observable<any> {
    return this.http.post(`${this.localBaseUrl}Create`, payload);
  }

  
updateOrg(id: number, payload: Role): Observable<any> {
  return this.http.patch(`${this.localBaseUrl}Update/${id}`, payload);
}


deleteOrg(id: number): Observable<any> {
  return this.http.delete(`${this.localBaseUrl}${id}`);
}
}
