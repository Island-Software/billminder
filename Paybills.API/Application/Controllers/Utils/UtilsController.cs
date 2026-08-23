using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.Utils;
using Paybills.API.Domain.Services.Interfaces;

namespace Paybills.API.Application.Controllers.Utils;

[Authorize]
public class UtilsController(IUtilsService utilsService) : BaseApiController
{
    [HttpPost("copy")]
    public async Task<ActionResult> CopyBillsAndReceivingsToNextMonth(PeriodDataDto periodData)
    {
        await utilsService.CopyBillsAndReceivingsToNextMonth(periodData.UserId, periodData.CurrentMonth, periodData.CurrentYear, 
            periodData.CopyValues);            

        return Ok();
    }
}