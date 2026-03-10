namespace CRM.Authentication.Application.Users.DTOs
{
    public class ForgotPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }

    public class VerifyOtpResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ConfirmToken { get; set; } = string.Empty;
    }

    public class ForgotPasswordPhoneRequestDto
    {
        public string Phone { get; set; } = string.Empty;
        public string Channel { get; set; } = "sms"; // sms | zalo
    }
}
