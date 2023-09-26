using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using static Infrastructure.Helper.AppConstant;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/vouchers")]
    [ApiController]

    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _service;
        private readonly IChannelService _channelService;
        public VouchersController(IVoucherService service, IChannelService channelService)
        {
            _service = service;
            _channelService = channelService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetVoucher(
            [FromQuery] PagingRequestParam param,
            [FromQuery] Guid VoucherGroupId,
            [FromQuery] Guid PromotionId,
            [FromQuery] Guid ChannelId,
            [FromQuery] Guid PromotionTierId,
            [FromQuery] Guid StoreId,
            [FromQuery] string SearchCode = "",
            [FromQuery] int VoucherStatus = 1)
        {
            if (SearchCode.Contains("-"))
            {
                SearchCode = SearchCode.Split("-").Last();
            }

            Expression<Func<Voucher, bool>> filter = el => el.VoucherCode.ToUpper().Contains(SearchCode.ToUpper());
            Expression<Func<Voucher, bool>> filter2;
            if (!VoucherGroupId.Equals(Guid.Empty))
            {
                filter2 = el => el.VoucherGroupId.Equals(VoucherGroupId);
                filter = filter.And(filter2);
            }
            if (!PromotionId.Equals(Guid.Empty))
            {

                filter2 = el => el.PromotionId.Equals(PromotionId);
                filter = filter.And(filter2);
            }
            if (!PromotionTierId.Equals(Guid.Empty))
            {

                filter2 = el => el.PromotionTierId.Equals(PromotionTierId);
                filter = filter.And(filter2);
            }
            if (!ChannelId.Equals(Guid.Empty))
            {

                filter2 = el => el.ChannelId.Equals(ChannelId);
                filter = filter.And(filter2);
            }
            if (!StoreId.Equals(Guid.Empty))
            {

                filter2 = el => el.StoreId.Equals(StoreId);
                filter = filter.And(filter2);
            }
            if (VoucherStatus > AppConstant.VoucherStatus.ALL)
            {
                switch (VoucherStatus)
                {
                    case AppConstant.VoucherStatus.USED:
                        {
                            filter2 = el => el.IsUsed;
                            filter = filter.And(filter2);
                            break;
                        }
                    case AppConstant.VoucherStatus.UNUSED:
                        {
                            filter2 = el => !el.IsUsed;
                            filter = filter.And(filter2);
                            break;
                        }
                    case AppConstant.VoucherStatus.REDEMPED:
                        {
                            filter2 = el => el.IsRedemped;
                            filter = filter.And(filter2);
                            break;
                        }
                }
            }

            try
            {
                var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: filter,
                includeProperties: "Promotion,Channel,Store,Membership,GameCampaign",
                orderBy: el => el.OrderBy(obj => obj.Index)
                );
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("check-voucher")]
        [Authorize]
        public async Task<IActionResult> CheckVoucher(
         [Required][FromQuery] Guid VoucherGroupId,
            [FromQuery] string SearchCode = "")
        {
            if (SearchCode.Contains("-"))
            {
                SearchCode = SearchCode.Split("-").Last();
            }
            Expression<Func<Voucher, bool>> filter = el => el.VoucherGroupId.Equals(VoucherGroupId)
                                                    && el.VoucherCode.ToUpper().Equals(SearchCode.ToUpper());
            try
            {
                return Ok(await _service.GetCheckVoucherInfo(searchCode: SearchCode, voucherGroupId: VoucherGroupId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("promotion-voucher-count")]
        [Authorize]
        public async Task<IActionResult> CountPromotionVoucher([FromQuery] Guid PromotionId, [FromQuery] Guid VoucherGroupId)
        {
            if (PromotionId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                return Ok(await _service.PromoVoucherCount(promotionId: PromotionId, voucherGroupId: VoucherGroupId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


        [HttpPost]
        [Route("voucher-on-site/{promotionId}/{tierId}")]
        public async Task<IActionResult> GetVoucherForCustomer([FromBody] VoucherForCustomerModel param, Guid promotionId, Guid tierId)
        {
            try
            {
                await _service.GetVoucherForCusOnSite(param, promotionId, tierId);
                return Ok();
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        [Route("send-email/{promotionId}/{tierId}")]
        public async Task<IActionResult> GetVoucherForCustomerOther([FromBody] VoucherForOtherChannel param, Guid promotionId, Guid tierId)
        {
            try
            {
                var voucherForCus = await _channelService.DecryptCustomer(param);
                if (voucherForCus != null)
                {
                    await _service.GetVoucherForCusOnSite(voucherForCus, promotionId, tierId);
                }
                return Ok();
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("urlSession")]
        public IActionResult GenerateSessionForLink([FromQuery] DateTime datetime)
        {
            try
            {
                var encryptext = _service.Encrypt(datetime.ToString());

                return Ok(encryptext);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        [Route("isExpiredUrl")]
        public IActionResult ValidateSessionLink([FromQuery] string sst)
        {
            try
            {
                var decypted = _service.Decrypt(sst);

                var timeConvert = DateTime.Parse(decypted);

                var result = timeConvert.AddMinutes(2) >= DateTime.Now;

                if (!result)
                {
                    return Forbid();
                }

                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Vouchers/count
        [HttpGet]
        [Route("count")]
        [Authorize]
        public async Task<IActionResult> CountVoucher([FromQuery] Guid VoucherGroupId)
        {
            try
            {
                return Ok(await _service.CountAsync(el => el.VoucherGroupId.Equals(VoucherGroupId)));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Vouchers/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetVoucher([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        // PUT: api/Vouchers/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutVoucher([FromRoute] Guid id, [FromBody] VoucherDto dto)
        {
            try
            {
                if (id != dto.VoucherId)
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
                dto.UpdDate = DateTime.Now;
                return Ok(await _service.UpdateAsync(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        // PUT: api/Vouchers/get-voucher-cus
        [HttpPost]
        [Route("get-voucher-cus")]
        public async Task<IActionResult> GetVoucherForCustomer([FromBody] VoucherGroupDto dto)
        {
            try
            {
                return Ok(await _service.GetVoucherForCustomer(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        // PUT: api/Vouchers/active
        //[HttpPut]
        //[Route("update-voucher-applied")]
        //public async Task<IActionResult> UpdateVoucherApplied([FromBody] CustomerOrderInfo order)
        //{
        //    try
        //    {
        //        return Ok(await _service.UpdateVoucherApplied(order));
        //    }
        //    catch (ErrorObj e)
        //    {
        //        return StatusCode(statusCode: e.Code, e);
        //    }

        //}

        // POST: api/Vouchers
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostVoucher([FromBody] VoucherDto dto)
        {
            try
            {
                dto.VoucherId = Guid.NewGuid();
                return Ok(await _service.CreateAsync(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // DELETE: api/Vouchers/5
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteVoucher([FromQuery] Guid id)
        {
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
