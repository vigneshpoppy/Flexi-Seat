import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { Seat } from '../Models/seat';

@Component({
  selector: 'app-seat-map',
  templateUrl: './seat-map.component.html',
  styleUrls: ['./seat-map.component.css']
})
export class SeatMapComponent implements OnChanges {
  @Input() seatRows: Seat[] = []; // ⬅️ Data from parent
  @Output() seatSelected = new EventEmitter<Seat>(); // ⬅️ Send back selected seat

  chunkedSeatRows: Seat[][] = [];

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
}