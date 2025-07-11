import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  adid = '';
  error = '';
  success = '';

  constructor(private router: Router) {}

  resetPassword() {
    this.error = '';
    this.success = '';

    if (!this.adid.trim()) {
      this.error = 'ADID is required';
      return;
    }

    // ✅ Simulated reset logic
    this.success = 'Password reset request sent. Redirecting to login...';

    setTimeout(() => {
      this.router.navigate(['/login']);
    }, 2000);
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
