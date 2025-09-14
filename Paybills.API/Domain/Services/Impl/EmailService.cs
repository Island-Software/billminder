using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Paybills.API.Application.Controllers;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Entities;
using Paybills.API.Infrastructure.Services;

namespace Paybills.API.Domain.Services.Impl
{
    public class EmailService : IEmailService
    {
        private SESService _simpleEmailService;

        public EmailService(SESService simpleEmailService)
        {
            _simpleEmailService = simpleEmailService;
        }

        public async Task<bool> SendVerificationEmail(AppUser user)
        {
            if (user.Email.IsNullOrEmpty()) return false;

            var result = await _simpleEmailService.SendEmailAsync(
                new List<string>() { user.Email },
                null,
                null,
                GenerateVerificationEmail(user.UserName, user.Email, user.EmailToken),
                "",
                "Required step - Email verification",
                "admin@billminder.com.br");

            return result != string.Empty;
        }

        private string GenerateVerificationEmail(string userName, string email, string emailToken)
        {
            var verificationEmail = Consts.VerificationEmail;

            verificationEmail = verificationEmail.Replace("{username}", userName);
            verificationEmail = verificationEmail.Replace("<email>", email);
            verificationEmail = verificationEmail.Replace("<email-token>", emailToken);

            return verificationEmail;
        }
    }
}