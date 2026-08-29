using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.Receiving;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Infrastructure.Extensions;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Application.Controllers.Receivings
{
    [Authorize]
    public class ReceivingsController(
        IReceivingService receivingService,
        IMapper mapper,
        IReceivingTypeService receivingTypeService)
        : BaseApiController
    {
        [HttpGet]
        [Route("name/{username}")]
        public async Task<ActionResult<IEnumerable<ReceivingDto>>> GetReceivings(string username, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }
            
            var receivings = await receivingService.GetAsync(username, userParams);

            Response.AddPaginationHeader(receivings.CurrentPage, receivings.PageSize, receivings.TotalCount, receivings.TotalPages);

            var receivingsToReturn = mapper.Map<IEnumerable<ReceivingDto>>(receivings);

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
                var receivings = await receivingService.GetByDateAsync(username, month, year, userParams);

                Response.AddPaginationHeader(receivings.CurrentPage, receivings.PageSize, receivings.TotalCount, receivings.TotalPages);

                var receivingsToReturn = mapper.Map<IEnumerable<ReceivingDto>>(receivings);

                return Ok(receivingsToReturn);
            }
            else
            {
                var receivings = await receivingService.GetByDateAsync(username, month, year);

                var receivingsToReturn = mapper.Map<IEnumerable<ReceivingDto>>(receivings);

                return Ok(receivingsToReturn);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceivingDto>> GetReceiving(int id)
        {
            var result = await receivingService.GetByIdAsync(id);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var receiving = await receivingService.GetByIdAsync(id);

            if (receiving == null) return NotFound();

            await receivingService.Delete(receiving);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ReceivingDto>> Create(ReceivingRegisterDto receivingRegisterDto)
        {
            var receivingType = await receivingTypeService.GetByIdAsync(receivingRegisterDto.TypeId);

            if (receivingType == null) return BadRequest($"Receiving type of id {receivingRegisterDto.TypeId} not found");

            Receiving newReceiving = mapper.Map<Receiving>(receivingRegisterDto);
            newReceiving.ReceivingType = receivingType;

            await receivingService.Create(newReceiving);
            await receivingService.AddToUser(receivingRegisterDto.UserId, newReceiving.Id);

            var receivingToReturn = mapper.Map<ReceivingDto>(newReceiving);

            return CreatedAtAction(nameof(GetReceiving), new { id = receivingToReturn.Id }, receivingToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ReceivingRegisterDto receivingRegisterDto)
        {
            if (!await ReceivingExists(id))
                return NotFound();

            var repoReceiving = await receivingService.GetByIdAsync(id);

            mapper.Map(receivingRegisterDto, repoReceiving);

            await receivingService.Update(repoReceiving);

            return Ok();
        }

        // TO-DO: create an out property for the found object
        private async Task<bool> ReceivingExists(int id)
        {
            return await receivingService.GetByIdAsync(id) != null;
        }
    }
}