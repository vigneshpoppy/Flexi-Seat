namespace FlexiSeat.DTO.DashboardDTOs
{
    public class DashboardSeatsDTO
    {
        public string TotalCount { get; set; }
        public string AvailableCountAndPercentage { get; set; }
        public string ConfirmedCountAndPercentage { get; set; }
        public string CheckedInCountAndPercentage { get; set; }
        public DateTime Date { get; set; }
    }
}
