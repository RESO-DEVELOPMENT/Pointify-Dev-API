using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.MembershipProgram;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers.MembershipProgram
{
    [Route("api/v1")]
    [ApiController]
    public class MembershipProgramController : ControllerBase
    {
        private readonly IMembershipProgramService _service;

        public MembershipProgramController(IMembershipProgramService service)
        {
            _service = service;
        }

        //done

        // GET:api/v1/membership-programs/{id}
        [HttpGet("membership-programs/{id}")]

        public async Task<IActionResult> GetMembershipProgram([FromRoute] Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.Id == id,
                includeProperties: "WalletType");
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        //done

        // GET:api/v1/membership-programs

        [HttpGet("membership-programs")]
        public async Task<IActionResult> GetMembershipProgram([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                               pageIndex: param.page,
                               pageSize: param.size,
                               filter: el => (bool)!el.DelFlg,
                               orderBy: el => el.OrderByDescending(b => b.StartDay));

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST: api/v1/membership-program
        [HttpPost("membership-program")]
        public async Task<IActionResult> PostMembershipProgram([FromBody] MembershipProgramDto membershipProgramDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.CreateAsync(membershipProgramDto);
            if (result == null)
            {
                return NotFound();
            }
            return CreatedAtAction("GetMembershipProgram", new { id = result.Id }, result);
        }

        //done
        // PATCH: api/v1/membership-programs/{id}
        [HttpPatch("membership-programs/{id}")]
        public async Task<IActionResult> PatchMembershipProgram([FromRoute] Guid id, [FromBody] MembershipProgramDto membershipProgramDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != membershipProgramDto.Id)
            {
                return BadRequest();
            }
            try
            {
                var result = await _service.UpdateAsync(membershipProgramDto);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

    }
}
