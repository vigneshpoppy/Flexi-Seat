import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { NotificationService } from '../Service/notification.service';



import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';


@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent implements AfterViewInit, OnInit {
  @ViewChild('notifier') notifier!: NotificationComponent;

  userid: string | null = null;
  isLoginPage: boolean = false;
  showChatBot:boolean=false
  constructor(private notificationService: NotificationService, private router: Router) {}

  ngOnInit(): void {
    this.userid = localStorage.getItem('userid')?.toUpperCase() || null;

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.isLoginPage = this.router.url === '/login';
      });

      this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showChatBot = event.urlAfterRedirects.startsWith('/seat');
      }
  })
}

  ngAfterViewInit() {
    this.notificationService.register(this.notifier);
  }

  logout(): void {
    localStorage.clear();
    this.userid = null;
    this.router.navigate(['/login']);
  }
}
