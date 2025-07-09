import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  constructor(private http:HttpClient) { }


    private roles: string[] = [];
  Login(username:string ,password:string){
    var payload={
      username:username,
      password:password
    }
     var response= this.http.post('https://api.example.com/data', payload);
     
  }
  setRoles(roles: string[]) {
    this.roles = roles;
    localStorage.setItem('roles', JSON.stringify(roles));
  }

  getRoles(): string[] {
    return this.roles.length ? this.roles : JSON.parse(localStorage.getItem('roles') || '[]');
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }
}
