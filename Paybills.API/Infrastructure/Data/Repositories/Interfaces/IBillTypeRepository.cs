using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Data.Repositories.Interfaces
{
    public interface IBillTypeRepository
    {
        Task<bool> CreateAsync(BillType bill);
        void Delete(BillType bill);
        void Update(BillType bill);
        // Change it to a base repository class
        Task<bool> SaveAllAsync();
        Task<IEnumerable<BillType>> GetAsync();
        Task<BillType> GetByIdAsync(int id);
        Task<IEnumerable<BillType>> GetByDescriptionAsync(string description);
        Task<bool> ExistsAsync(string description);
    }
}