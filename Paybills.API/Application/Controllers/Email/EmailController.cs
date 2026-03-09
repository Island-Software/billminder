using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Controllers;
using Paybills.API.Infrastructure.Services;
using Paybills.API.Interfaces;

namespace Paybills.API.Application.Controllers.Email
{
    [Authorize]
    public class EmailController(SESService simpleEmailService, IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
        : BaseApiController
    {
        private readonly SESService _simpleEmailService = simpleEmailService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

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
                {
                    var alreadyValidatedPath = Path.Combine(_webHostEnvironment.WebRootPath, "email", "email-already-validated.html");
                    var alreadyValidatedContent = await System.IO.File.ReadAllTextAsync(alreadyValidatedPath);
                    return Content(alreadyValidatedContent, "text/html");
                }

                user.EmailValidated = true;

                userRepository.Update(user);

                await userRepository.SaveAllAsync();

                var successPath = Path.Combine(_webHostEnvironment.WebRootPath, "email", "validation-success.html");
                var successContent = await System.IO.File.ReadAllTextAsync(successPath);
                return Content(successContent, "text/html");
            }

            return BadRequest();
        }
    }
}