using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        // GET: api/Roles
        [HttpGet]
        // api/Roles?pageIndex=...&pageSize=...
        public async Task<IActionResult> GetRole([FromQuery] PagingRequestParam param)
        {
            try
            {
                return Ok(await _service.GetAsync(
                filter: el => !el.DelFlg));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Roles/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountRole()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

       


    }
}
