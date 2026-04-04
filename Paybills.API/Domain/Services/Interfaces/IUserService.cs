using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.Entities;

namespace Paybills.API.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateAsync(AppUser user);
        Task<bool> UserExistsAsync(string userName);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameWithDetailsAsync(string userName);
        Task<bool> UpdateAsync(AppUser user);
        Task<bool> UpdateLastActiveAsync(int userId);
    }
}