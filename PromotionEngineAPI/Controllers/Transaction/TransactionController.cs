using ApplicationCore.Request;
using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;


namespace PromotionEngineAPI.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }
        //[Authorize]
        [HttpPost]
        [Route("check-out")]
        public async Task<IActionResult> Checkout([FromBody] Order order, [FromQuery] Guid brandId, [FromQuery] Guid deviceId)
        {
            try
            {
                return Ok(await _service.PlaceOrder(brandId: brandId, order: order, deviceId: deviceId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        [HttpPost]
        [Route("{channelCode}/check-out-other")]
        public async Task<IActionResult> CheckoutOther([FromBody] Order order, string channelCode)
        {
            try
            {
                var result = await _service.PlaceOrderForChannel(order: order, channelCode);
                if (result != null)
                {
                    return Ok(
                     new
                     {
                         code = (int)HttpStatusCode.OK,
                         message = AppConstant.EnvVar.Success_Message,
                         order = result
                     });
                }
                else
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request);
                }

            }
            catch (ErrorObj e)
            {
                e.StackTrace = null;
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, e);
            }
            catch (Exception)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new
                {
                    code = (int)HttpStatusCode.BadRequest,
                    message = AppConstant.ErrMessage.Bad_Request
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransaction(
          [FromQuery] PagingRequestParam param,
          [FromQuery] Guid PromotionId)
        {
            try
            {
                var result = await _service.GetPromoTrans(promotionId: PromotionId, param: param);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

    }
}
