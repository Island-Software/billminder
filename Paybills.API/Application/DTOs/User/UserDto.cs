using System;
using System.Collections.Generic;
using Paybills.API.Application.DTOs.Bill;

namespace Paybills.API.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<BillDto> Bills { get; set; }
        public string Email { get; set; }
    }
}