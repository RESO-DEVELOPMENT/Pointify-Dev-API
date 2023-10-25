using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Models;
using Infrastructure.DTOs;
using ShaNetHoliday.Syntax.Composition;
using ApplicationCore.Services;
using Swashbuckle.Application;

namespace WebAPI.Controllers
{
    [Route("api/member-wallet")]
    [ApiController]
    public class MemberWalletsController : ControllerBase
    {
        private readonly IMemberWalletService _service;

        public MemberWalletsController(IMemberWalletService service)
        {
            _service = service;
        }

        // GET: api/member-wallet
        [HttpGet]
        public async Task<IActionResult> GetMemberWallet([FromQuery] PagingRequestParam param, [FromQuery] Guid apiKey)
        {
            var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => (bool)!el.DelFlag && el.WalletType.MemberShipProgram.BrandId.Equals(apiKey),
                includeProperties: "WalletType"
            );

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/member-wallet/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberWallet([FromRoute] Guid id, [FromQuery] Guid apiKey)
        {
            var result = await _service.GetMemberWalletByIdKey(id, apiKey);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PATCH: api/member-wallet/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutMemberWallet([FromRoute] Guid id, [FromBody] UpMemberWallet dto, [FromQuery] Guid apiKey)
        {
            var result = await _service.UpdateWallet(id, dto, apiKey);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/member-wallet
        [HttpPost]
        public async Task<IActionResult> PostMemberWallet([FromQuery] Guid apiKey, [FromBody] MemberWalletDto dto)
        {
            try
            {
                var check = await _service.GetFirst(filter: el => el.WalletType.MemberShipProgram
                .BrandId.Equals(apiKey));
                if (check == null) { return NotFound(); }
                dto.Id = Guid.NewGuid();
                dto.DelFlag = false;
                dto.Balance = 0;
                dto.BalanceHistory = 0;
                var result = await _service.CreateWallet(dto);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


        // DELETE: api/member-wallet/5
        [HttpDelete]
        public async Task<IActionResult> DeleteWallet([FromQuery] Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await _service.DeleteWallet(id);
            return Ok(result);
        }
    }
}