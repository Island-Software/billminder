using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Create(AppUser user);
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> GetUserByUsernameWithDetailsAsync(string username);
        Task<AppUser> GetUserByEmailAsync(string email);

        Task<bool> ExistsAsync(string userName);

        Task<bool> UpdateLastActiveAsync(int userId);
    }
}