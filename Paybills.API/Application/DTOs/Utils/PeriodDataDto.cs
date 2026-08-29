namespace Paybills.API.Application.DTOs.Utils
{
    public class PeriodDataDto
    {
        public int UserId { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        public bool CopyValues { get; set; }
    }
}