using System;
using System.Collections.Generic;

namespace Paybills.API.Domain.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public ICollection<Bill> Bills { get; set; }
        public ICollection<Receiving> Receivings { get; set; }
        public string Email { get; set; }
        public string EmailToken { get; set; }
        public bool EmailValidated { get; set; }
    }
}