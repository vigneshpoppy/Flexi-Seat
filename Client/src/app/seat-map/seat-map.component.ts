import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { Seat } from '../Models/seat';
import { NotificationService } from '../Service/notification.service';

@Component({
  selector: 'app-seat-map',
  templateUrl: './seat-map.component.html',
  styleUrls: ['./seat-map.component.css']
})
export class SeatMapComponent implements OnChanges {

  constructor(private notify:NotificationService){

  }
  @Input() seatRows: Seat[] = []; // ⬅️ Data from parent
  @Output() seatSelected = new EventEmitter<Seat>(); // ⬅️ Send back selected seat

// New Inputs for bulk booking
  @Input() isSupervisor: boolean = false;
  @Input() teamMembers: { id: string; name: string; selected?: boolean }[] = [];
  @Output() bulkBooked = new EventEmitter<{ employeeId: string; seatId: string }[]>();

  chunkedSeatRows: Seat[][] = [];

 showPopup = false;
  

  ngOnChanges(changes: SimpleChanges) {
    if (changes['seatRows']) {
      this.chunkedSeatRows=[];
      this.createChunks()
    }
  }


  createChunks(){
    
    const perRow = 8;
  for (let i = 0; i < this.seatRows.length; i += perRow) {
    this.chunkedSeatRows.push(this.seatRows.slice(i, i + perRow));
  }
  console.log(this.chunkedSeatRows);
  
  }
  onSeatClick(seat: Seat) {
    if (seat.status === 'booked') return;
    this.seatSelected.emit(seat);
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
    const availableSeats = this.seatRows.filter(seat => seat.status === 'available');

    if (selectedMembers.length > availableSeats.length) {
      this.notify.showWarning(' Not enough seats available!')
     // alert(' Not enough seats available!');
      return;
    }

    const bookings = selectedMembers.map((member, index) => ({
      employeeId: member.id,
      seatId: availableSeats[index].id,
    }));

    this.bulkBooked.emit(bookings);
    this.closePopup();

}
}