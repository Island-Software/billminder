using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paybills.API.Domain.Entities
{
    [Table("Bills")]
    public class Bill
    {
        public int Id { get; set; }
        public BillType BillType { get; set; }
        public float Value { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Paid { get; set; }
        public ICollection<AppUser> Users { get; set; }
    }
}