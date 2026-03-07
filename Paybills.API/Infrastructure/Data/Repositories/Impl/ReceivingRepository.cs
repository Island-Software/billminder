using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paybills.API.Data;
using Paybills.API.Domain.Entities;
using Paybills.API.Helpers;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Infrastructure.Data.Repositories.Impl
{
    public class ReceivingRepository : RepositoryBase, IReceivingRepository
    {
        private IReceivingTypeRepository _receivingTypeRepository { get; }

        public ReceivingRepository(DataContext context, IReceivingTypeRepository receivingTypeRepository) : base(context)
        {
            _receivingTypeRepository = receivingTypeRepository;
        }

        public async Task<bool> AddToUserAsync(int userId, int receivingId)
        {
            var user = await _context.Users
                .Include(u => u.Receivings)
                .SingleAsync(u => u.Id == userId);

            var receiving = await _context.Receivings
                .SingleAsync(r => r.Id == receivingId);

            user.Receivings.Add(receiving);

            return await SaveAllAsync();
        }

        public async Task<bool> AddToUserAsync(int userId, IEnumerable<Receiving> receivings)
        {
            var user = await _context.Users
                .Include(u => u.Receivings)
                .SingleAsync(u => u.Id == userId);

            foreach (var receiving in receivings)
            {
                user.Receivings.Add(receiving);
            }

            return await SaveAllAsync();
        }

        private async Task<List<Receiving>> GetAsync(int userId, int currentMonth, int currentYear)
        {
            var receivings = _context.Receivings
                .Include(b => b.ReceivingType).AsNoTrackingWithIdentityResolution()
                .Where(b => b.Users.Count(u => u.Id == userId) > 0 && b.Month == currentMonth && b.Year == currentYear)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            return await receivings.ToListAsync();
        }

        public async Task<bool> CopyToNextMonthAsync(int userId, int currentMonth, int currentYear)
        {
            var receivings = await GetAsync(userId, currentMonth, currentYear);
            var newReceivings = new List<Receiving>();

            foreach (var receiving in receivings)
            {
                var newReceiving = new Receiving();
                var receivingType = await _receivingTypeRepository.GetByIdAsync(receiving.ReceivingType.Id);

                newReceiving.ReceivingType = receivingType;
                newReceiving.Month = receiving.Month == 12 ? 1 : receiving.Month + 1;
                newReceiving.Year = receiving.Month == 12 ? receiving.Year + 1 : receiving.Year;

                await CreateAsync(newReceiving);

                newReceivings.Add(newReceiving);
            }
            return await AddToUserAsync(userId, newReceivings);
        }

        public Task<bool> CreateAsync(Receiving entity)
        {
            _context.Receivings.Add(entity);
            return SaveAllAsync();
        }

        public Task<bool> DeleteAsync(Receiving entity)
        {
            _context.Receivings.Remove(entity);
            return SaveAllAsync();
        }

        public Task<PagedList<Receiving>> GetAsync(string username, UserParams userParams)
        {
            var query = _context.Receivings
                .Include(r => r.ReceivingType)
                .Include(r => r.Users)
                .Where(r => r.Users.Any(u => u.UserName == username))
                .OrderBy(r => r.Id)
                .AsNoTracking();

            return PagedList<Receiving>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<Receiving>> GetByDateAsync(string username, int month, int year, UserParams userParams)
        {
            var query = _context.Receivings
                .Include(r => r.ReceivingType)
                .Include(r => r.Users)
                .Where(r => r.Users.Any(u => u.UserName == username) && r.Month == month && r.Year == year)
                .OrderBy(r => r.Id)
                .AsNoTracking();

            return await PagedList<Receiving>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public Task<List<Receiving>> GetByDateAsync(string username, int month, int year)
        {
            return _context.Receivings
                .Include(r => r.ReceivingType)
                .Where(r => r.Users.Any(u => u.UserName == username) && r.Month == month && r.Year == year)
                .OrderBy(r => r.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Receiving> GetByIdAsync(int id) => await _context.Receivings.Include(r => r.ReceivingType).SingleAsync(r => r.Id == id);

        public Task<bool> UpdateAsync(Receiving entity)
        {
            _context.Receivings.Update(entity);
            
            return SaveAllAsync();
        }
        
    }
}