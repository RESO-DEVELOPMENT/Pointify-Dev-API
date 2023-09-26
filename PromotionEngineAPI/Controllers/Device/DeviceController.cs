using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DeviceController : ControllerBase
    {

        private readonly IDeviceService _service;
        private readonly IGameCampaignService _gameCampaignService;

        public DeviceController(IDeviceService service, IGameCampaignService gameCampaignService)
        {
            _service = service;
            _gameCampaignService = gameCampaignService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDevice([FromQuery] PagingRequestParam param, [FromQuery] Guid storeId)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => o.StoreId.Equals(storeId) && !o.DelFlg);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpGet]
        [Route("game-item")]
        public async Task<IActionResult> GetGameItem(string deviceCode, string brandCode, Guid gameCampaignId)
        {
            try
            {
                var device = await _service.GetFirst(filter: el =>
                            !el.DelFlg
                            && el.Store.Brand.BrandCode == brandCode
                            && el.Code != deviceCode,
                            includeProperties:
                            "Store.Brand," +
                            "Store.StoreGameCampaignMapping.GameCampaign.GameItems");

                var campaign = device.Store.StoreGameCampaignMapping.Select(s => s.GameCampaign);
                if (campaign != null)
                {
                    return Ok(campaign.FirstOrDefault(f => f.Id == gameCampaignId).GameItems);
                }
                return StatusCode(statusCode: (int)HttpStatusCode.NotFound);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountDevice()
        {
            try
            {
                return Ok(await _service.CountAsync());
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpGet]
        [Route("brand")]
        public async Task<IActionResult> GetBrandDevice([FromQuery] PagingRequestParam param, [FromQuery] Guid brandId)
        {
            try
            {
                var result = await _service.GetBrandDevice(PageSize: param.size, PageIndex: param.page, brandId: brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDevice([FromRoute] Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);

            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice([FromRoute] Guid id, [FromBody] DeviceDto dto)
        {
            if (id != dto.DeviceId || id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.Update(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostDevice([FromBody] DeviceDto dto)
        {
            if (dto.GameCampaignId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                dto.DeviceId = Guid.NewGuid();
                dto.Code = _service.GenerateCode(dto.DeviceId);
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        [Route("pair/{pairCode}/{channelCode}")]
        public async Task<IActionResult> PairDevice([FromRoute] string pairCode, [FromRoute] string channelCode)
        {

            try
            {
                var result = await _service.GetTokenDevice(pairCode, channelCode);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDevice([FromQuery] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.DeleteAsync(id);
                return Ok();
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("checkGameCode")]
        public async Task<IActionResult> CheckGameCode([FromQuery] Int64 code, [FromQuery] Guid gameCampaignId)
        {
            var firstDayOfTYear = new DateTime(2021, 01, 01);
            var gameCampaign = await _gameCampaignService.GetByIdAsync(gameCampaignId);

            bool isValidGameCd = false;
            if (gameCampaign != null)
            {
                try
                {
                    var now = Common.GetCurrentDatetime();

                    var dateRedemped = code - int.Parse(gameCampaign.SecretCode);
                    var dateRedempedStr = dateRedemped.ToString();
                    if (dateRedempedStr.Length < 10)
                    {
                        dateRedempedStr = "0" + dateRedempedStr;
                    }
                    var minus = DateTime.ParseExact(dateRedempedStr, AppConstant.FormatGameCode, CultureInfo.InvariantCulture);
                    var dateStr = new DateTime(minus.Year - 2000, minus.Month, minus.Day, minus.Hour, minus.Minute, 0).Add(new TimeSpan(firstDayOfTYear.Ticks)).AddMinutes(2);
                    isValidGameCd = dateStr.AddMinutes(gameCampaign.ExpiredDuration) >= now;
                }
                catch (Exception)
                {
                    isValidGameCd = false;
                }

            }
            return Ok(isValidGameCd);

        }

    }
}
