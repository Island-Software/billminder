using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Controllers;
using Paybills.API.Infrastructure.Services;
using Paybills.API.Interfaces;

namespace Paybills.API.Application.Controllers.Email
{
    [Authorize]
    public class EmailController(SESService simpleEmailService, IUserRepository userRepository)
        : BaseApiController
    {
        private readonly SESService _simpleEmailService = simpleEmailService;

        [AllowAnonymous]
        [HttpGet]
        [Route("validate")]
        public async Task<ActionResult> ValidateEmail(string email, string emailToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(emailToken);

            if (DateTime.Compare(token.ValidTo, DateTime.Now) < 0)
                return BadRequest("Token expired");

            var emailValidator = new EmailAddressAttribute();

            if (!emailValidator.IsValid(email))
                return BadRequest("Invalid email");

            var user = await userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            if (user.EmailToken == emailToken)
            {
                if (user.EmailValidated)
                    return Redirect("/email/email-already-validated.html");

                user.EmailValidated = true;

                userRepository.Update(user);

                await userRepository.SaveAllAsync();

                return Redirect("/email/validation-success.html");
            }

            return BadRequest();
        }
    }
}