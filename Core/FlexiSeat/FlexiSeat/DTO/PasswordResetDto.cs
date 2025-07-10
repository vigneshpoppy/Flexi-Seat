namespace FlexiSeat.DTO
{
    public class PasswordResetDto
    {
        public string ADID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string OTP { get; set; }
    }
    
}

