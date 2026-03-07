using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Helpers;

namespace Paybills.API.Domain.Services.Interfaces
{
    public interface IReceivingService
    {
        Task<bool> Create(Receiving receiving);
        Task<bool> Delete(Receiving receiving);
        Task<bool> Update(Receiving receiving);
        Task<bool> CopyToNextMonth(int userId, int currentMonth, int currentYear);
        Task<PagedList<Receiving>> GetAsync(string username, UserParams userParams);
        Task<PagedList<Receiving>> GetByDateAsync(string username, int month, int year, UserParams userParams);
        Task<List<Receiving>> GetByDateAsync(string username, int month, int year);
        Task<Receiving> GetByIdAsync(int id);
        Task<bool> AddToUser(int userId, int receivingId);
        Task<bool> AddToUser(int userId, IEnumerable<Receiving> receivings);
    }
}