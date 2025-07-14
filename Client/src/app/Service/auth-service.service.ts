import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, map } from 'rxjs';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  accessToken: string;   // <-- whatever your backend returns
  roles: string[];       // e.g. ["admin","editor"]
}

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {
  private readonly tokenKey = 'access-token';
  private readonly roleKey  = 'roles';

  constructor(private http: HttpClient, private router: Router) {}

  /** Sends credentials to the API and stores everything that comes back. */
  login(username: string, password: string): Observable<string> {
    const payload: LoginRequest = { username, password };

    return this.http.post<string>('http://localhost:39752/api/Login', payload);
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
    return JSON.parse(localStorage.getItem(this.roleKey) || '');
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
