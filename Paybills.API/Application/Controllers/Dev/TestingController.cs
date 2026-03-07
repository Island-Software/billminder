using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Paybills.API.Controllers;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Entities;

namespace Paybills.API.Application.Controllers.Dev
{
    public class TestingController(IHostEnvironment environment, IEmailService emailService) : BaseApiController
    {
        [HttpPost("test-email")]
        public async Task<ActionResult> TestEmail()
        {
            if (!environment.IsDevelopment()) return Unauthorized();

            await emailService.SendVerificationEmail(new AppUser
            {
                UserName = "Nilso",
                Email = "nilsojr@gmail.com",
                EmailToken = "1234567890"
            });

            return Ok("Email sent.");
        }
    }
}