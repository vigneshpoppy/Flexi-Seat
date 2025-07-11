import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode, JwtPayload } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  constructor(private http:HttpClient,private router:Router) { }


    private roles: string ='';

    tokenKey="access-token";
    roleKey="roles"
  Login(username:string ,password:string){
    var payload={
      username:username,
      password:password
    }
    const role="Admin"
    this.setRoles(role);
     var response= this.http.post('https://api.example.com/data', payload);
     
  }
  setRoles(roles: string) {
    this.roles = roles;
    localStorage.setItem(this.roleKey, JSON.stringify(roles));
  }


  setToken(token:string){
    localStorage.setItem(this.tokenKey,token);
  }

   logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
    this.router.navigate(['/login']);
  }

 
  getRoles(): string {
    return this.roles.length ? this.roles : JSON.parse(localStorage.getItem(this.roleKey) || '');
  }


  getToken(): string | null {
  return localStorage.getItem(this.tokenKey);
}

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
  if (!token) return true;

  try {
    const decoded = jwtDecode<JwtPayload>(token);

    
    if (!decoded.exp) return true;

    const now = Date.now().valueOf() / 1000;
    return decoded.exp < now;
  } catch (error) {
    return true; 
  }
}

 isAuthenticated(): boolean {
  return !!this.getToken() && !this.isTokenExpired();
}

}


 