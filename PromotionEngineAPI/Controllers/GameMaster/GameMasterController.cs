using ApplicationCore.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers.GameMaster
{
    [Route("api/game-master")]
    [ApiController]
    [Authorize]
    public class GameMasterController : ControllerBase
    {
        private readonly IGameMasterService _service;
        public GameMasterController(IGameMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameMaster([FromQuery] PagingRequestParam param)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => !o.DelFlg);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateGameMaster([FromBody] GameMasterDto dto, [FromRoute] Guid id)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }
            if (dto.Id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                dto.UpdDate = DateTime.Now;
                var result = await _service.UpdateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameMaster([FromBody] GameMasterDto dto)
        {
            try
            {
                dto.UpdDate = DateTime.Now;
                dto.InsDate = DateTime.Now;
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteGameMaster([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.DeleteAsync(id);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
    }
}
