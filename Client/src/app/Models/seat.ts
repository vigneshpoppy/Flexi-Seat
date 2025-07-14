export interface Seat {
  id: string;            // e.g., A20C1
  row: string;           // Region like 'A'
  number: number;        // Seat number
  status: SeatStatus;
  // 'available' | 'booked' | 'confirmed' | 'checked-in'
}
export type SeatStatus = 'available' | 'booked' | 'confirmed' | 'checked-in';

export interface NewSeat {
  number: string;            // e.g., A20C1
  zoneId: string;           // Region like 'A'
      
  isActive: boolean;    // 'available' | 'booked' | 'confirmed' | 'checked-in'
}


export interface Seatpostpayload{

  number: string;
  zoneId: number;

}


export interface SeatPatchpayload {
  number: string;
  zoneId: number;
  isActive :boolean
}