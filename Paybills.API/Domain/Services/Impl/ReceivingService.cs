using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Domain.Services.Impl
{
    public class ReceivingService(IReceivingRepository repository) : IReceivingService
    {
        public async Task<bool> AddToUser(int userId, int receivingId) => await repository.AddToUserAsync(userId, receivingId);

        public async Task<bool> AddToUser(int userId, IEnumerable<Receiving> receivings) => await repository.AddToUserAsync(userId, receivings);

        public async Task<bool> CopyToNextMonth(int userId, int currentMonth, int currentYear, bool copyValues) => await repository.CopyToNextMonthAsync(userId, currentMonth, currentYear, copyValues);

        public async Task<bool> Create(Receiving receiving) => await repository.CreateAsync(receiving);

        public Task<bool> Delete(Receiving receiving) => repository.DeleteAsync(receiving);

        public Task<PagedList<Receiving>> GetAsync(string username, UserParams userParams) => repository.GetAsync(username, userParams);

        public Task<PagedList<Receiving>> GetByDateAsync(string username, int month, int year, UserParams userParams) => repository.GetByDateAsync(username, month, year, userParams);

        public Task<List<Receiving>> GetByDateAsync(string username, int month, int year)
        {
            return repository.GetByDateAsync(username, month, year);
        }

        public Task<Receiving> GetByIdAsync(int id) => repository.GetByIdAsync(id);

        public Task<bool> Update(Receiving receiving) => repository.UpdateAsync(receiving);
    }
}