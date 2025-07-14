import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthServiceService } from '../Service/auth-service.service';

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
      next: (data: string) => {
        console.log(data);
        this.router.navigate(['/seat']);
      },
      error: err => {
        console.error(err);
        this.error = 'Invalid username or password';
      }
    });
  }
}
