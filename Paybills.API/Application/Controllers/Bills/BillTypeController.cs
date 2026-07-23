using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Application.DTOs.BillType;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;

namespace Paybills.API.Application.Controllers.Bills
{
    [Authorize]
    public class BillTypeController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IBillTypeService _service;

        public BillTypeController(IBillTypeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<BillTypeDto>> Create(BillTypeRegisterDto billType)
        {
            if (await TypeExists(billType.Description)) return BadRequest("Bill type already exists");

            var newBillType = new BillType
            {
                Description = billType.Description,
                Active = billType.Active
            };

            await _service.Create(newBillType);

            return _mapper.Map<BillTypeDto>(newBillType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillTypeDto>>> GetBillTypes() => Ok(_mapper.Map<IEnumerable<BillTypeDto>>(await _service.GetAsync()));

        [HttpGet("{id}")]
        public async Task<ActionResult<BillTypeDto>> GetBillType(int id) => _mapper.Map<BillTypeDto>(await _service.GetByIdAsync(id));

        [HttpGet]
        [Route("search/{description}")]
        public async Task<ActionResult<IEnumerable<BillTypeDto>>> GetBillTypesByDescription(string description) => Ok(await _service.GetByDescription(description));

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BillTypeDto billType)
        {
            if (!await TypeExists(id)) return NotFound();

            var repoBillType = await _service.GetByIdAsync(id);

            repoBillType.Description = billType.Description;
            repoBillType.Active = billType.Active;

            await _service.Update(repoBillType);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var billType = await _service.GetByIdAsync(id);

            if (billType == null) return NotFound();

            await _service.Delete(billType);

            return Ok();
        }
        
        private async Task<bool> TypeExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }

        private async Task<bool> TypeExists(string description)
        {                    
            return await _service.Exists(description);
        }
    }
}