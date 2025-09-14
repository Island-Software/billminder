using Paybills.API.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Domain.Services.Interfaces;
using System.Threading.Tasks;
using Paybills.API.Entities;

namespace Paybills.API.Controllers
{
    public class TestingController : BaseApiController
    {
        private readonly IHostEnvironment _environment;
        private readonly IEmailService _emailService;

        public TestingController(IHostEnvironment environment, IEmailService emailService)
        {
            _environment = environment;
            _emailService = emailService;
        }

        [HttpPost("test-email")]
        public async Task<ActionResult> TestEmail()
        {
            if (!_environment.IsDevelopment()) return Unauthorized();

            await _emailService.SendVerificationEmail(new AppUser
            {
                UserName = "Nilso",
                Email = "nilsojr@gmail.com",
                EmailToken = "1234567890"
            });

            return Ok("Email sent.");
        }
    }
}