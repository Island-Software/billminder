using System.Collections.Generic;
using System.Threading.Tasks;
using Paybills.API.DTOs;
using Paybills.API.Extensions;
using Paybills.API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Domain.Entities;
using System.Security.Claims;

namespace Paybills.API.Controllers
{
    [Authorize]
    public class BillsController : BaseApiController
    {
        private readonly IBillService _service;
        private readonly IBillTypeService _billTypeService;
        private readonly IMapper _mapper;

        public BillsController(IBillService billService, IBillTypeService billTypesRepository, IMapper mapper)
        {            
            _mapper = mapper;
            _service = billService;
            _billTypeService = billTypesRepository;
        }

        [HttpGet]
        [Route("name/{username}")]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills(string username, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            var bills = await _service.GetBillsAsync(username, userParams);

            Response.AddPaginationHeader(bills.CurrentPage, bills.PageSize, bills.TotalCount, bills.TotalPages);

            var billsToReturn = _mapper.Map<IEnumerable<BillDto>>(bills);            

            return Ok(billsToReturn);
        }

        [HttpGet]
        [Route("name/{username}/{month}/{year}")]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBillsByDate(string username, int month, int year, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            if (userParams.PageSize > 0)
            {
                var bills = await _service.GetBillsByDateAsync(username, month, year, userParams);

                Response.AddPaginationHeader(bills.CurrentPage, bills.PageSize, bills.TotalCount, bills.TotalPages);

                var billsToReturn = _mapper.Map<IEnumerable<BillDto>>(bills);

                return Ok(billsToReturn);
            }
            else
            {
                var bills = await _service.GetBillsByDateAsync(username, month, year);

                var billsToReturn = _mapper.Map<IEnumerable<BillDto>>(bills);

                return Ok(billsToReturn);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var result = await _service.GetBillByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var bill = await _service.GetBillByIdAsync(id);

            if (bill == null) return NotFound();

            await _service.Delete(bill);

            return Ok();
        }

        [HttpPost("create")]
        public async Task<ActionResult<BillDto>> Create(BillRegisterDto bill)
        {
            var billType = await _billTypeService.GetByIdAsync(bill.TypeId);

            if (billType == null) return BadRequest($"Bill type of id {bill.TypeId} not found");

            var newBill = new Bill
            {
                BillType = billType,
                Month = bill.Month,
                Year = bill.Year,
                Value = bill.Value,
                DueDate = bill.DueDate,
                Paid = bill.Paid
            };

            await _service.Create(newBill);
            await _service.AddBillToUser(bill.UserId, newBill.Id);

            var billToReturn = _mapper.Map<BillDto>(newBill);

            return CreatedAtAction(nameof(GetBill), new { id = billToReturn.Id }, billToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BillRegisterDto bill)
        {
            if (!await BillExists(id))
                return NotFound();

            var repoBill = await _service.GetBillByIdAsync(id);

            // TO-DO: add automapper to project
            repoBill.Value = bill.Value;
            repoBill.Month = bill.Month;
            repoBill.DueDate = bill.DueDate;
            repoBill.Year = bill.Year;
            repoBill.Paid = bill.Paid;

            await _service.Update(repoBill);

            return Ok();
        }

        [HttpPost("copy")]
        public async Task<ActionResult> CopyBillsToNextMonth(PeriodDataDto periodData)
        {
            await _service.CopyBillsToNextMonth(periodData.UserId, periodData.CurrentMonth, periodData.CurrentYear);            

            return Ok();
        }

        // TO-DO: create an out property for the found object
        private async Task<bool> BillExists(int id)
        {
            return await _service.GetBillByIdAsync(id) != null;
        }
    }
}