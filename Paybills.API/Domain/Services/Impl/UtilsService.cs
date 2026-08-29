using System.Threading.Tasks;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Domain.Services.Impl;

public class UtilsService(IReceivingRepository receivingRepository, IBillRepository billRepository)
    : IUtilsService
{
    public async Task<bool> CopyBillsAndReceivingsToNextMonth(int userId, int currentMonth, int currentYear, bool copyValues)
    {
        if (await billRepository.CopyToNextMonthAsync(userId, currentMonth, currentYear, copyValues))
        {
            return await receivingRepository.CopyToNextMonthAsync(userId, currentMonth, currentYear, copyValues);
        }
        return false;
    }
}