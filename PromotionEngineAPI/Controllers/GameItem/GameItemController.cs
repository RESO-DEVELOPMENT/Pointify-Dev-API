using ApplicationCore.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers.GameItem
{
    [Route("api/game-item")]
    [ApiController]
    public class GameItemController : ControllerBase
    {
        private readonly IGameItemService _service;
        private readonly IGameCampaignService _gameService;
        public GameItemController(IGameItemService service, IGameCampaignService gameService)
        {
            _service = service;
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameItem([FromQuery] PagingRequestParam param, [FromQuery] Guid gameId)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => o.GameId.Equals(gameId) && !o.DelFlg);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGameItem([FromBody] GameItemDto dto)
        {
            if (dto.Id.Equals(Guid.Empty) || dto.GameId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var isExistGame = await _gameService.GetFirst(filter: o => o.Id.Equals(dto.GameId) && !o.DelFlg) != null;
            if (!isExistGame)
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
        public async Task<IActionResult> CreateGameItem([FromBody] GameItemDto dto)
        {
            if (dto.GameId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var isExistGame = await _gameService.GetFirst(filter: o => o.Id.Equals(dto.GameId) && !o.DelFlg) != null;
            if (!isExistGame)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
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
        public async Task<IActionResult> DeleteGame([FromRoute] Guid id)
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
