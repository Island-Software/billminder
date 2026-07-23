using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paybills.API.Domain.Entities;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Infrastructure.Data.Repositories.Impl
{
    public class BillRepository : RepositoryBase, IBillRepository
    {
        private IBillTypeRepository _billTypeRepository { get; }

        public BillRepository(DataContext context, IBillTypeRepository billTypeRepository) : base(context)
        {
            _billTypeRepository = billTypeRepository;
        }

        public async Task<bool> CreateAsync(Bill bill)
        {
            _context.Bills.Add(bill);

            return await SaveAllAsync();
        }

        public async Task<bool> DeleteAsync(Bill bill)
        {
            _context.Bills.Remove(bill);

            return await SaveAllAsync();
        }

        public async Task<PagedList<Bill>> GetBillsAsync(string username, UserParams userParams)
        {
            var query = _context.Bills
                .Include(b => b.BillType)
                .Include(b => b.Users)
                .Where(b => b.Users.Count(u => u.UserName == username) > 0)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            return await PagedList<Bill>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<List<Bill>> GetBillsAsync(int userId)
        {
            var bills = _context.Bills
                .Include(b => b.BillType)
                .Where(b => b.Users.Count(u => u.Id == userId) > 0)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            return await bills.ToListAsync();
        }

        private async Task<List<Bill>> GetBillsAsync(int userId, int currentMonth, int currentYear)
        {
            var bills = _context.Bills
                .Include(b => b.BillType).AsNoTrackingWithIdentityResolution()
                .Where(b => b.Users.Count(u => u.Id == userId) > 0 && b.Month == currentMonth && b.Year == currentYear)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            return await bills.ToListAsync();
        }

        public async Task<PagedList<Bill>> GetBillsByDateAsync(string username, int month, int year, UserParams userParams)
        {
            var query = _context.Bills
                .Include(b => b.BillType)
                .Include(b => b.Users)
                .Where(b => b.Users.Count(u => u.UserName == username) > 0 && b.Month == month && b.Year == year)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            return await PagedList<Bill>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public Task<List<Bill>> GetBillsByDateAsync(string username, int month, int year)
        {
            return _context.Bills
                .Include(b => b.BillType)
                .Where(b => b.Users.Count(u => u.UserName == username) > 0 && b.Month == month && b.Year == year)
                .OrderBy(b => b.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bill> GetBillByIdAsync(int id) => await _context.Bills.Include(b => b.BillType).SingleAsync(b => b.Id == id);

        public async Task<bool> UpdateAsync(Bill bill)
        {
            _context.Entry(bill).State = EntityState.Modified;

            return await SaveAllAsync();
        }

        public async Task<bool> AddBillToUserAsync(int userId, int billId)
        {
            var user = await _context.Users
                .Include(u => u.Bills)
                .SingleAsync(u => u.Id == userId);

            var bill = await _context.Bills
                .SingleAsync(b => b.Id == billId);

            user.Bills.Add(bill);

            return await SaveAllAsync();
        }

        public async Task<bool> AddBillsToUserAsync(int userId, IEnumerable<Bill> bills)
        {
            var user = await _context.Users
                .Include(u => u.Bills)
                .SingleAsync(u => u.Id == userId);

            foreach (var bill in bills)
            {
                user.Bills.Add(bill);
            }

            return await SaveAllAsync();
        }

        public async Task<bool> CopyBillsToNextMonthAsync(int userId, int currentMonth, int currentYear)
        {
            var bills = await GetBillsAsync(userId, currentMonth, currentYear);
            var newBills = new List<Bill>();

            foreach (var bill in bills)
            {
                // _context.Entry(bill.BillType).State = EntityState.Unchanged;

                var newBill = new Bill();
                var billType = await _billTypeRepository.GetByIdAsync(bill.BillType.Id);

                newBill.BillType = billType;
                newBill.Month = bill.Month;
                newBill.Year = bill.Year;
                newBill.Month = bill.Month == 12 ? 1 : bill.Month + 1;
                newBill.Year = bill.Month == 12 ? bill.Year + 1 : bill.Year;

                await CreateAsync(newBill);

                newBills.Add(newBill);
            }
            return await AddBillsToUserAsync(userId, newBills);
        }        
    }
}