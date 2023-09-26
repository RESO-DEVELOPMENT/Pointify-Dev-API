
using ApplicationCore.Services;

using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/promotion-tiers")]
    [ApiController]
    [Authorize]
    public class PromotionTiersController : ControllerBase
    {
        private readonly IPromotionTierService _service;

        public PromotionTiersController(IPromotionTierService service)
        {
            _service = service;
        }

        // GET: api/PromotionTiers
        //[HttpGet]
        //// api/PromotionTiers?pageIndex=...&pageSize=...
        //public async Task<IActionResult> GetPromotionTier([FromQuery] PagingRequestParam param)
        //{
        //    var result = await _service.GetAsync(pageIndex: param.PageIndex, pageSize: param.PageSize);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        // GET: api/PromotionTiers/count
        //[HttpGet]
        //[Route("count")]
        //public async Task<IActionResult> CountPromotionTier()
        //{
        //    return Ok(await _service.CountAsync());
        //}

        // GET: api/PromotionTiers/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetPromotionTier([FromRoute] Guid id)
        //{
        //    var result = await _service.GetByIdAsync(id);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotionTier([FromRoute] Guid id, [FromBody] PromotionTierDto dto)
        {
            if (!id.Equals(dto.PromotionTierId) || id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: 400, new ErrorObj(400, "Required promotion tier id"));
            }
            var exist = await _service.GetFirst(filter: el => el.PromotionTierId.Equals(id));
            if (exist == null)
            {
                return StatusCode(statusCode: 400, new ErrorObj(400, "Promotion tier is not exist"));
            }
            try
            {
                var result = await _service.UpdateTier(dto: dto, entity: exist);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }



        }
        [HttpPost]
        public async Task<IActionResult> PostPromotionTier([FromBody] PromotionTierDto dto)
        {
            try
            {
                var result = await _service.CreateTier(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePromotionTier([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: 400, new ErrorObj(400, "Required promotion tier id"));
            }
            try
            {
                return Ok(await _service.DeleteTier(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("available")]
        public async Task<IActionResult> GetAvailable([FromQuery] PagingRequestParam param, [FromQuery] Guid brandId, [FromQuery] string actionType, [FromQuery] string discountType)
        {
            try
            {
                return Ok(await _service.GetAvailable(pageSize: param.size, pageIndex: param.page, brandId: brandId, actionType: actionType, discountType: discountType));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("assign")]
        public async Task<IActionResult> AssignTierToPromo([FromQuery] Guid promotionId, [FromQuery] Guid promotionTierId)
        {
            if (promotionId.Equals(Guid.Empty) || promotionTierId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.AssignTierToPromo(promotionId: promotionId, promotionTierId: promotionTierId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


    }
}
