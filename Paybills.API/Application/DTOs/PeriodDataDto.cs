namespace Paybills.API.Application.DTOs
{
    public class PeriodDataDto
    {
        public int UserId { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
    }
}