import { Component } from '@angular/core';
import { NewSeat, Seat, SeatStatus } from '../Models/seat';

@Component({
  selector: 'app-seat-admin',
  templateUrl: './seat-admin.component.html',
  styleUrls: ['./seat-admin.component.css']
})
export class SeatAdminComponent {


    seatData: NewSeat[] = [
    { id: 'A20C1', zone: 'A',  status: 'available' },
    { id: 'B20C2', zone: 'B',  status: 'booked' },
    { id: 'C20C3', zone: 'C',  status: 'confirmed' },
    { id: 'D20C4', zone: 'D',  status: 'available' }
  ];

  searchId: string = '';
  filteredSeats: NewSeat[] = [];

  newSeat: NewSeat = { id: '', zone: '',status: 'available' };

  onSearch() {
    const query = this.searchId.trim().toLowerCase();
    this.filteredSeats = this.seatData.filter(seat =>
      seat.id.toLowerCase() === query
    );
  }

  saveSeat(seat: NewSeat) {
    const index = this.seatData.findIndex(s => s.id === seat.id);
    if (index !== -1) {
      this.seatData[index] = { ...seat };
      alert('Seat updated!');
    }
  }

  deleteSeat(seat: NewSeat) {
    this.seatData = this.seatData.filter(s => s.id !== seat.id);
    this.filteredSeats = this.filteredSeats.filter(s => s.id !== seat.id);
  }

  addSeat() {
    if (!this.newSeat.id || !this.newSeat.zone) {
      alert('All fields are required.');
      return;
    }

    const exists = this.seatData.some(s => s.id === this.newSeat.id);
    if (exists) {
      alert('Seat ID already exists.');
      return;
    }

    this.seatData.push({ ...this.newSeat });
    this.newSeat = { id: '', zone: '', status: 'available' };
    alert('Seat added!');
  }

}
