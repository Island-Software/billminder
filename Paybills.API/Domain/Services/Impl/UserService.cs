using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Data.Repositories.Interfaces;

namespace Paybills.API.Domain.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CreateAsync(AppUser user)
        {
            _userRepository.Create(user);

            return await _userRepository.SaveAllAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {            
            return await _userRepository.GetUserByUsernameAsync(userName);
        }

        public async Task<AppUser> GetUserByUserNameWithDetailsAsync(string userName)
        {
            return await _userRepository.GetUserByUsernameWithDetailsAsync(userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<bool> UpdateAsync(AppUser user)
        {
            _userRepository.Update(user);

            return await _userRepository.SaveAllAsync();
        }

        public async Task<bool> UserExistsAsync(string userName)
        {
            return await _userRepository.ExistsAsync(userName);
        }

        public async Task<bool> UpdateLastActiveAsync(int userId)
        {
            return await _userRepository.UpdateLastActiveAsync(userId);
        }
    }
}