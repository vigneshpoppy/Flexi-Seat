namespace FlexiSeat.DTO
{
    public class RegisterDto
    {
        public string ADID { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "employee"; // Optional
        public string OTP { get; set; } = "000000";
    }
}
