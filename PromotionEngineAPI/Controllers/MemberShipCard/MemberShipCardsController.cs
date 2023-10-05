using Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using ApplicationCore.Services;
using ApplicationCore.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers.MemberShipCard
{
    [Route("api/membership-card")]
    [ApiController]
    public class MemberShipCardsController : ControllerBase
    {
        private readonly IMemberShipCardService _service;

        public MemberShipCardsController(IMemberShipCardService service)
        {
            _service = service;
        }

        // GET: api/membership-card
        [HttpGet]
        public async Task<ActionResult> GetMemberShipCard([FromQuery] PagingRequestParam param, [FromQuery] Guid apiKey)
        {
            var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                filter: el => (bool)el.Active
            && el.BrandId.Equals(apiKey));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/membership-card/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountMembershipCard([FromQuery] Guid apiKey)
        {
            return Ok(await _service.CountAsync(el => (bool)!el.Active && el.BrandId.Equals(apiKey)));
        }

        // PATCH: api/Memberships/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateMembershipCard([FromRoute] Guid id, [FromBody] MemberShipCardDto dto, [FromQuery] Guid apiKey)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var result = await _service.UpdateMemberShipCard(id, dto, apiKey);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        // POST: api/membership-card
        [HttpPost]
        public async Task<IActionResult> PostMembership([FromBody] MemberShipCardDto dto)
        {
            var check = await _service.GetFirst(filter: el => el.BrandId.Equals(dto.BrandId));
            if (check == null) { return NotFound(); }
            dto.Id = Guid.NewGuid();
            dto.MembershipCardCode = Common.makeCode(5);
            dto.Active = false;
            dto.CreatedTime = DateTime.Now;
            var result = await _service.CreateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            //var result = dto;

            return Ok(result);
        }

        //Kích hoạt khi khách hàng nhận thẻ cứng
        [HttpPost("add-code")]
        public async Task<IActionResult> AddMembership([FromQuery] Guid apiKey, [FromQuery] Guid id)
        {
            var result = _service.AddCodeForMember(id, apiKey);
            //var result = dto;

            return Ok(result);
        }
    }
}
