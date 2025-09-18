using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.Receiving;
using Paybills.API.Controllers;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.DTOs;
using Paybills.API.Extensions;
using Paybills.API.Helpers;

namespace Paybills.API.Application.Controllers.Receivings
{
    [Authorize]
    public class ReceivingsController : BaseApiController
    {
        private readonly IReceivingService _receivingService;
        private readonly IMapper _mapper;
        private readonly IReceivingTypeService _receivingTypeService;
        // private readonly IUserService _userService;

        public ReceivingsController(IReceivingService receivingService, IMapper mapper, IReceivingTypeService receivingTypeService)
        {
            _receivingService = receivingService;
            _mapper = mapper;
            _receivingTypeService = receivingTypeService;
        }


        [HttpGet]
        [Route("name/{username}")]
        public async Task<ActionResult<IEnumerable<ReceivingDto>>> GetReceivings(string username, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }
            
            var receivings = await _receivingService.GetAsync(username, userParams);

            Response.AddPaginationHeader(receivings.CurrentPage, receivings.PageSize, receivings.TotalCount, receivings.TotalPages);

            var receivingsToReturn = _mapper.Map<IEnumerable<ReceivingDto>>(receivings);

            return Ok(receivingsToReturn);
        }

        [HttpGet]
        [Route("name/{username}/{month}/{year}")]
        public async Task<ActionResult<IEnumerable<ReceivingDto>>> GetByDate(string username, int month, int year, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }
            
            if (userParams.PageSize > 0)
            {
                var receivings = await _receivingService.GetByDateAsync(username, month, year, userParams);

                Response.AddPaginationHeader(receivings.CurrentPage, receivings.PageSize, receivings.TotalCount, receivings.TotalPages);

                var receivingsToReturn = _mapper.Map<IEnumerable<ReceivingDto>>(receivings);

                return Ok(receivingsToReturn);
            }
            else
            {
                var receivings = await _receivingService.GetByDateAsync(username, month, year);

                var receivingsToReturn = _mapper.Map<IEnumerable<ReceivingDto>>(receivings);

                return Ok(receivingsToReturn);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceivingDto>> GetReceiving(int id)
        {
            var result = await _receivingService.GetByIdAsync(id);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var receiving = await _receivingService.GetByIdAsync(id);

            if (receiving == null) return NotFound();

            await _receivingService.Delete(receiving);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ReceivingDto>> Create(ReceivingRegisterDto receivingRegisterDto)
        {
            var reeceivingType = await _receivingTypeService.GetByIdAsync(receivingRegisterDto.TypeId);

            if (reeceivingType == null) return BadRequest($"Receiving type of id {receivingRegisterDto.TypeId} not found");

            Receiving newReceiving = _mapper.Map<Receiving>(receivingRegisterDto);
            newReceiving.ReceivingType = reeceivingType;

            await _receivingService.Create(newReceiving);
            await _receivingService.AddToUser(receivingRegisterDto.UserId, newReceiving.Id);

            var receivingToReturn = _mapper.Map<ReceivingDto>(newReceiving);

            return CreatedAtAction(nameof(GetReceiving), new { id = receivingToReturn.Id }, receivingToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ReceivingRegisterDto receivingRegisterDto)
        {
            if (!await ReceivingExists(id))
                return NotFound();

            var repoReceiving = await _receivingService.GetByIdAsync(id);

            _mapper.Map(receivingRegisterDto, repoReceiving);

            await _receivingService.Update(repoReceiving);

            return Ok();
        }

        [HttpPost("copy")]
        public async Task<ActionResult> CopyToNextMonth(PeriodDataDto periodData)
        {
            await _receivingService.CopyToNextMonth(periodData.UserId, periodData.CurrentMonth, periodData.CurrentYear);            

            return Ok();
        }

        // TO-DO: create an out property for the found object
        private async Task<bool> ReceivingExists(int id)
        {
            return await _receivingService.GetByIdAsync(id) != null;
        }
    }
}