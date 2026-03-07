using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Helpers;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Domain.Services.Impl
{
    public class ReceivingService : IReceivingService
    {
        private readonly IReceivingRepository _repository;
        
        public ReceivingService(IReceivingRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddToUser(int userId, int receivingId) => await _repository.AddToUserAsync(userId, receivingId);

        public async Task<bool> AddToUser(int userId, IEnumerable<Receiving> receivings) => await _repository.AddToUserAsync(userId, receivings);

        public async Task<bool> CopyToNextMonth(int userId, int currentMonth, int currentYear) => await _repository.CopyToNextMonthAsync(userId, currentMonth, currentYear);

        public async Task<bool> Create(Receiving receiving) => await _repository.CreateAsync(receiving);

        public Task<bool> Delete(Receiving receiving) => _repository.DeleteAsync(receiving);

        public Task<PagedList<Receiving>> GetAsync(string username, UserParams userParams) => _repository.GetAsync(username, userParams);

        public Task<PagedList<Receiving>> GetByDateAsync(string username, int month, int year, UserParams userParams) => _repository.GetByDateAsync(username, month, year, userParams);

        public Task<List<Receiving>> GetByDateAsync(string username, int month, int year)
        {
            return _repository.GetByDateAsync(username, month, year);
        }

        public Task<Receiving> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task<bool> Update(Receiving receiving) => _repository.UpdateAsync(receiving);
    }
}