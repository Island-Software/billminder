using System;

namespace Paybills.API.Application.DTOs.User
{
    public class UserEditDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime LastActive { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}