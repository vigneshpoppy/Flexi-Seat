import { Component, OnInit } from '@angular/core';
import { Seat } from './Models/seat';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent  implements OnInit{


  title = 'FlexiSeatBooking';

  // Final seat data to use in seat map UI
  seatData: Seat[] = [];
  filteredSeatData: Seat[]=[]
  seatRows:Seat[]=[];
  isManagerSelected=false;
  ngOnInit() {
    this.isManagerSelected=false;
    this.mockData();
    
  }

  applyRegionFilter(selectedRegions: string[]) {
  this.filteredSeatData = this.seatData.filter(seat => selectedRegions.includes(seat.row));
   }

   CheckManager(manager: string) {
    console.log('Parent component:'+manager);
    
    if(manager!=undefined){
     this.isManagerSelected=true
    };
    };
  onSeatSelected(seat: Seat) {
    console.log('Seat selected:', seat);
  }
  
  mockData(){
this.seatData=[
  { id: 'A20C1', row: 'A', number: 1, status: 'available' },
  { id: 'A20C2', row: 'A', number: 2, status: 'booked' },
  { id: 'A20C3', row: 'A', number: 3, status: 'confirmed' },
  { id: 'A20C4', row: 'A', number: 4, status: 'checked-in' },
  { id: 'A20C5', row: 'A', number: 5, status: 'booked' },
  { id: 'A20C6', row: 'A', number: 6, status: 'checked-in' },
  { id: 'A20C7', row: 'A', number: 7, status: 'confirmed' },
  { id: 'A20C8', row: 'A', number: 8, status: 'available' },
  { id: 'A20C9', row: 'A', number: 9, status: 'booked' },
  { id: 'A20C10', row: 'A', number: 10, status: 'available' },

  { id: 'A20C11', row: 'A', number: 11, status: 'checked-in' },
  { id: 'A20C12', row: 'A', number: 12, status: 'available' },
  { id: 'A20C13', row: 'A', number: 13, status: 'booked' },
  { id: 'A20C14', row: 'A', number: 14, status: 'confirmed' },
  { id: 'A20C15', row: 'A', number: 15, status: 'available' },
  { id: 'A20C16', row: 'A', number: 16, status: 'booked' },
  { id: 'A20C17', row: 'A', number: 17, status: 'available' },
  { id: 'A20C18', row: 'A', number: 18, status: 'confirmed' },
  { id: 'A20C19', row: 'A', number: 19, status: 'available' },
  { id: 'A20C20', row: 'A', number: 20, status: 'booked' },

  { id: 'A20C21', row: 'A', number: 21, status: 'confirmed' },
  { id: 'A20C22', row: 'A', number: 22, status: 'checked-in' },
  { id: 'A20C23', row: 'A', number: 23, status: 'booked' },
  { id: 'A20C24', row: 'A', number: 24, status: 'confirmed' },
  { id: 'A20C25', row: 'A', number: 25, status: 'available' },
  { id: 'A20C26', row: 'A', number: 26, status: 'available' },
  { id: 'A20C27', row: 'A', number: 27, status: 'booked' },
  { id: 'A20C28', row: 'A', number: 28, status: 'confirmed' },
  { id: 'A20C29', row: 'A', number: 29, status: 'booked' },
  { id: 'A20C30', row: 'A', number: 30, status: 'checked-in' },

  { id: 'B20C1', row: 'B', number: 1, status: 'available' },
  { id: 'B20C2', row: 'B', number: 2, status: 'booked' },
  { id: 'B20C3', row: 'B', number: 3, status: 'confirmed' },
  { id: 'B20C4', row: 'B', number: 4, status: 'checked-in' },
  { id: 'C20C1', row: 'C', number: 1, status: 'confirmed' },

  { id: 'C20C2', row: 'C', number: 2, status: 'checked-in' },
  { id: 'C20C3', row: 'C', number: 3, status: 'booked' },
  { id: 'C20C4', row: 'C', number: 4, status: 'available' },
   
  { id: 'D20C1', row: 'D', number: 1, status: 'available' },
  { id: 'D20C2', row: 'D', number: 2, status: 'confirmed' },
  { id: 'D20C3', row: 'D', number: 3, status: 'available' },
  { id: 'D20C4', row: 'D', number: 4, status: 'booked' },

  { id: 'E20C1', row: 'E', number: 1, status: 'booked' },
  { id: 'E20C2', row: 'E', number: 2, status: 'checked-in' },
  { id: 'E20C3', row: 'E', number: 3, status: 'available' },
  { id: 'E20C4', row: 'E', number: 4, status: 'confirmed' },
  
  { id: 'F20C1', row: 'F', number: 1, status: 'confirmed' },
  { id: 'F20C2', row: 'F', number: 2, status: 'booked' },
  { id: 'F20C3', row: 'F', number: 3, status: 'available' },
  { id: 'F20C4', row: 'F', number: 4, status: 'available' },



]
  }
  

  // generateMockSeats() {
  //   const regions = ['A', 'B', 'C', 'D', 'E', 'F'];
  //   const campuses = [20, 30];
  //   const seatsPerRegion = 5;

  //   const mockSeatMap: Seat[][] = [];

  //   for (const region of regions) {
  //     const row: Seat[] = [];

  //     for (let i = 1; i <= seatsPerRegion; i++) {
  //       const campus = campuses[i % 2]; // Alternate between 20 and 30
  //       const cabin = 'C';
  //       const seatNumber = i;

  //       const id = `${region}${campus}${cabin}${seatNumber}`;
  //       const statusIndex = i % 4;
  //       const status: SeatStatus = ['available', 'booked', 'confirmed', 'checked-in'][statusIndex] as SeatStatus;

  //       row.push({
  //         id,
  //         row: region,
  //         number: seatNumber,
  //         status
  //       });
  //     }

  //     mockSeatMap.push(row);
  //   }

  //   this.seatData = mockSeatMap;
  // }
}

