using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Paybills.API.Application.DTOs.User;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Services;
using Paybills.API.Infrastructure.Services.Interfaces;

namespace Paybills.API.Application.Controllers.Users
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private const int EMAIL_TOKEN_EXP_TIME_IN_DAYS = 1;
        private readonly IUserService _userRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private SESService _simpleEmailService;

        public UsersController(IUserService userRepository, IMapper mapper, ITokenService tokenService, SESService simpleEmailService)
        {
            _simpleEmailService = simpleEmailService;
            _tokenService = tokenService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) return NotFound();

            return _mapper.Map<UserDto>(user);
        }

        [HttpGet]
        [Route("name/{username}")]
        public async Task<ActionResult<UserEditDto>> GetUserByName(string username)
        {
            return _mapper.Map<UserEditDto>(await _userRepository.GetUserByUserNameWithDetailsAsync(username));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UserEditDto userDto)
        {
            var emailValidator = new EmailAddressAttribute();

            if (!emailValidator.IsValid(userDto.Email)) return BadRequest();

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) return NotFound();

            var validateEmail = user.Email != userDto.Email;

            user.Email = userDto.Email;
            if (validateEmail)
            {
                user.EmailToken = _tokenService.CreateToken(user, EMAIL_TOKEN_EXP_TIME_IN_DAYS);
                user.EmailValidated = false;
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                using var hmac = new HMACSHA512();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
                user.PasswordSalt = hmac.Key;
            }

            if (validateEmail)
                await SendEmailVerification(user);

            await _userRepository.UpdateAsync(user);

            return Ok();
        }

        private async Task<bool> SendEmailVerification(AppUser user)
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