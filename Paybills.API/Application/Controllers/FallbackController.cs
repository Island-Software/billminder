using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Paybills.API.Application.Controllers
{
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("wwwroot", "index.html"));
            return PhysicalFile(
                path,
                "text/HTML");
        }
    }
}