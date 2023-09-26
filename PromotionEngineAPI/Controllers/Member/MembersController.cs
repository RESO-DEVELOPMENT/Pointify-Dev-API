using Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using ApplicationCore.Services;
using Infrastructure.Helper;
using System.Net;
using ShaNetHoliday.Syntax.Composition;
using Microsoft.AspNetCore.Routing;

namespace WebAPI.Controllers.Member
{
    [Route("api/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;
       

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        // POST: api/members
        [HttpPost]
        public async Task<IActionResult> PostMember([FromBody] MemberDto dto)
        {

            try
            {
                var result = await _service.CreateMember(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/members/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberDetail([FromRoute] Guid id)
        {
            var result = await _service.GetMemberDetail(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/members/{id}
        [HttpGet("{id}/{programId}")]
        public async Task<IActionResult> GetMemberProgram([FromRoute] Guid id, [FromRoute] Guid programId)
        {
            var result = await _service.GetMemberProgram(id, programId);
           
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //PATCH:api/members/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateMember([FromRoute] Guid id, [FromBody] MemberDto dto)
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

        //PATCH:api/members/5
        [HttpPatch]
        [Route("{id}/delete")]
        public async Task<IActionResult> HideMember([FromRoute] Guid id, [FromBody] bool DelFlg)
        {
            var result = await _service.HideItem(id, DelFlg);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

    }
}
