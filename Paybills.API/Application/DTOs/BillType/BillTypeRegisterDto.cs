using System.ComponentModel.DataAnnotations;

namespace Paybills.API.Application.DTOs.BillType
{
    public class BillTypeRegisterDto
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}