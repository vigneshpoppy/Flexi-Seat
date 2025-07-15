import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent implements OnInit {

  role: string = '';

goToEmployeeManagement() {
this.router.navigate(['/employeemanagement']);
}

  constructor(private router: Router) {}
  ngOnInit(): void {
    this.role = localStorage.getItem('roles') || '';
  }

  goToSeatManagement() {
    this.router.navigate(['/seatmanagement']);
  }

   goToRoleManagement() {
    this.router.navigate(['/rolemanagement']);
  }
   goToZoneManagement() {
    this.router.navigate(['/zonemanagement']);
  }

     goToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}

