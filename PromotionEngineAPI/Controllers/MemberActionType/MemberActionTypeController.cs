using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.MemberActionType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers.MemberActionType
{
    [Route("api")]
    [ApiController]
    public class MemberActionTypeController : ControllerBase
    {
        private readonly IMemberActionTypeService _service;

        public MemberActionTypeController(IMemberActionTypeService service)
        {
            _service = service;
        }
        //done
        // GET: api/member-action-types
        [HttpGet("member-action-types")]
        public async Task<IActionResult> GetMemberActionTypes([FromQuery]Guid apiKey,[FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                               pageIndex: param.page,
                               pageSize: param.size,
                               filter: el => (bool)!el.DelFlag && el.MemberShipProgram.BrandId.Equals(apiKey));

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        //done

        // GET: api/member-action-types/{id}
        [HttpGet("member-action-types/{id}")]
        public async Task<IActionResult> GetMemberActionType([FromQuery]Guid apiKey,[FromRoute] Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.Id == id && el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        //done

        // POST: api/member-action-type
        [HttpPost("member-action-type")]
        public async Task<IActionResult> CreateMemberActionType([FromQuery]Guid apiKey,[FromBody] MemberActionTypeDto dto)
        {
            //check MemberActionType
            var result = await _service.GetFirst(filter: el => el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound(dto);
            }
            dto.Id = Guid.NewGuid();
            dto.DelFlag = false;
            var newMemberActionType = await _service.CreateAsync(dto);
            return StatusCode(statusCode: StatusCodes.Status201Created, newMemberActionType);
        }
        //done

        //PATCH: api/member-action-type/{id}
        [HttpPatch("member-action-types/{id}")]
        public async Task<IActionResult> UpdateMemberActionType([FromQuery]Guid apiKey,[FromRoute] Guid id, [FromBody] MemberActionTypeDto dto)
        {
            //check MemberActionType
            var result = await _service.GetFirst(filter: el => el.Id == id && el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound();
            }
            var newMemberActionType = await _service.UpdateAsync(dto);
            return StatusCode(statusCode: StatusCodes.Status201Created, newMemberActionType);
        }
    }
}
