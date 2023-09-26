using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/post-actions")]
    [ApiController]
    [Authorize]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftService _service;

        public GiftsController(IGiftService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetGift(
            [FromQuery] PagingRequestParam param,
            [FromQuery] Guid brandId,
            [FromQuery] int PostActionType = 0)
        {
            try
            {
                if (brandId.Equals(Guid.Empty))
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
                }
                Expression<Func<Gift, bool>> myFilter = el => !el.DelFlg && el.BrandId.Equals(brandId);
                if (PostActionType > 0)
                {
                    myFilter = el => !el.DelFlg
                                     && el.BrandId.Equals(brandId)
                                     && el.PostActionType == PostActionType;
                }
                return Ok(await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: myFilter,
                orderBy: el => el.OrderByDescending(b => b.InsDate)));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGift([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.GetGiftDetail(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpPut("{postActionId}")]
        public async Task<IActionResult> PutGift([FromRoute] Guid postActionId, [FromBody] GiftDto dto)
        {
            if (!postActionId.Equals(dto.GiftId) || dto.GiftId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var exist = (await _service.GetFirst(filter: o => o.GiftId.Equals(postActionId) && !o.DelFlg));
            if (exist == null)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                dto.UpdDate = DateTime.Now;
                dto.InsDate = exist.InsDate;
                var result = await _service.UpdateGift(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Gift([FromBody] GiftDto dto)
        {

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
        [Route("{postActionId}")]
        public async Task<IActionResult> DeleteGift([FromRoute] Guid postActionId)
        {
            if (postActionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            var exist = (await _service.GetFirst(filter: o => o.GiftId.Equals(postActionId) && !o.DelFlg));
            if (exist == null)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                var result = await _service.Delete(exist);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


    }
}