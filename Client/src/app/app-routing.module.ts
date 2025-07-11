import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SeatAdminComponent } from './seat-admin/seat-admin.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { EmployeeAdminComponent } from './employee-admin/employee-admin.component';
import { AppComponent } from './app.component';
import { CheckinClientComponent } from './checkin-client/checkin-client.component';
import { RoleAdminComponent } from './role-admin/role-admin.component';
import { ZoneAdminComponent } from './zone-admin/zone-admin.component';

const routes: Routes = [
   { path: '', redirectTo: '/seat', pathMatch: 'full' },
  {path: 'seatmanagement', component: SeatAdminComponent },
   {path: 'rolemanagement', component: RoleAdminComponent },
   {path: 'zonemanagement', component:  ZoneAdminComponent},
  {path: 'admin', component: AdminPageComponent},
  {path: 'employeemanagement', component: EmployeeAdminComponent},
  {path: 'seat', component: AppComponent},
  {path: 'checkin-client', component: CheckinClientComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
