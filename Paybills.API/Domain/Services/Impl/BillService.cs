using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Domain.Services.Impl
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _repository;

        public BillService(IBillRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddBillsToUser(int userId, IEnumerable<Bill> bills) => await _repository.AddBillsToUserAsync(userId, bills);

        public async Task<bool> AddBillToUser(int userId, int billId) => await _repository.AddBillToUserAsync(userId, billId);

        public async Task<bool> CopyBillsToNextMonth(int userId, int currentMonth, int currentYear) => await _repository.CopyBillsToNextMonthAsync(userId, currentMonth, currentYear);

        public async Task<bool> Create(Bill bill) => await _repository.CreateAsync(bill);

        public Task<bool> Delete(Bill bill) => _repository.DeleteAsync(bill);

        public async Task<Bill> GetBillByIdAsync(int id) => await _repository.GetBillByIdAsync(id);

        public Task<PagedList<Bill>> GetBillsAsync(string username, UserParams userParams) => _repository.GetBillsAsync(username, userParams);

        public Task<PagedList<Bill>> GetBillsByDateAsync(string username, int month, int year, UserParams userParams) => _repository.GetBillsByDateAsync(username, month, year, userParams);

        public Task<List<Bill>> GetBillsByDateAsync(string username, int month, int year)
        {
            return _repository.GetBillsByDateAsync(username, month, year);
        }

        public Task<bool> Update(Bill bill) => _repository.UpdateAsync(bill);
    }
}