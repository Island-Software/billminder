using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paybills.API.Domain.Entities
{
    [Table("Receivings")]
    public class Receiving
    {
        public int Id { get; set; }
        public ReceivingType ReceivingType { get; set; }
        public float Value { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public bool Received { get; set; } = false;
        public ICollection<AppUser> Users { get; set; }        
    }
}