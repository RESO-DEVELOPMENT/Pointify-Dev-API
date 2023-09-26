using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers.Product
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProduct([FromQuery] PagingRequestParam param, [FromQuery] Guid productCateId)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => o.ProductCateId.Equals(productCateId) && !o.DelFlg);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [Authorize]
        [HttpGet]
        [Route("brand")]
        public async Task<IActionResult> GetBrandProduct([FromQuery] PagingRequestParam param, [FromQuery] Guid brandId)
        {
            try
            {
                var result = await _service.GetBrandProduct(PageIndex: param.page, PageSize: param.size, brandId: brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [Authorize]
        [HttpGet]
        [Route("brand/all/{brandId}")]
        public async Task<IActionResult> GetAllBrandProduct([FromRoute] Guid brandId)
        {
            try
            {
                var result = await _service.GetAllBrandProduct(brandId: brandId);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [Authorize]
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountProduct()
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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
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

        [Authorize]
        [HttpGet]
        [Route("exist")]
        public async Task<IActionResult> ExistProduct([FromQuery] string ProductCode, [FromQuery] Guid BrandId, [FromQuery] Guid ProductId)
        {
            if (String.IsNullOrEmpty(ProductCode) || BrandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                var result = await _service.CheckExistin(code: ProductCode, brandId: BrandId, productId: ProductId);
                return Ok(result);

            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] Guid id, [FromBody] ProductDto dto)
        {
            if (id != dto.ProductId || id.Equals(Guid.Empty))
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductDto dto)
        {
            if (await _service.CheckExistin(code: dto.Code, brandId: dto.BrandId, productId: Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, new ErrorObj((int)AppConstant.ErrCode.Product_Exist, AppConstant.ErrMessage.Product_Exist));
            }
            try
            {
                dto.ProductId = Guid.NewGuid();
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
        
        [Authorize]
        [HttpPost]
        [Route("sync-product")]
        public async Task<IActionResult> SyncProduct([FromQuery] Guid brandId, [FromBody] ProductRequestParam productRequestParam)
        {

            try
            {
                var result = await _service.SyncProduct(brandId, productRequestParam);
                return Ok(result);
            }

            catch (ErrorObj e)
            {

                return StatusCode(statusCode: e.Code, e);
            }

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid id)
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
        
        [HttpPost]
        [Route("check-product")]
        public async Task<IActionResult> CheckGiftProduct([FromBody] ProductDto dto)
        {
            try
            {
                var result = await _service.CheckGiftProduct(dto);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request);
                }

            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
    }
}
