using System.Threading.Tasks;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Domain.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmail(AppUser user);
    }
}