using System;
using System.ComponentModel.DataAnnotations;

namespace Paybills.API.Application.DTOs.Bill
{
    public class BillRegisterDto
    {
        [Required]
        public int TypeId { get; set; }
        public float Value { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Paid { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}