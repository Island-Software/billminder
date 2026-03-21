using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Paybills.API.DTOs;
using Paybills.API.Entities;
using Paybills.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Domain.Services.Interfaces;

namespace Paybills.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private const int EXPIRATION_TIME_IN_DAYS = 7;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        
        public AccountController(IUserService userService, ITokenService tokenService, IEmailService emailService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _emailService = emailService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<LoginResultDto>> Register(RegisterDto registerDto)
        {
            if (await _userService.UserExistsAsync(registerDto.UserName)) 
                return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            
            await _userService.CreateAsync(user);

            await _emailService.SendVerificationEmail(user);

            return new LoginResultDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user, EXPIRATION_TIME_IN_DAYS),
                UserId = user.Id
            };
        }

        [HttpPost("login")]        
        public async Task<ActionResult<LoginResultDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.GetUserByUserNameAsync(loginDto.UserName);

            if (user == null) return Unauthorized("Invalid username/password");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid username/password");
            }

            if (!user.EmailValidated) return Unauthorized("Email not validated. Please check your inbox.");

            return new LoginResultDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user, EXPIRATION_TIME_IN_DAYS),
                UserId = user.Id
            };
        }
    }
}