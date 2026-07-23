using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Domain.Services.Impl
{
    public class BillTypeService : IBillTypeService
    {
        private readonly IBillTypeRepository _repository;

        public BillTypeService(IBillTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Exists(string description) => await _repository.ExistsAsync(description);

        public async Task<bool> Create(BillType billType)
        {
            await _repository.CreateAsync(billType);

            return await _repository.SaveAllAsync();
        }

        public async Task<bool> Delete(BillType bill)
        {
            _repository.Delete(bill);

            return await _repository.SaveAllAsync();
        }

        public Task<IEnumerable<BillType>> GetByDescription(string description) => _repository.GetByDescriptionAsync(description);

        public Task<BillType> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task<IEnumerable<BillType>> GetAsync() => _repository.GetAsync();

        public Task<bool> Update(BillType bill)
        {
            _repository.Update(bill);

            return _repository.SaveAllAsync();
        }
    }
}