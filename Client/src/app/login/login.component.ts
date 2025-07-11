import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  // ✅ Declare these variables
  username: string = '';
  password: string = '';
  error: string = '';

  constructor(private router: Router) {}

  login() {
    if (this.username === 'admin' && this.password === 'admin') {
      this.router.navigate(['/seat']);
    } else {
      this.error = 'Invalid username or password';
    }
  }
}
