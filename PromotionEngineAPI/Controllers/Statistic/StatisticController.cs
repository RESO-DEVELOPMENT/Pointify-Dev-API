using ApplicationCore.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers.Statistic
{
    [Route("api/statistic")]
    [ApiController]
    [Authorize]
    public class StatisticController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public StatisticController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        [Route("promotion/status/{brandId}")]
        public async Task<IActionResult> CountPromotionStatus([FromRoute] Guid brandId)
        {
            try
            {
                return Ok(await _promotionService.CountPromotionStatus(brandId: brandId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpGet]
        [Route("promotion/distribution")]
        public async Task<IActionResult> DistributionCount([FromQuery] Guid promotionId, [FromQuery] Guid brandId)
        {

            try
            {
                return Ok(await _promotionService.DistributionStatistic(promotionId: promotionId, brandId: brandId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

    }
}
