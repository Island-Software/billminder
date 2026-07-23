using System;
using Paybills.API.Application.DTOs.BillType;

namespace Paybills.API.Application.DTOs.Bill
{
    public class BillDto
    {
        public int Id { get; set; }
        public BillTypeDto BillType { get; set; }
        public float Value { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Paid { get; set; }
        public string[] UserName { get; set; }
    }
}