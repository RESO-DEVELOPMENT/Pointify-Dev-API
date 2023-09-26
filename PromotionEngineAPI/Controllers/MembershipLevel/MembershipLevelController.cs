using ApplicationCore.Services;
using Infrastructure.DTOs.MemberAction;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Infrastructure.DTOs.MembershipLevel;
using static Infrastructure.Helper.AppConstant;

namespace WebAPI.Controllers.MembershipLevel
{
    [Route("api")]
    [ApiController]
    public class MembershipLevelController : ControllerBase
    {
        private readonly IMembershipLevelService _service;

        public MembershipLevelController(IMembershipLevelService service)
        {
            _service = service;
        }
        // GET: api/v1/member-actions
        [HttpGet("membership-levels")]
        public async Task<IActionResult> GetMembershipLevel([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                               pageIndex: param.page,
                               pageSize: param.size,
                               filter: el => (bool)!el.DelFlag);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        //GET: api/member-actions/{id}
        [HttpGet("membership-levels/{id}")]
        public async Task<IActionResult> GetMembershipLevel([FromRoute] Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //done
        // POST : api/member-action
        [HttpPost("membership-level")]
        public async Task<IActionResult> CreateMembershipLevel([FromBody] MembershipLevelModel model)
        {
            var dto = new MembershipLevelDto
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                DelFlag = model.DelFlag,
                MaxPoint = model.MaxPoint,
                Status = model.Status,
                PointRedeemRate = model.PointRedeemRate,
                LevelRank = model.LevelRank
            };
            //check MembershipLevel
            var result = await _service.GetFirst(filter: el => el.Id == dto.Id);
            if (result != null)
            {
                return StatusCode(statusCode: StatusCodes.Status409Conflict, new ErrorObj(StatusCodes.Status409Conflict, "MembershipLevel is already exist"));
            }
            var newMembershipLevel = await _service.CreateAsync(dto);
            return StatusCode(statusCode: StatusCodes.Status201Created, newMembershipLevel);
        }
        //PATCH: api/membership-levels/{id}
        [HttpPatch("membership-levels/{id}")]
        public async Task<IActionResult> UpdateMembershipLevel([FromRoute] Guid id, [FromBody] MembershipLevelDto dto)
        {
            //check MembershipLevel
            var result = await _service.GetFirst(filter: el => el.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            var newMemberAction = await _service.UpdateAsync(dto);
            return Ok(newMemberAction);
        }
    }
}
