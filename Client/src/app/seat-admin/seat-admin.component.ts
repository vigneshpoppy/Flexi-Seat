import { Component, OnInit } from '@angular/core';
import { NewSeat, Seat, SeatPatchpayload, Seatpostpayload, SeatStatus } from '../Models/seat';
import { ZoneServiceService } from '../Service/zone-service.service';
import { NotificationService } from '../Service/notification.service';
import { SeatServiceService } from '../Service/seat-service.service';

@Component({
  selector: 'app-seat-admin',
  templateUrl: './seat-admin.component.html',
  styleUrls: ['./seat-admin.component.css']
})
export class SeatAdminComponent  implements OnInit{

  constructor(private zoneservice:ZoneServiceService,private notify:NotificationService
    ,private seatService:SeatServiceService
  ){

  }

  postpayload:Seatpostpayload={number:"",zoneId:0}
  zones:any=[];
  ngOnInit(): void {
    this.getAllZone();
  }


    getAllZone(){
      this.zoneservice.getZoneAllData().subscribe({
        next: result=>{
          console.log(result)
          this.zones=result;
        },
        error: err=>{
        console.log(err);
        }
      })
    }

  //   seatData: NewSeat[] = [
  //   { number: 'A20C1', zone: 'A',  status: 'available' },
  //   { id: 'B20C2', zone: 'B',  status: 'booked' },
  //   { id: 'C20C3', zone: 'C',  status: 'confirmed' },
  //   { id: 'D20C4', zone: 'D',  status: 'available' }
  // ];

  searchId: string = '';
  filteredSeats: SeatPatchpayload[] = [];

  newSeat: NewSeat = { number: '', zoneId: '',isActive: true };

  onSearch() {
    const term = this.searchId.trim();
     this.seatService.getseatByID(term).subscribe({
    next: result => {
      this.filteredSeats = result ? [result] : [];
    },
    error: err => {
      console.error('Error fetching seats ID:', err);
      this.filteredSeats = [];
    }
  });
  }

  saveSeat(seat: SeatPatchpayload) {
   
    this.seatService.updateSeat(seat.number,seat).subscribe({
       next: result => {
     this.notify.showSuccess("Seat added")
    },
    error: err => {
      console.error('Error:', err);
      
    }
    })
  }

  deleteSeat(seat: SeatPatchpayload) {

  const term =this.searchId.trim();
    this.seatService.deleteSeat(term).subscribe({
        next: result => {
     this.notify.showSuccess("Delete seats")
    },
    error: err => {
      console.error('Error:', err);
    }
    })
  }

  addSeat() {
    if (!this.newSeat.number || !this.newSeat.isActive) {

      this.notify.showInfo('All fields are required.')
     
      return;
    }else{
      this.postpayload.number=this.newSeat.number;
      this.postpayload.number=this.newSeat.zoneId;
      this.seatService.postseatData(this.postpayload).subscribe({
       next: result => {
     this.notify.showSuccess("Seat added")
    },
    error: err => {
      console.error('Error:', err);
      
    }
    })
    }

  
    this.newSeat = { number: '', zoneId: '', isActive: true };
    this.postpayload={number:"",zoneId:0};
    
  }

}
