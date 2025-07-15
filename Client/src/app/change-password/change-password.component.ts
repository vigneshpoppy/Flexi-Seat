import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthServiceService } from '../Service/auth-service.service';
import { NotificationService } from '../Service/notification.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  adid = '';
  oldPassword = '';
  newPassword = '';
  error = '';
  success = '';

  constructor(
    private router: Router,
    private authService: AuthServiceService,
    private notify: NotificationService
  ) {}

  changePassword() {
    this.error = '';
    this.success = '';

    if (!this.adid.trim() || !this.oldPassword || !this.newPassword) {
      this.notify.showError('All fields are required');
      return;
    }

    this.authService.changePassword(this.adid, this.oldPassword, this.newPassword).subscribe({
      next: (res) => {
        this.notify.showSuccess(res.message || 'Password changed successfully!');
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        console.error('Password change error:', err);
        this.notify.showError(err?.error?.message || 'Password change failed');
      }
    });
  }
}
