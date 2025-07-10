namespace FlexiSeat.DTO
{
    public class SeatDTO
    {
        public int ID { get; set; }
        public string Number { get; set; }    
        public int ZoneId { get; set; }
        public bool IsActive { get; set; }
        public string? ZoneName { get; set; }
        public bool IsBooked { get; set; }
    }
}
