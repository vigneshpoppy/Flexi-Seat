import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Seat } from '../Models/seat';
import { NotificationService } from '../Service/notification.service';
import { individualSeatReservation, UserSeatReservation } from '../Models/reservation';
import { UserService } from '../Service/user.service';

@Component({
  selector: 'app-seat-map',
  templateUrl: './seat-map.component.html',
  styleUrls: ['./seat-map.component.css']
})


export class SeatMapComponent implements OnChanges,OnInit {

  constructor(private notify:NotificationService,private userService:UserService){

  }
  userAdID:any;
  ngOnInit(): void {
   this.userAdID=localStorage.getItem('userid')?.toUpperCase()
   this.fetchTeamMembers();
  }
  @Input() seatRows: Seat[] = []; // ⬅️ Data from parent
  @Output() seatSelected = new EventEmitter<Seat>(); // ⬅️ Send back selected seat

   @Output() individualSelected = new EventEmitter<UserSeatReservation>(); 

// New Inputs for bulk booking
  @Input() isSupervisor: boolean = false;
  @Input() teamMembers: { adid: string; name: string; selected?: boolean }[] = [];
  @Output() bulkBooked = new EventEmitter<{ employeeId: string; seatId: string }[]>();

  chunkedSeatRows: Seat[][] = [];

 showPopup = false;
selectedSeat: Seat | null = null;
  
myTeam:Employee1[]=[];
  ngOnChanges(changes: SimpleChanges) {
    if (changes['seatRows']) {
      this.chunkedSeatRows=[];
      this.createChunks()
    }
  }

fetchTeamMembers(){
this.userService.getTeamMembersByManagerID(this.userAdID).subscribe({
    next:result=>{
     // this.allSeatData=result;
     this.myTeam=result;
console.log(this.myTeam);
    },
    error:err=>{
      console.log(err);
    }
    
  })
}

  createChunks(){
    console.log("Data here "+this.seatRows);
    
    const perRow = 8;
  for (let i = 0; i < this.seatRows.length; i += perRow) {
    this.chunkedSeatRows.push(this.seatRows.slice(i, i + perRow));
  }
  console.log(this.chunkedSeatRows);
  
  }
  onSeatClick(seat: Seat) {
    console.log(seat);
    
    if (seat.status != 'Available') return;
      // Set the selected seat to open confirmation modal
  this.selectedSeat = seat;
   // this.seatSelected.emit(seat);
  }

bookSelectedSeat() {
  console.log("Booking selection"+this.selectedSeat);
  
  
  if (this.selectedSeat) {
    this.selectedSeat.status = 'Booked'; // or 'booked' based on your flow
    this.seatSelected.emit(this.selectedSeat);
    this.selectedSeat = null;
  }
}


cancelSeat() {
  if (this.selectedSeat) {
      console.log("Cancel selection"+this.selectedSeat);
    this.selectedSeat.status = 'Available';
    this.seatSelected.emit(this.selectedSeat);
    this.selectedSeat = null;
  }
}

closeConfirmation() {
  this.selectedSeat = null;
}
//  Bulk Booking: Show team popup
  openBulkBookingPopup() {
    this.showPopup = true;
  }

  closePopup() {
    this.showPopup = false;
  }

  // Bulk Booking logic
  confirmBulkBooking() {
    const selectedMembers = this.teamMembers.filter(emp => emp.selected);
    const availableSeats = this.seatRows.filter(seat => seat.status === 'Available');

    if (selectedMembers.length > availableSeats.length) {
      this.notify.showWarning(' Not enough seats available!')
     // alert(' Not enough seats available!');
      return;
    }

    const bookings = selectedMembers.map((member, index) => ({
      employeeId: member.adid,
      seatId: availableSeats[index].id,
    }));

    this.bulkBooked.emit(bookings);
    this.closePopup();

}

  refresh(): void {
    
    
  }



}

export interface Employee1 {
  adid: string;
  name: string;
  selected?: boolean;
}