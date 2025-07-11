namespace FlexiSeat.DTO.ZoneDTOs
{
    public class GetZoneDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ManagerADID { get; set; }
        public string? ManagerName { get; set; }
    }
}
