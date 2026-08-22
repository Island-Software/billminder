using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.Utils;
using Paybills.API.Domain.Services.Interfaces;

namespace Paybills.API.Application.Controllers.Utils;

[Authorize]
public class UtilsController : BaseApiController
{
    private readonly IBillService _service;

    public UtilsController(IBillService billService)
    {
        _service =  billService;
    }
        
    [HttpPost("copy")]
    public async Task<ActionResult> CopyBillsToNextMonth(PeriodDataDto periodData)
    {
        await _service.CopyBillsToNextMonth(periodData.UserId, periodData.CurrentMonth, periodData.CurrentYear, 
            periodData.CopyValues);            

        return Ok();
    }
}