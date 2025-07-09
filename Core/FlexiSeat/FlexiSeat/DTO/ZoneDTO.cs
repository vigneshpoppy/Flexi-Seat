namespace FlexiSeat.DTO
{
    public class ZoneDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LocationName { get; set; }
        public bool IsActive { get; set; }

        public string? ManagerADID { get; set; }
        public string? ManagerName { get; set; }
    }
}
