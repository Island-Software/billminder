using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paybills.API.Domain.Entities;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Infrastructure.Data.Repositories.Impl
{
    public class BillTypeRepository : RepositoryBase, IBillTypeRepository
    {
        public BillTypeRepository(DataContext context) : base(context) { }

        public async Task<bool> ExistsAsync(string description) => await _context.BillTypes.CountAsync(bt => bt.Description == description) > 0;

        public async Task<bool> CreateAsync(BillType billType) => await _context.BillTypes.AddAsync(billType) != null;

        public void Delete(BillType billType) => _context.BillTypes.Remove(billType);

        public async Task<IEnumerable<BillType>> GetByDescriptionAsync(string description) => await _context.BillTypes.Where(bt => bt.Description == description).ToListAsync();

        public async Task<BillType> GetByIdAsync(int id) => await _context.BillTypes.SingleOrDefaultAsync(bt => bt.Id == id);

        public async Task<IEnumerable<BillType>> GetAsync() => await _context.BillTypes.ToListAsync();

        public void Update(BillType billType) => _context.Entry(billType).State = EntityState.Modified;
    }
}