using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Helpers;

namespace Paybills.API.Infrastructure.Data.Repositories.Interfaces
{
    public interface IReceivingRepository
    {
        Task<bool> CreateAsync(Receiving entity);
        Task<bool> DeleteAsync(Receiving entity);
        Task<bool> UpdateAsync(Receiving entity);
        Task<bool> CopyToNextMonthAsync(int userId, int currentMonth, int currentYear);
        Task<bool> SaveAllAsync();
        Task<PagedList<Receiving>> GetAsync(string username, UserParams userParams);
        Task<PagedList<Receiving>> GetByDateAsync(string username, int month, int year, UserParams userParams);
        Task<List<Receiving>> GetByDateAsync(string username, int month, int year);
        Task<Receiving> GetByIdAsync(int id);
        Task<bool> AddToUserAsync(int userId, int receivingId);
        Task<bool> AddToUserAsync(int userId, IEnumerable<Receiving> receivings);
    }
}