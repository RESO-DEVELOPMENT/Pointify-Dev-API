using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Authorize]
    [Route("api/game-campaign")]
    [ApiController]
    public class GameCampaignController : ControllerBase
    {
        private readonly IGameCampaignService _service;

        public GameCampaignController(IGameCampaignService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetGameConfig([FromQuery] PagingRequestParam param, [FromQuery] Guid brandId)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => o.BrandId.Equals(brandId) && !o.DelFlg,
                      orderBy: el => el.OrderByDescending(b => b.InsDate),
                      includeProperties:"GameItems"
                    );
                result.Items = result.Items.Select(s =>
                {
                    s.Quantity = s.GameItems.Count();
                    return s;
                }).ToList();
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGame([FromBody] GameCampaignDto dto, [FromQuery] Guid gameConfigId)
        {
            if (dto.BrandId.Equals(Guid.Empty) || !dto.Id.Equals(gameConfigId) || gameConfigId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.UpdateGameCampaign(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameCampaignDto dto)
        {
            if (dto.BrandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }

            try
            {
                var result = await _service.CreateGameCampaign(dto, dto.StoreIdList);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteGameConfig([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.DeleteGameConfig(id);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetGameConfigInfo([FromQuery] Guid gameConfigId, [FromQuery] Guid brandId)
        {
            if (gameConfigId.Equals(Guid.Empty) || brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.GetFirst(filter: o => o.Id.Equals(gameConfigId) && o.BrandId.Equals(brandId) && !o.DelFlg,
                    includeProperties: "StoreGameCampaignMapping,GameItems");
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("device/{deviceId}/{gameCampaignId}")]
        public async Task<IActionResult> GetGameCampaignDevice([FromRoute] Guid deviceId, [FromRoute] Guid gameCampaignId)
        {
            if (deviceId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.GetGameCampaignItems(deviceId, gameCampaignId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("get-game-campaign-device/{deviceId}/brand/{brandId}")]
        public async Task<IActionResult> GetGameCampaignForDevice([FromRoute] Guid deviceId, [FromRoute] Guid brandId)
        {
            try
            {
                if (deviceId.Equals(Guid.Empty) || brandId.Equals(Guid.Empty)) 
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
                }
                var result = await _service.GetGameCampaignForDevice(deviceId: deviceId, brandId: brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
    }
}
