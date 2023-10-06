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
    public class BrandParnerController : ControllerBase
    {
        private readonly IBrandPartnerService _service;

        public BrandParnerController(IBrandPartnerService service)
        {
            _service = service;
        }

        [HttpGet("BrandPartners/{id}")]
        public async Task<IActionResult> GetBrandPartnerById(Guid id)
        {
            var result = await _service.GetBrandPartnerById(id);
            if (result == null)
            {
                return Ok("BrandPartner Không tồn tại trong hệ thống");
            }
            return Ok(result);
        }

        [HttpPost("BrandPartner")]
        public async Task<IActionResult> CreateNewBrandPartner(BrandPartnerDto dto)
        {
            var result =  await _service.CreateNewBrandPartner(dto);
            return Ok(result);
        }
        [HttpDelete("BrandPartners/{id}")]
        public async Task<IActionResult> DeleteBrandPartner (Guid id)
        {
            var result = await _service.DeleteBrandPartner(id);
            return Ok(result);
        }
    }
}
