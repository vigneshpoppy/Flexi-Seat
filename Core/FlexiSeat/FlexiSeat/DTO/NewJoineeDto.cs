namespace FlexiSeat.DTO
{
    public class NewJoineeDto
    {
        public string ADID { get; set; }
        public string ManagerADID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BadgeID { get; set; }  // ✅ Included
    }
}
