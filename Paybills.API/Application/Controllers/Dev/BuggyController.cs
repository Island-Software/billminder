using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Controllers;
using Paybills.API.Data;
using Paybills.API.Entities;

namespace Paybills.API.Application.Controllers.Dev
{
    
    public class BuggyController : BaseApiController
    {
        /// This controller was created for the purpose of testing the error handling of the API

        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            this._context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var nonExistingUser = _context.Users.Find(-1);

            if (nonExistingUser == null) return NotFound();

            return Ok(nonExistingUser);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var nonExistingUser = _context.Users.Find(-1);

            var nonExistingUserToReturn = nonExistingUser.ToString();

            return nonExistingUserToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was a bad request");
        }
    }
}