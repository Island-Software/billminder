using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var seedData = await System.IO.File.ReadAllTextAsync("Data/UsersSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(seedData);

            var passwordsData = await System.IO.File.ReadAllTextAsync("Data/UsersSeedData.json");
            var userPasswordList = JsonSerializer.Deserialize<List<UserPassword>>(passwordsData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();

                var userPassword = userPasswordList.Single(up => up.Username.ToLower() == user.UserName);

                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userPassword.Password));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}