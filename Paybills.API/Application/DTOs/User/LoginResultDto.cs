namespace Paybills.API.Application.DTOs.User
{
    public class LoginResultDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}