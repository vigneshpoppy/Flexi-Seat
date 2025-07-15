import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthServiceService } from '../Service/auth-service.service';
import { NotificationService } from '../Service/notification.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  adid = '';
  error = '';
  success = '';

  constructor(
    private router: Router,
    private authService: AuthServiceService,private notify:NotificationService
  ) {}

  resetPassword() {
    this.error = '';
    this.success = '';

    if (!this.adid.trim()) {
      this.notify.showError('ADID is required');
      return;
    }

    this.authService.ResetPassword(this.adid).subscribe({
      next: (response) => {
        this.notify.showSuccess(response.message);
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        console.error('Reset error:', err);
        this.notify.showError(err?.error?.message);
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
