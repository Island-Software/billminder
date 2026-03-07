using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.ReceivingType;
using Paybills.API.Controllers;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;

namespace Paybills.API.Application.Controllers.Receivings
{
    [Authorize]
    public class ReceivingTypeController(IReceivingTypeService service, IMapper mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<ReceivingTypeDto>> Create(ReceivingTypeRegisterDto receivingTypeRegisterDto)
        {
            if (await service.Exists(receivingTypeRegisterDto.Description)) return BadRequest("Receiving type already exists");

            var newReceivingType = new ReceivingType
            {
                Description = receivingTypeRegisterDto.Description,
                Active = receivingTypeRegisterDto.Active
            };

            await service.Create(newReceivingType);

            var result = mapper.Map<ReceivingTypeDto>(newReceivingType);
            
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceivingTypeDto>>> GetAll() => Ok(mapper.Map<IEnumerable<ReceivingTypeDto>>(await service.GetAsync()));

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceivingTypeDto>> Get(int id) => mapper.Map<ReceivingTypeDto>(await service.GetByIdAsync(id));

        [HttpGet]
        [Route("search/{description}")]
        public async Task<ActionResult<IEnumerable<ReceivingTypeDto>>> GetByDescription(string description) => Ok(await service.GetByDescription(description));

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ReceivingTypeDto receivingType)
        {
            if (!await TypeExists(id)) return NotFound();

            var repoReceivingType = await service.GetByIdAsync(id);

            repoReceivingType.Description = receivingType.Description;
            repoReceivingType.Active = receivingType.Active;

            await service.Update(repoReceivingType);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var receivingType = await service.GetByIdAsync(id);

            if (receivingType == null) return NotFound();

            await service.Delete(receivingType);

            return Ok();
        }

        private async Task<bool> TypeExists(int id)
        {
            return await service.GetByIdAsync(id) != null;
        }
    }
}