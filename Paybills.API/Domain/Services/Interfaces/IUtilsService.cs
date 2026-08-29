using System.Threading.Tasks;

namespace Paybills.API.Domain.Services.Interfaces;

public interface IUtilsService
{
    Task<bool> CopyBillsAndReceivingsToNextMonth(int userId, int currentMonth, int currentYear, bool copyValues);
}