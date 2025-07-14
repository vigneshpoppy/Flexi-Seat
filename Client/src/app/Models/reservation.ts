export interface individualSeatReservation {
  userADID: string;
  seatID: string;
  reservedDate: string; // or Date if you parse it
  reservedByADID: string;
}

export interface UserSeatReservation {
  userADID: string;
  seatID: number;

}