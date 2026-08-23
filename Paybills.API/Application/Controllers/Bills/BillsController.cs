using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Domain.Entities;
using System.Security.Claims;
using Paybills.API.Application.DTOs.Bill;
using Paybills.API.Infrastructure.Extensions;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.API.Application.Controllers.Bills
{
    [Authorize]
    public class BillsController(IBillService billService, IBillTypeService billTypesRepository, IMapper mapper)
        : BaseApiController
    {
        [HttpGet]
        [Route("name/{username}")]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills(string username, [FromQuery] UserParams userParams)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user == null || user != username)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            var bills = await billService.GetBillsAsync(username, userParams);

            Response.AddPaginationHeader(bills.CurrentPage, bills.PageSize, bills.TotalCount, bills.TotalPages);

            var billsToReturn = mapper.Map<IEnumerable<BillDto>>(bills);            

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
                var bills = await billService.GetBillsByDateAsync(username, month, year, userParams);

                Response.AddPaginationHeader(bills.CurrentPage, bills.PageSize, bills.TotalCount, bills.TotalPages);

                var billsToReturn = mapper.Map<IEnumerable<BillDto>>(bills);

                return Ok(billsToReturn);
            }
            else
            {
                var bills = await billService.GetBillsByDateAsync(username, month, year);

                var billsToReturn = mapper.Map<IEnumerable<BillDto>>(bills);

                return Ok(billsToReturn);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var result = await billService.GetBillByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var bill = await billService.GetBillByIdAsync(id);

            if (bill == null) return NotFound();

            await billService.Delete(bill);

            return Ok();
        }

        [HttpPost("create")]
        public async Task<ActionResult<BillDto>> Create(BillRegisterDto bill)
        {
            var billType = await billTypesRepository.GetByIdAsync(bill.TypeId);

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

            await billService.Create(newBill);
            await billService.AddBillToUser(bill.UserId, newBill.Id);

            var billToReturn = mapper.Map<BillDto>(newBill);

            return CreatedAtAction(nameof(GetBill), new { id = billToReturn.Id }, billToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BillRegisterDto bill)
        {
            if (!await BillExists(id))
                return NotFound();

            var repoBill = await billService.GetBillByIdAsync(id);

            // TO-DO: add automapper to project
            repoBill.Value = bill.Value;
            repoBill.Month = bill.Month;
            repoBill.DueDate = bill.DueDate;
            repoBill.Year = bill.Year;
            repoBill.Paid = bill.Paid;

            await billService.Update(repoBill);

            return Ok();
        }

        // TO-DO: create an out property for the found object
        private async Task<bool> BillExists(int id)
        {
            return await billService.GetBillByIdAsync(id) != null;
        }
    }
}