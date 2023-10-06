using ApplicationCore.Services;
using Infrastructure.DTOs.BrandPartner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers.BrandPartner
{
    [Route("api")]
    [ApiController]
    public class BrandPartnerController : ControllerBase
    {
        private readonly IBrandPartnerService _service;

        public BrandPartnerController(IBrandPartnerService service)
        {
            _service = service;
        }

        [HttpPost("BrandPartner")]
        public async Task<IActionResult> CreateBrandPartner([FromBody] BrandPartnerDto brandPartnerDto)
        {
            var result = await _service.CreateBrandPartner(brandPartnerDto);
            if(result == null)
            {
                return Ok("Create BrandPartner thất bại");
            }
            return Ok(result);
        }
        [HttpDelete("BrandPartner")]
        public async Task<IActionResult> DeleteBrandPartner([FromQuery] Guid id)
        {
            var result = await _service.DeleteBrandPartner(id);
            if(result == null)
            {
                return Ok("Xóa BrandPartner thất bại hoặc đã bị xoá");
            }
            return Ok(result + "/n Xoá thành công");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandPartner([FromQuery] Guid id)
        {
            var result = await _service.getBrandPartner(id);
            if(result == null)
            {
                return Ok("Không tìm thấy BrandPartner");
            }
            return Ok(result);
        }

    }
}
