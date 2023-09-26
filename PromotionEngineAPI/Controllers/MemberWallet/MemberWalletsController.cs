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
        public async Task<ActionResult> GetMemberWallet([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => (bool)!el.DelFlag
                );

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/member-wallet/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetMemberWalletDetail(Guid id)
        {
            var memberWallet = _service.GetWalletDetail(id);

            if (memberWallet == null)
            {
                return NotFound();
            }

            return Ok(memberWallet);
        }

        // PATCH: api/member-wallet/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutMemberWallet([FromRoute] Guid id, [FromBody] MemberWalletDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var result = await _service.UpdateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/member-wallet
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostMemberWallet([FromBody] MemberWalletDto dto)
        {
            try
            {
                dto.Id = Guid.NewGuid();
                dto.DelFlag = false;
                dto.Balance = 0.0;
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
        public async Task<IActionResult> DeleteBrand([FromQuery] Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var result = await _service.DeleteAsync(id);
            if (result == false)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
