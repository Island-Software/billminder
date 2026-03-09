using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.DTOs;
using Paybills.API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Entities;
using Paybills.API.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Application.Controllers;
using Paybills.API.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;

namespace Paybills.API.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();

            var usersToReturn = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            return _mapper.Map<UserDto>(await _userRepository.GetUserByIdAsync(id));
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