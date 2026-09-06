namespace Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
