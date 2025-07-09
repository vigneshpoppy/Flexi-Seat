namespace FlexiSeat.DTO
{
    public class BulkReservationDto
    {
        public string RequestedByADID { get; set; }
        public List<ReservationDto> Reservations { get; set; }
    }
}
