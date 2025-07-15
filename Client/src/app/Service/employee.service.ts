import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee, User, UserPatch } from '../Models/employee';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

constructor(private http:HttpClient ) {}

  localBaseUrl=`${environment.apiUrl}/api/Users/`
    getRoleAllData(): Observable<any> {
      return this.http.get(this.localBaseUrl+"All");
    }
  

    
  
  
    getRoleByID(id:string): Observable<any> {
      return this.http.get(`${this.localBaseUrl}${id}`);
    }
  
  
  
    postRoleData(payload: User): Observable<any> {
      return this.http.post(`${this.localBaseUrl}Create`, payload);
    }
  
    
  updateRole(id: string, payload: UserPatch): Observable<any> {
    return this.http.patch(`${this.localBaseUrl}Update/${id}`, payload);
  }
  
  
  deleteRole(id: string): Observable<any> {
    return this.http.delete(`${this.localBaseUrl}${id}`);
  }
}
