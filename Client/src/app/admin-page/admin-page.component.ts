import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent {


goToEmployeeManagement() {
this.router.navigate(['/employeemanagement']);
}

  constructor(private router: Router) {}

  goToSeatManagement() {
    this.router.navigate(['/seatmanagement']);
  }
}
