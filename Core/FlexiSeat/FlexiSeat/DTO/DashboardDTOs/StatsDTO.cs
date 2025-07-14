namespace FlexiSeat.DTO.DashboardDTOs
{
    public class StatsDTO
    {
        public DashboardSeatsDTO DashboardSeatsDTO { get; set; }
        public List<Dictionary<string, string>> DashboardWeeklyCheckinCount { get; set; }
        public List<Dictionary<string, string>> ZoneDetails { get; set; }
        public List<Dictionary<string, string>> RoleDetails { get; set; }
    }
}
