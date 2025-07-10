import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { NotificationService } from '../Service/notification.service';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent implements AfterViewInit {
  
  @ViewChild('notifier') notifier!: NotificationComponent;
constructor(private notificationService: NotificationService) {}

  ngAfterViewInit() {
    this.notificationService.register(this.notifier);
  }


}
