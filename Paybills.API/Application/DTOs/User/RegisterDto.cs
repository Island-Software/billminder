using System.ComponentModel.DataAnnotations;

namespace Paybills.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(16, MinimumLength = 4)]
        public string Password { get; set; }
    }
}