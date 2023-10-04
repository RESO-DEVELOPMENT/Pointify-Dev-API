using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.MembershipProgram;
using Infrastructure.DTOs.WalletType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers.WalletType
{
    [Route("api")]
    [ApiController]
    public class WalletTypeController : ControllerBase
    {
        private readonly IWalletTypeService _service;

        public WalletTypeController(IWalletTypeService service)
        {
            _service = service;
        }
        //done
        // GET: api/v1/wallet-types
        [HttpGet("wallet-types")]
        public async Task<IActionResult> GetWalletType([FromQuery] Guid apiKey, [FromQuery] PagingRequestParam param)
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
        //GET: api/v1/wallet-types/{id}
        [HttpGet("wallet-types/{id}")]
        public async Task<IActionResult> GetWalletType([FromQuery] Guid apiKey, [FromRoute] Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.Id == id && el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //done
        // POST : api/v1/wallet-type
        [HttpPost("wallet-type")]
        public async Task<IActionResult> PostWalletType([FromQuery] Guid apiKey, [FromBody] WalletTypeDto dto)
        {
            //check MemberShipProgramId
            var result = await _service.GetFirst(filter: el => el.Name == dto.Name && el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result != null)
            {
                return StatusCode(statusCode: StatusCodes.Status409Conflict, new ErrorObj(StatusCodes.Status409Conflict, "WalletType Name is already exist"));
            }
            dto.Id = Guid.NewGuid();
            dto.DelFlag = false;
            var newWalletType = await _service.CreateAsync(dto);
            return StatusCode(statusCode: StatusCodes.Status201Created, newWalletType);
        }
        //PATCH: api/v1/wallet-type/{id}
        [HttpPatch("wallet-types/{id}")]
        public async Task<IActionResult> PatchWalletType([FromQuery]Guid apiKey,[FromRoute] Guid id, [FromBody] WalletTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.GetFirst(filter: el => el.Id == id && el.MemberShipProgram.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound();
            }
            dto.Id = result.Id;
            dto.DelFlag = result.DelFlag;
            dto.MemberShipProgramId = result.MemberShipProgramId;
            var newWalletType = await _service.UpdateAsync(dto);
            return Ok(newWalletType);
        }
    }
}
