using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Domain.Services.Interfaces
{
    public interface IBillService
    {
        Task<bool> Create(Bill bill);
        Task<bool> Delete(Bill bill);
        Task<bool> Update(Bill bill);
        Task<bool> CopyBillsToNextMonth(int userId, int currentMonth, int currentYear);
        Task<PagedList<Bill>> GetBillsAsync(string username, UserParams userParams);
        Task<PagedList<Bill>> GetBillsByDateAsync(string username, int month, int year, UserParams userParams);
        Task<List<Bill>> GetBillsByDateAsync(string username, int month, int year);
        Task<Bill> GetBillByIdAsync(int id);
        Task<bool> AddBillToUser(int userId, int billId);
        Task<bool> AddBillsToUser(int userId, IEnumerable<Bill> bills);
    }
}