using System;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Services;
using System.Threading.Tasks;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/memberships")]
    [ApiController]
    [Authorize]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _service;

        public MembershipsController(IMembershipService service)
        {
            _service = service;
        }

        // GET: api/Memberships
        [HttpGet]
        // api/Memberships?pageIndex=...&pageSize=...
        public async Task<IActionResult> GetMembership([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size, filter: el => !el.DelFlg);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/Memberships/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountMembership()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

        // GET: api/Memberships/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembership([FromRoute]Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/Memberships/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembership([FromRoute]Guid id, [FromBody] MembershipDto dto)
        {
            if (id != dto.MembershipId)
            {
                return BadRequest();
            }

            dto.UpdDate = DateTime.Now;

            var result = await _service.UpdateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        // POST: api/Memberships
        [HttpPost]
        public async Task<IActionResult> PostMembership([FromBody] MembershipDto dto)
        {
            dto.MembershipId = Guid.NewGuid();

            var result = await _service.CreateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            //var result = dto;

            return Ok(result);
        }

        // DELETE: api/Memberships/5
        [HttpDelete]
        public async Task<IActionResult> DeleteMembership([FromQuery]Guid id)
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
