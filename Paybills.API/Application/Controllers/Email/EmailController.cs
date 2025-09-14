using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Infrastructure.Services;
using Paybills.API.Interfaces;

namespace Paybills.API.Controllers
{
    [Authorize]
    public class EmailController : BaseApiController
    {
        private readonly SESService _simpleEmailService;
        private readonly IUserRepository _userRepository;

        public EmailController(SESService simpleEmailService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _simpleEmailService = simpleEmailService;
        }

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

            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            if (user.EmailToken == emailToken)
            {
                if (user.EmailValidated)
                    return BadRequest("Email already validated");

                user.EmailValidated = true;

                _userRepository.Update(user);

                await _userRepository.SaveAllAsync();

                return Ok();
            }

            return BadRequest();
        }
    }
}