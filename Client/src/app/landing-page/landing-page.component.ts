import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { NotificationService } from '../Service/notification.service';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent implements AfterViewInit,OnInit {
  
showChatBot:boolean=false;
  @ViewChild('notifier') notifier!: NotificationComponent;
constructor(private notificationService: NotificationService,private router:Router) {}
  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showChatBot = event.urlAfterRedirects.startsWith('/seat');
      }
    });
  }

  ngAfterViewInit() {
    this.notificationService.register(this.notifier);
  }


}
