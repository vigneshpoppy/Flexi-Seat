import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationComponent } from '../notification/notification.component';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

constructor(private snackBar: MatSnackBar) {}

//   showSuccess(message: string, duration: number = 3000) {
//     this.snackBar.open(message, 'Close', {
//       duration,
//       panelClass: ['success-snackbar'],
//      horizontalPosition: 'center', // 👈 horizontally center
//   verticalPosition: 'top' 
//     });
//   }

//   showError(message: string, duration: number = 3000) {
//     this.snackBar.open(message, 'Close', {
//       duration,
//       panelClass: ['error-snackbar'],
//     horizontalPosition: 'center', // 👈 horizontally center
//   verticalPosition: 'top' 
//     });
//  }


 private notificationComponent!: NotificationComponent;

  register(notificationComponent: NotificationComponent) {
    this.notificationComponent = notificationComponent;
  }

  showSuccess(message: string) {
    this.notificationComponent?.show('success', message);
  }

  showError(message: string) {
    this.notificationComponent?.show('error', message);
  }

  showInfo(message: string) {
    this.notificationComponent?.show('info', message);
  }

  showWarning(message: string) {
    this.notificationComponent?.show('warning', message);
  }
}
