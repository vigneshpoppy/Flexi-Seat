import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SeatAdminComponent } from './seat-admin/seat-admin.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { EmployeeAdminComponent } from './employee-admin/employee-admin.component';
import { AppComponent } from './app.component';
import { CheckinClientComponent } from './checkin-client/checkin-client.component';

const routes: Routes = [
   { path: '', redirectTo: '/seat', pathMatch: 'full' },
  {path: 'seatmanagement', component: SeatAdminComponent },
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
