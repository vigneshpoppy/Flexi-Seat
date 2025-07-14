import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SeatMapComponent } from './seat-map/seat-map.component';
import { ManagerSeatFilterComponent } from './manager-seat-filter/manager-seat-filter.component';
import { FormsModule } from '@angular/forms';
import { SeatAdminComponent } from './seat-admin/seat-admin.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { EmployeeAdminComponent } from './employee-admin/employee-admin.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { HttpClientModule } from '@angular/common/http';
import { CheckinClientComponent } from './checkin-client/checkin-client.component';
import { ZXingScannerModule } from '@zxing/ngx-scanner'
import { BarcodeScannerLivestreamModule  } from 'ngx-barcode-scanner';
import { RoleAdminComponent } from './role-admin/role-admin.component';
import { ZoneAdminComponent } from './zone-admin/zone-admin.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ToastrModule } from 'ngx-toastr';
import { NotificationComponent } from './notification/notification.component';
import { LoginComponent } from './login/login.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ZoneViewerComponent } from './zone-viewer/zone-viewer.component';
@NgModule({
  declarations: [
    AppComponent,
    SeatMapComponent,
    ManagerSeatFilterComponent,
    SeatAdminComponent,
    AdminPageComponent,
    AdminDashboardComponent,
    EmployeeAdminComponent,
    LandingPageComponent,
    CheckinClientComponent,
    RoleAdminComponent,
    ZoneAdminComponent,
    NotificationComponent,
    LoginComponent,
    ChangePasswordComponent,
    ResetPasswordComponent,
    ZoneViewerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,FormsModule,HttpClientModule ,ZXingScannerModule,BarcodeScannerLivestreamModule, BrowserAnimationsModule,MatSnackBarModule
  ,
  //ToastrModule.forRoot({
    //   timeOut: 6000,
    //   positionClass: 'toast-top-right',
    //   preventDuplicates: true,
    // })
  ],
  providers: [],
  bootstrap: [LandingPageComponent]
})
export class AppModule { }
