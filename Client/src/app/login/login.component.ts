import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthServiceService } from '../Service/auth-service.service';
import { jwtDecode } from 'jwt-decode';
//import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  error   = '';

  constructor(private auth: AuthServiceService, private router: Router) {}

  login(): void {
    if (!this.username.trim() || !this.password) {
      this.error = 'Please enter username and password';
      return;
    }

   this.auth.login(this.username, this.password).subscribe({
    next: (res) => {
      console.log('Token:', res.token);
      localStorage.setItem('access-token', res.token);  // Or use a method in your AuthService
      // this.jwtHelper.isTokenExpired(res.token);
      const tokenPayload = jwtDecode<any>(res.token);
          const userRole = tokenPayload["Role"].toLowerCase();
          const managerId = tokenPayload["Manager ADID"];
          const userId = tokenPayload["nameid"];
          localStorage.setItem('token',res.token);
          localStorage.setItem('roles',userRole);
          localStorage.setItem('manageradid',managerId);
          localStorage.setItem('userid',userId);
          const userDetails: string[] = ['admin', 'supervisor-1', 'supervisor-2'];
          if (userDetails.includes(userRole)) {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['/seat']);
          }
//       manageradid
// userid
    },
    error: err => {
      console.error('Login error:', err);
      this.error = 'Invalid username or password';
    }
  });
}
}
