using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user, int expirationTimeInDays);
    }
}