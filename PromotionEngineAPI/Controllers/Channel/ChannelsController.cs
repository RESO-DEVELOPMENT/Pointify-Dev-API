using ApplicationCore.Request;
using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.DTOs.VoucherChannel;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/channels")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelService _service;

        public ChannelsController(IChannelService service)
        {
            _service = service;
        }

        // GET: api/Channels
        [HttpGet]
        public async Task<IActionResult> GetChannel([FromQuery] PagingRequestParam param, [FromQuery] Guid BrandId)
        {
            var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => !el.DelFlg && el.BrandId.Equals(BrandId),
                orderBy: el => el.OrderByDescending(b => b.InsDate));

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost]
        [Route("checkChannelCodeExist")]
        [Authorize]
        public async Task<IActionResult> CheckChannelExisting([FromBody] DuplicateParam param)
        {
            bool isExisting = false;
            isExisting = (await _service.GetAsync(filter: el =>
                    el.BrandId == param.BrandID
                   && (param.ChannelId != Guid.Empty ? (el.ChannelId != param.ChannelId && el.ChannelCode == param.ChannelCode) : (el.ChannelCode == param.ChannelCode) && !el.DelFlg)
                   && !el.DelFlg)).Items.Count > 0;
            return Ok(isExisting);
        }
        // GET: api/Channels/count
        [HttpGet]
        [Route("count")]
        [Authorize]
        public async Task<IActionResult> CountChannel([FromQuery] Guid BrandId)
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg && el.BrandId.Equals(BrandId)));
        }

        // GET: api/Channels/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetChannel([FromRoute] Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/Channels/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutChannel([FromRoute] Guid id, [FromBody] ChannelDto dto)
        {
            if (id != dto.ChannelId)
            {
                return BadRequest();
            }

            dto.UpdDate = DateTime.Now;

            var result = await _service.UpdateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        // POST: api/Channels
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> PostChannel([FromBody] ChannelDto dto)
        {
            dto.ChannelId = Guid.NewGuid();
            if (dto.ChannelType == (int) AppConstant.ChannelType.Other)
            {
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(AppConstant.RSA_LENGTH_2048);
                dto.PublicKey = Common.EncodeToBase64(RSACryptoUtils.ExportPublicKey(rsaProvider));
                dto.PrivateKey = Common.EncodeToBase64(RSACryptoUtils.ExportPrivateKey(rsaProvider));
            }
            dto.ApiKey = Common.CreateApiKey();
            var result = await _service.CreateAsync(dto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // DELETE: api/Channels/5
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteChannel([FromQuery] Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var result = await _service.DeleteAsync(id);
            if (result == false)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpGet]

        [Route("{channelCode}/vouchers/{promotionId}")]
        public async Task<IActionResult> GetVoucherForChannel(Guid promotionId, string channelCode, [FromBody] VoucherChannelParam param)
        {
            try
            {
                if (channelCode != param.ChannelCode)
                {
                    return NotFound();
                }
                if (promotionId != param.PromotionId)
                {
                    return NotFound();
                }
                var result = await _service.GetVouchersForChannel(param);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("{channelCode}/brands/{brandCode}/store/{storeCode}/promotions")]
        public async Task<IActionResult> GetPromotionForChannel(string channelCode, string brandCode, string storeCode, [FromQuery] string key, string promotionType)
        {
            try
            {
                VoucherChannelParam param = new VoucherChannelParam
                {
                    ChannelCode = channelCode,
                    BrandCode = brandCode,
                    StoreCode = storeCode
                };
                var channel = await _service.GetFirst(filter: el => el.ChannelCode == channelCode
                                && el.Brand.BrandCode == brandCode,
                                includeProperties: "Brand");
                if (channel != null)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        return Unauthorized(new ErrorObj(code: (int) HttpStatusCode.NotFound, message: AppConstant.ErrMessage.ApiKey_Required));
                    }
                    if (channel.ApiKey.Equals(key))
                    {
                        return Ok(new
                        {
                            code = HttpStatusCode.OK,
                            message = AppConstant.EnvVar.Success_Message,
                            data = await _service.GetPromotionsForChannel(param, promotionType)
                        });
                    }
                    else
                    {
                        return Unauthorized(new ErrorObj(code: (int) HttpStatusCode.Unauthorized, message: AppConstant.ErrMessage.ApiKey_Not_Exist));
                    }
                }
                return StatusCode(statusCode: (int) HttpStatusCode.NotFound,
                    new ErrorObj(code: (int) HttpStatusCode.NotFound, message: AppConstant.ErrMessage.Not_Found_Resource));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpGet]
        [Route("promotions")]
        public async Task<IActionResult> GetPromotionForOtherChannel([FromBody] ChannelOtherRequestParam param, string promotionType)
        {
            try
            {
                var channel = await _service.GetFirst(filter: el => el.ChannelCode == param.ChannelCode);
                if (channel != null)
                {
                    var voucherParam = RSACryptoUtils.Decrypt(param.Hash, Common.DecodeFromBase64(channel.PrivateKey));
                    VoucherChannelParam voucher = JsonConvert.DeserializeObject<VoucherChannelParam>(voucherParam);
                    return Ok(await _service.GetPromotionsForChannel(voucher, promotionType));
                }
                return StatusCode(statusCode: (int) HttpStatusCode.NotFound,
                    new ErrorObj(code: (int) HttpStatusCode.NotFound, message: AppConstant.ErrMessage.Not_Found_Resource));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpGet]
        [Route("promotion/{promotionId}")]
        public async Task<IActionResult> GetChannelOfPromotion([FromRoute] Guid promotionId, [FromQuery] Guid brandId)
        {
            if (promotionId.Equals(Guid.Empty) || brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int) HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {

                return Ok(await _service.GetChannelOfPromotion(promotionId: promotionId, brandId: brandId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("promotion/{promotionId}")]
        public async Task<IActionResult> UpdateChannelOfPromotion([FromRoute] Guid promotionId, [FromBody] UpdateChannelOfPromotion dto)
        {
            if (promotionId.Equals(Guid.Empty) || !promotionId.Equals(dto.PromotionId))
            {
                return StatusCode(statusCode: (int) HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.UpdateChannelOfPromotion(dto: dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        [Route("test/encrypt")]
        public IActionResult EncryptData([FromBody] CustomerOrderInfo param)
        {
            var json = JsonConvert.SerializeObject(param.Attributes);

            var pubKey = Common.DecodeFromBase64("LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0KTUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUF6NW5hNGVGRkRiQ2Y2czBSSWU4VAp5RjZsZk1uS3p3MEFBVmZCVHVmK0VPQldSTGVoL2x6ZGJQMzNxdHVFN2U1bmhxQVh5anhWamhGckxSWFIrK3lrCktUK1h1d3NYUmxQOUVFS21Iamw5ZmRYakh0aEZlTjgzWXBLT0FudlFNVk0ySFdQMmxpQjZac00xMlh3MzN4YmYKeWdSY05TSEhiL1dlckRiNDFDQnU5N25ORzNLcVcvWEFYd3JIUEJ0azVZUmhwTDhvL0lnYXNrbW1BS3ZiNkhKcgpJUWVpMVErcDNZWWg2aCt2VkV2TitHaWgyaG15UkhBWGFoRG9aTFgxa2lMZWVOZk45Um1RWUFrc3hEeG5qOVNxCmFjQ2ZTelZObEZZVkxtZitROVlBb0pCd3kvM0FYN2h0L0kwZ09rZGg1OUJwUnMzajR5T29na2tSbHdoeFFneVgKaFFJREFRQUIKLS0tLS1FTkQgUFVCTElDIEtFWS0tLS0t");
            var hash = RSACryptoUtils.Encrypt(json, pubKey);

            List<ChannelItem> items = new List<ChannelItem>();
            foreach (var item in param.CartItems)
            {
                items.Add(
                    new ChannelItem
                    {
                        ProductCode = item.ProductCode,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        SubTotal = item.SubTotal
                    }
                   );
            }

            var vouchers = new List<ChannelCouponCode>();
            foreach (var voucher in param.Vouchers)
            {
                vouchers.Add(new ChannelCouponCode
                {
                    PromotionCode = voucher.PromotionCode,
                    VoucherCode = voucher.VoucherCode
                });
            }
            ChannelOtherRequestParam channelOtherRequestParam = new ChannelOtherRequestParam
            {
                ApiKey = "198wKzgcx1t1xjFhovqux6fHO2MOsUzIfR4hZUIfzqq",
                BrandCode = param.Attributes.StoreInfo.BrandCode,
                ChannelCode = "BeanOi",
                Amount = param.Amount,
                BookingDate = param.BookingDate,
                CartItems = items,
                Customer = new ChannelCustomer
                {
                    CustomerGender = 3,
                    CustomerLevel = "Nam"
                },
                Id = param.Id,
                ShippingFee = 0,
                Vouchers = vouchers,
                Hash = hash
            };
            return Ok(channelOtherRequestParam);
        }

        [HttpPost]
        [Route("decrypt")]
        public async Task<IActionResult> DecryptData([FromBody] ChannelOtherRequestParam request)
        {
            var voucherParam = RSACryptoUtils.Decrypt(request.Hash, Common.DecodeFromBase64("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcEFJQkFBS0NBUUVBbTlDOEl1R1NnUEJrYTVqM0Q5WHYrZFNSQ0pPbXZkUmdUQy9RUjYxUkhZOTlhQUoyCjhLNHJIVGdBWndlRDVOV1diVVF3cEJoL1dGUWo4cHhxeWJxWXNOMmxoeEpxTU1QYk5LNzJUQkVOWS9QemQ0N0QKcDU4SHZETVRVU29lY2tHbWNaaXN4K1BPRGFvbkRRczhrYWlJMFhrS3lFK0JWZzFReHYzeXhUODFTZ1hNaTFBNQp3NWxYejIwTkxCd0FjRGk5d2ZBRURsRUNZR254NjNLbzl6SkNURUNUTmhNc0ZGYUk0aGhtWENzNVE1R3Y0N0E3CkZ3N3NoS1d5U1FUdXpzeEFteWpyZnFjNXZRWlNtWXRIU2h0eC9id2RGTVBJVCtCbWxlSmpqYzhNZm5BNDh3MFYKMlhLN1JxdGJVL21BRncwZWoxeCtMbmxUalJKeDZZNXpaZURWY1FJREFRQUJBb0lCQUhERk9lVGs3V3Qwa0xsdgpGQ0RaN2IwYkkzelpvQ3h6c041ekhJTkQ1UmxINkxPR1ZSOE1ieGZPbUR2NUxIUktRWDBEaFZDK2lpd2JlWWoxCnZEUVVZTDVoTEpQOXQrMWpVeHRtSmN3WDYyRVVCbm5aVWJIWFgzbk9YWVM0dnlCaWMxeHo2MWtnZnRsVTlMNTAKQzNwQVNBV1RYVUpzaUdjSGJCY1paTU50WTl3V0JUMXpGcFhHa01LV2ZkRnpQMmhYMXBDT1grS0pGd2dmcmJsYgozaVFJa0x4aXYzbEkvR2VWa09Sa2pPY0xzelhxRWVqUTFKLzRKZ1gyR0I2MzNjRlFvbVJEMitoS1QxbEtmOGhqCi9EWTUzVnk0aDdhVUIwZVprWklIT2tVcWI0ZzczSGgrT01YNUVnQWYrWFVMNXFOam9EbUYrMUhxd1haSUFQS3kKcDFKVUNLRUNnWUVBeWlVV1FHc0hkeUtuVkwzKzhCc0t1RWtGVDFaM3FBMkhNdDNneUgrYVk1REJqNHV1RWo1MQovZ3VDdk9XS1NyK1VjNXBsbFhHQVlVendVZUJ0NVFqRzlsbnA0VTVkK1poenFKWlkraG5TUHNsaDZvVURHWU9HClhuYUFlNmt5cVV3eGdnV2kzSXdPYVU4ZU1WeXErYTBBSWswVjkyRDc0YXNHSkdSdzJhQkNpOE1DZ1lFQXhWUFUKdm5KeTEwR0Y4ZVpiZm5XY04yMDdjaGNYODY4ZXUwcDhIall2cUNFVEFaMVBRL2xGcVdXN2IzVGE2RFB6M2UxNwpNVWRjVnM5cGtTeGJlTnowNTk2dVpOYlU3cHFuVnlPYVA1TmpqK29aSmVwUnIwdjY1N2hRTGdoc1F5VGlnVWIrCmNobjFHRVo0Szh1ckh3SDdBUVlZZVIydVk5ZzlKQTRPTi9LaWFyc0NnWUVBaHdZKzFzaW5NK3p4MktrUW9WRnUKMTZudTRnL2YzV0VyN2M1SFY2WGtlcDAycmF1Zm1wQWVRSk52d0wyU29sdFZ6ZUpUK0g3WVFpWWlZSTZJMlhRRApjb3FjcnVLcDR3N3lNcW82eE5SNm0zWG84YjNuVkNPR25aS0tRQS9FeDFFZHdMd0REVTZBVWRlSFUzR1N3elBMCjR4MmFqcU01bklPZ2xxNkFzdDFabGdFQ2dZQXVldEdZem9LSWU1R2VhaUZSQjBqMXNWQVlUcUpBcnhZeERabHcKMEZpblpLc0NiVmgzYldiZ1FPdEdsS0xmb1NVbk9FSVZXSGJDcC9aNDBKYjNRQ3liMVZNRXc2bkNUa0Z1Z0I5YwpMRTUrWHFqdnltTFZSTE5rRTRFblpxcUJvbFdNRi9ubFRJSHo1dDExaTNMU1NmZ2l4Sm5OSlpXblJROFp5QW5ICldlcXg4d0tCZ1FDc1FLa3JHNlRJcFJTWUVEbWVJOUxBaG43WTVUUkI5RnphTnFzd0o5azRYL2NzK1EwS1RaRncKcWhRNC9FQlgrYjVzRENIcFJnY282ZytQdWNDcTBjVFpEL0ZrQmdWc2h0ZFdzM1JHRmthMFlDYzJhSkdkL3VzQgpNbnAyOThlamJ4TUhoK21PbFZMVnpVMUVTdW9UdW53UTZXb0x5UW5GV1ltVlJVQ1Jncm1IWmc9PQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ=="));
            VoucherChannelParam voucher = JsonConvert.DeserializeObject<VoucherChannelParam>(voucherParam);
            return Ok(voucher);
        }
    }
}