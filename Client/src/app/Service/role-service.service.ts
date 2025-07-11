import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PostRole, Role } from '../Models/role';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class RoleServiceService {

constructor(private http:HttpClient ) {}

localBaseUrl="http://localhost:39752/api/Role/"
  getRoleAllData(): Observable<any> {
    return this.http.get(this.localBaseUrl+"GetAllRoles");
  }


  getRoleByID(id:string): Observable<any> {
    return this.http.get(`${this.localBaseUrl}GetRoleById/${id}`);
  }



  postRoleData(payload: Role): Observable<any> {
    return this.http.post(`${this.localBaseUrl}CreateRole`, payload);
  }

  
updateRole(id: number, payload: Role): Observable<any> {
  return this.http.patch(`${this.localBaseUrl}UpdateRole/${id}`, payload);
}


deleteRole(id: number): Observable<any> {
  return this.http.delete(`${this.localBaseUrl}DeleteRole/${id}`);
}
}
