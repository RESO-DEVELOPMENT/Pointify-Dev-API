using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Services;

using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandsController(IBrandService service)
        {
            _service = service;
        }

        // GET: api/brands
        [HttpGet]
        // api/brands?pageIndex=...&pageSize=...
        public async Task<IActionResult> GetBrand([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => !el.DelFlg,
                orderBy: el => el.OrderByDescending(b => b.InsDate)
                );

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/brands/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountBrand()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

        // GET: api/brands/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand([FromRoute]Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.BrandId == id, 
                includeProperties: "Account,ConditionRule,Action,Channel,Promotion");
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/brands/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand([FromRoute]Guid id, [FromBody] BrandDto dto)
        {
            if (id != dto.BrandId)
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

        // POST: api/Brands
        [HttpPost]
        public async Task<IActionResult> PostBrand([FromBody] BrandDto dto)
        {
            dto.BrandId = Guid.NewGuid();

            var result = await _service.CreateAsync(dto);

            if (result == null)
            {
                return NotFound();
            }

            //var result = dto;

            return Ok(result);
        }

        // DELETE: api/Brands/5
        [HttpDelete]
        public async Task<IActionResult> DeleteBrand([FromQuery]Guid id)
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

        
    }
}