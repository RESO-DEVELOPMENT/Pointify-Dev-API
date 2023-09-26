using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/actions")]
    [ApiController]
    //[Authorize]
    public class ActionsController : ControllerBase
    {
        private readonly IActionService _service;

        public ActionsController(IActionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAction(
            [FromQuery] PagingRequestParam param,
            [FromQuery] Guid brandId,
            [FromQuery] int ActionType = 0)
        {
            try
            {
                if (brandId.Equals(Guid.Empty))
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Not_Found_Resource));
                }
                Expression<Func<Infrastructure.Models.Action, bool>> myFilter = el => !el.DelFlg && el.BrandId.Equals(brandId);
                if (ActionType > 0)
                {
                    myFilter = el => !el.DelFlg
                                     && el.BrandId.Equals(brandId)
                                     && el.ActionType == ActionType;
                }
                var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: myFilter,
                orderBy: el => el.OrderByDescending(b => b.InsDate));
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAction([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.GetActionDetail(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpPut("{actionId}")]
        public async Task<IActionResult> PutAction([FromRoute] Guid actionId, [FromBody] ActionDto dto)
        {
            if (!actionId.Equals(dto.ActionId) || dto.ActionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var exist = (await _service.GetFirst(filter: o => o.ActionId.Equals(actionId) && !o.DelFlg));
            if (exist == null)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.NotFound, new ErrorObj((int)HttpStatusCode.NotFound, AppConstant.ErrMessage.Not_Found_Resource));
            }
            try
            {
                var now = Common.GetCurrentDatetime();
                dto.UpdDate = now;
                dto.InsDate = exist.InsDate;
                var result = await _service.UpdateAction(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Gift([FromBody] ActionModel actionModel)
        {
            var dto = new ActionDto()
            {
                Name = actionModel.Name,
                BrandId = actionModel.BrandId,
                ActionType = actionModel.ActionType,
                DiscountType = actionModel.DiscountType,
                DiscountQuantity = actionModel.DiscountQuantity,
                DiscountAmount = actionModel.DiscountAmount,
                DiscountPercentage = actionModel.DiscountPercentage,
                FixedPrice = actionModel.FixedPrice,
                MaxAmount = actionModel.MaxAmount,
                MinPriceAfter = actionModel.MinPriceAfter,
                OrderLadderProduct = actionModel.OrderLadderProduct,
                LadderPrice = actionModel.LadderPrice,
                BundlePrice = actionModel.BundlePrice,
                BundleQuantity = actionModel.BundleQuantity,
                BundleStrategy = actionModel.BundleStrategy
            };
            if (dto.BrandId.Equals(Guid.Empty))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {

                var result = await _service.MyAddAction(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpDelete]
        [Route("{actionId}")]
        public async Task<IActionResult> DeleteAction([FromRoute] Guid actionId)
        {
            if (actionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var exist = (await _service.GetFirst(filter: o => o.ActionId.Equals(actionId) && !o.DelFlg));
            if (exist == null)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.NotFound, new ErrorObj((int)HttpStatusCode.NotFound, AppConstant.ErrMessage.Not_Found_Resource));
            }
            try
            {
                var result = await _service.Delete(entity: exist);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
    }
}