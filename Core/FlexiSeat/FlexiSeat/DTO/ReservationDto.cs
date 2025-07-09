namespace FlexiSeat.DTO
{
    public class ReservationDto
    {
        public string UserADID { get; set; }
        public string SeatNo { get; set; }
        public DateTime ReserveDate { get; set; }
        public int Status { get; set; }
    }
}
