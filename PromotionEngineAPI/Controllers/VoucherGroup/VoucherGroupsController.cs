
using ApplicationCore.Services;
using ApplicationCore.Worker;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/voucher-groups")]
    [ApiController]
    [Authorize]
    public class VoucherGroupsController : ControllerBase
    {
        private readonly IVoucherGroupService _service;
        private readonly IVoucherWorker _workerService;

        public VoucherGroupsController(IVoucherGroupService service, IVoucherWorker workerService)
        {
            _service = service;
            _workerService = workerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVoucherGroup(
            [FromQuery] SearchPagingRequestParam param,
            [FromQuery] Guid BrandId,
            [FromQuery] int ActionType = 0,
            [FromQuery] int PostActionType = 0)
        {
            try
            {


                if (String.IsNullOrWhiteSpace(param.SearchContent))
                {
                    param.SearchContent = "";
                }
                Expression<Func<VoucherGroup, bool>> myFilter = el => !el.DelFlg
                                                             && el.BrandId.Equals(BrandId)
                                                             && el.VoucherName.ToLower().Contains(param.SearchContent.ToLower().Trim());
                Expression<Func<VoucherGroup, bool>> filter2;
                if (ActionType > 0)
                {
                    filter2 = el => el.Action.ActionType == ActionType && !el.Action.DelFlg;
                    myFilter = myFilter.And(filter2);
                }
                else if (PostActionType > 0)
                {
                    filter2 = el => el.Gift.PostActionType == PostActionType && !el.Gift.DelFlg;
                    myFilter = myFilter.And(filter2);
                }
                var result = await _service.GetAsync(
                    pageIndex: param.page, pageSize: param.size, filter: myFilter,
                    includeProperties: "Action,Gift,PromotionTier",
                    orderBy: el => el.OrderByDescending(b => b.InsDate));
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        /* [HttpGet]
         [Authorize]
         [Route("game")]
         // api/VoucherGroups/game
         public async Task<IActionResult> GetVoucherGroupForGame([FromQuery] PagingRequestParam param, [FromQuery] string brandCode,
             [FromQuery] string storeCode)
         {
             try
             {
                 if (storeCode == null || brandCode == null)
                     return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
                 return Ok(await _service.GetVoucherGroupForGame(PageIndex: param.PageIndex, PageSize: param.PageSize, StoreCode: storeCode, BrandCode: brandCode)); ;
             }
             catch (ErrorObj e)
             {
                 return StatusCode(statusCode: e.Code, e.Message);
             }
         }*/


        // GET: api/VoucherGroups/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountVoucherGroup()
        {
            try
            {
                return Ok(await _service.CountAsync(el => !el.DelFlg));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/VoucherGroups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherGroup([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                return Ok(await _service.GetDetail(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // PUT: api/VoucherGroups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVoucherGroup([FromRoute] Guid id, [FromBody] VoucherGroupDto dto)
        {
            try
            {
                if (id != dto.VoucherGroupId)
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
                return Ok(await _service.UpdateVoucherGroup(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        // POST: api/VoucherGroups
        [HttpPost]
        public async Task<IActionResult> PostVoucherGroup([FromBody] VoucherGroupDto dto)
        {
            try
            {
                dto.VoucherGroupId = Guid.NewGuid();
                await _service.CreateAsync(dto);

                _workerService.InsertVouchers(voucherDto: dto);
                return Ok(dto);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPut]
        [Route("add-more")]
        public async Task<IActionResult> AddMoreVoucher([FromQuery] Guid voucherGroupId, int quantity)
        {
            try
            {
                if (voucherGroupId.Equals(Guid.Empty))
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);

                }

                return Ok(await _service.AddMoreVoucher(voucherGroupId: voucherGroupId, quantity: quantity));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("check-before-add-more/{voucherGroupId}")]
        public async Task<IActionResult> CheckBeforeAddMoreVoucher([FromRoute] Guid voucherGroupId)
        {
            try
            {
                if (voucherGroupId.Equals(Guid.Empty))
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);

                }

                return Ok(await _service.GetAddMoreInfo(id: voucherGroupId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        // DELETE: api/VoucherGroups/5
        [HttpDelete]
        public async Task<IActionResult> DeleteVoucherGroup([FromQuery] Guid voucherGroupId)
        {
            if (voucherGroupId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.DeleteVoucherGroup(voucherGroupId);
                _workerService.DeleteVouchers(voucherGroupId: voucherGroupId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPut]
        [Route("hide/{id}")]
        public async Task<IActionResult> HideVoucherGroup([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.HideVoucherGroup(id);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("assign")]
        public async Task<IActionResult> AssignVoucherGroup([FromQuery] Guid voucherGroupId, [FromQuery] Guid promotionId)
        {
            if (voucherGroupId.Equals(Guid.Empty) || promotionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.AssignVoucherGroup(promotionId: promotionId, voucherGroupId: voucherGroupId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("reject")]
        public async Task<IActionResult> RejectVoucherGroup([FromQuery] Guid voucherGroupId, [FromQuery] Guid promotionId)
        {
            if (voucherGroupId.Equals(Guid.Empty) || promotionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.RejectVoucherGroup(voucherGroupId: voucherGroupId, promotionId: promotionId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("available")]
        public async Task<IActionResult> GetAvailableVoucherGroup([FromQuery] Guid brandId, [FromQuery] PagingRequestParam param)
        {
            if (brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.GetAvailable(PageSize: param.size, PageIndex: param.page, BrandId: brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpGet]
        [Route("for-promo/{brandId}")]
        public async Task<IActionResult> GetAvailableVoucherGroup([FromRoute] Guid brandId)
        {
            if (brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.GetVoucherGroupForPromo(brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("index-info/{voucherGroupId}")]
        public async Task<IActionResult> GetIndexInfo([FromRoute] Guid voucherGroupId)
        {
            if (voucherGroupId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.CheckAvailableIndex(voucherGroupId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }




    }
}
