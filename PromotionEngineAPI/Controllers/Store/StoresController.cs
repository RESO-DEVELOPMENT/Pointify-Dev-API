using ApplicationCore.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/stores")]
    [ApiController]
    [Produces("application/json")]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _service;

        public StoresController(IStoreService service)
        {
            _service = service;
        }

        // GET: api/Stores
        [HttpGet]
        public async Task<IActionResult> GetStore([FromQuery] PagingRequestParam param, [FromQuery] Guid BrandId)
        {
            try
            {
                return Ok(await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => !el.DelFlg && el.BrandId.Equals(BrandId),
                orderBy: el => el.OrderByDescending(obj => obj.InsDate)
                ));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        // GET: api/Stores
        [HttpGet]
        //[Authorize]
        [Route("promotions")]
        public async Task<IActionResult> GetPromotionForStore([FromQuery] string storeCode, [FromQuery] string brandCode)
        {
            try
            {
                var result = await _service.GetPromotionsForStore(brandCode, storeCode);
                if (result == null)
                {
                    return NoContent();
                }
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //public HttpResponseMessage Get(string brandCode)
        //{
        //    var stream = new FileStream($@"c:\listPromotion_{brandCode}.json", FileMode.Open);
        //    var result = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new StreamContent(stream)
        //};
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    return result;
        //}
        // GET: api/Stores
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeCode"></param>
        /// <param name="brandCode"></param>
        /// <returns> Json File </returns>
        [HttpGet]
        //[Authorize]
        [Route("promotionsJsonFile")]
        public async Task<IActionResult> GetPromotionForStoreInJsonFile([FromQuery] string storeCode, [FromQuery] string brandCode)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var stream = _service.Get(brandCode, storeCode);
                    stream.Position = 0;
                    stream.CopyTo(ms);

                    string fileName = $"listPromotion_{storeCode}.json";
                    return new FileContentResult(ms.ToArray(), "application/json")
                    {
                        FileDownloadName = fileName
                    };
                    //return File(stream, MediaTypeNames.Text.Plain, fileName);
                };
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
        // GET: api/Stores/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountStore([FromQuery] Guid BrandId)
        {
            try
            {
                return Ok(await _service.CountAsync(el => !el.DelFlg && el.BrandId.Equals(BrandId)));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }
        [HttpPost]
        [Route("checkStoreCodeExist")]
        public async Task<IActionResult> CheckEmailExisting([FromBody] DuplicateParam param)
        {
            bool isExisting = false;
            isExisting = (await _service.GetAsync(filter: el =>
                    el.BrandId == param.BrandID
                   && (param.StoreId != Guid.Empty ? (el.StoreId != param.StoreId && el.StoreCode == param.StoreCode) : (el.StoreCode == param.StoreCode) && !el.DelFlg)
                   && !el.DelFlg)).Items.Count > 0;
            return Ok(isExisting);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore([FromRoute] Guid id)
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

        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore([FromRoute] Guid id, [FromBody] StoreDto dto)
        {
            try
            {
                if (id != dto.StoreId)
                    return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
                dto.UpdDate = DateTime.Now;
                return Ok(await _service.UpdateAsync(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // POST: api/Stores
        [HttpPost]
        public async Task<IActionResult> PostStore([FromBody] StoreDto dto)
        {
            try
            {
                dto.StoreId = Guid.NewGuid();
                return Ok(await _service.CreateAsync(dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        // DELETE: api/Stores/5
        [HttpDelete]
        public async Task<IActionResult> DeleteStore([FromQuery] Guid id)
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

        [HttpGet]
        [Route("promotion/{promotionId}")]
        public async Task<IActionResult> GetStoreOfPromotion([FromRoute] Guid promotionId, [FromQuery] Guid brandId)
        {
            if (promotionId.Equals(Guid.Empty) || brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {

                return Ok(await _service.GetStoreOfPromotion(promotionId: promotionId, brandId: brandId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPut]
        [Route("promotion/{promotionId}")]
        public async Task<IActionResult> UpdateStoreOfPromotion([FromRoute] Guid promotionId, [FromBody] UpdateStoreOfPromotion dto)
        {
            if (promotionId.Equals(Guid.Empty) || !promotionId.Equals(dto.PromotionId))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.UpdateStoreOfPromotion(dto: dto));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }
    }
}

