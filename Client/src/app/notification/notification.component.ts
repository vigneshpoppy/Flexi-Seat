import { Component } from '@angular/core';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent {
messages: { type: string, text: string }[] = [];

  show(type: 'success' | 'error' | 'info' | 'warning', text: string) {
    this.messages.push({ type, text });

    // Auto-dismiss after 3 seconds
    setTimeout(() => {
      this.messages.shift();
    }, 3000);
  }
}
