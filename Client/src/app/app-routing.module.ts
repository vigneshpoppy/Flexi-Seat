import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';   
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ChangePasswordComponent } from './change-password/change-password.component';     
import { ResetPasswordComponent } from './reset-password/reset-password.component';                      
import { SeatAdminComponent } from './seat-admin/seat-admin.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { EmployeeAdminComponent } from './employee-admin/employee-admin.component';
import { AppComponent } from './app.component';
import { CheckinClientComponent } from './checkin-client/checkin-client.component';
import { RoleAdminComponent } from './role-admin/role-admin.component';
import { ZoneAdminComponent } from './zone-admin/zone-admin.component';
import { authGuard } from './Service/Gaurds/auth.guard';
import { ZoneViewerComponent } from './zone-viewer/zone-viewer.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { TicketRaisingComponent } from './ticket-raising/ticket-raising.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },  // Default route → login
  { path: 'login', component: LoginComponent },
  { path: 'change-password', component: ChangePasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'seatmanagement', component: SeatAdminComponent  , canActivate: [authGuard],
    data: { roles: ['admin'] }},
  { path: 'rolemanagement', component: RoleAdminComponent , canActivate: [authGuard],
    data: { roles: ['admin'] }},
  { path: 'zonemanagement', component: ZoneAdminComponent , canActivate: [authGuard],
    data: { roles: ['admin'] }},
  { path: 'admin', component: AdminPageComponent , canActivate: [authGuard],
    data: { roles: ['admin'] }},
  { path: 'employeemanagement', component: EmployeeAdminComponent , canActivate: [authGuard],
    data: { roles: ['admin','supervisor-1','supervisor-2'] }},
  { path: 'seat', component: AppComponent },
  { path: 'checkin-client', component: CheckinClientComponent },
  { path: 'zone-viewer', component: ZoneViewerComponent },
  { path: 'dashboard', component: AdminDashboardComponent },
  { path: 'raiseticket', component: TicketRaisingComponent},
  { path: '**', redirectTo: '/login' }  // Wildcard to catch undefined routes
];

@NgModule({
  imports: [RouterModule.forRoot(routes),FormsModule],
  exports: [RouterModule]
})
export class AppRoutingModule { }
