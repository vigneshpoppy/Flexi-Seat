namespace FlexiSeat.DTO
{
    public class ReservationDto
    {
        public string UserADID { get; set; }
        public string? ReservedByADID { get; set; }
        public int SeatID { get; set; }
        public DateTime ReservedDate { get; set; }
    }
}
