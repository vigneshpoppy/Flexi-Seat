namespace FlexiSeat.DTO
{
    public class UserDTO
    {
        public string ADID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string BadgeID { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public string? LeadADID { get; set; }
        public string? LeadName { get; set; }

        public string? ManagerADID { get; set; }
        public string? ManagerName { get; set; }
    }
}
