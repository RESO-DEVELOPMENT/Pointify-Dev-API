using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/member-level")]
    [ApiController]
    [Authorize]
    public class MemberLevelController : ControllerBase
    {
        private readonly IMemberLevelService _service;

        public MemberLevelController(IMemberLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberLevel([FromQuery] PagingRequestParam param, [FromQuery] Guid brandId)
        {
            try
            {
                var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size,
                    filter: o => o.BrandId.Equals(brandId) && !o.DelFlg);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountMemberLevel()
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberLevel([FromRoute] Guid id)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMemberLevel([FromRoute] Guid id, [FromBody] MemberLevelDto dto)
        {
            if (id != dto.MemberLevelId || id.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                dto.UpdDate = DateTime.Now;
                var result = await _service.Update(dto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, new ErrorObj(500, e.Message));
            }
            //catch (ErrorObj e)
            //{
            //    return StatusCode(statusCode: e.Code, e);
            //}
        }
        [HttpGet]
        [Route("exist")]
        public async Task<IActionResult> ExistMemberLevel([FromQuery] string Level, [FromQuery] Guid BrandId, [FromRoute] Guid MemberLevelId)
        {
            if (String.IsNullOrEmpty(Level) || BrandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, value: new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.CheckExistingLevel(name: Level, brandId: BrandId, memberLevelId: MemberLevelId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostMemberLevel([FromBody] MemberLevelDto dto)
        {
            if (await _service.CheckExistingLevel(name: dto.Name, brandId: dto.BrandId, memberLevelId: Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, new ErrorObj((int)AppConstant.ErrCode.MemberLevel_Exist, AppConstant.ErrMessage.MemberLevel_Exist));
            }
            try
            {
                dto.MemberLevelId = Guid.NewGuid();
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

        [HttpDelete]
        public async Task<IActionResult> DeleteMemberLevel([FromQuery] Guid id)
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
    }
}
