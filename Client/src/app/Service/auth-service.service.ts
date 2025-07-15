import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginRequest {
  adid: string;
  password: string;
}

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {
  private readonly tokenKey = 'access-token';
  private readonly roleKey  = 'roles';

  constructor(private http: HttpClient, private router: Router) {}

 login(username: string, password: string): Observable<LoginResponse> {
  const payload = { adid: username, password: password };
  return this.http.post<LoginResponse>(`${environment.apiUrl}/api/Login/Login`, payload);
}

  /** Local‑storage helpers */
  private setRoles(roles: string[]) {
    localStorage.setItem(this.roleKey, JSON.stringify(roles));
  }
  private setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }

  /** Accessors */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
  getRoles(): string {
    return localStorage.getItem(this.roleKey) || '';
  }

  /** Session helpers */
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
    this.router.navigate(['/login']);
  }
  isAuthenticated(): boolean {
    return !!this.getToken();               // add expiry‑check if needed
  }
  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }
}
