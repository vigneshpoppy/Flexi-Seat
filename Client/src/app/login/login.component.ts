import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthServiceService } from '../Service/auth-service.service';

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

  constructor(private router: Router,private authservice:AuthServiceService) {}

  login() {
    if (this.username === 'admin' && this.password === 'admin') {

      this.authservice.Login(this.username,this.password);
      this.router.navigate(['/seat']);
    } else {
      this.error = 'Invalid username or password';
    }
  }
}
