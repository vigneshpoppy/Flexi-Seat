import { Component } from '@angular/core';
import { Router } from '@angular/router'; // ✅ Import Router

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  oldPassword = '';
  newPassword = '';
  confirmPassword = '';
  error = '';
  success = '';

  constructor(private router: Router) {}  // ✅ Inject Router

  changePassword() {
    this.error = '';
    this.success = '';

    if (!this.oldPassword || !this.newPassword || !this.confirmPassword) {
      this.error = 'All fields are required';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.error = 'New passwords do not match';
      return;
    }

    // ✅ Simulate password change logic (replace with real API call)
    if (this.oldPassword === 'admin') {
      this.success = 'Password changed successfully! Redirecting to login...';

      // ✅ Redirect to login after 2 seconds
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
    } else {
      this.error = 'Old password is incorrect';
    }
  }
}
