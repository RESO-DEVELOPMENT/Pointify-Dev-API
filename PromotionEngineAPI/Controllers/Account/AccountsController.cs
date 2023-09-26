using ApplicationCore.Services;
using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;
        //constructor
        public AccountsController(IAccountService service)
        {
            _service = service;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<IActionResult> GetAccount([FromQuery] PagingRequestParam param)
        {
            try
            {
                var result = await _service.GetAsync(
                               pageIndex: param.page,
                               pageSize: param.size,
                               filter: el => !el.DelFlg
                               && el.RoleId != AppConstant.EnvVar.AdminId,
                               orderBy: el => el.OrderByDescending(b => b.InsDate),
                               includeProperties: "Brand");
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/Accounts/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountAccount()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

        //GET: api/Accounts/5
        [HttpGet("{username}")]
        public async Task<IActionResult> GetAccount([FromRoute] string username)
        {
            var result = await _service.GetByUsernameAsync(username);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/Accounts/5
        [HttpPut("{username}")]
        public async Task<IActionResult> PutAccount([FromRoute] string username, [FromBody] AccountDto dto)
        {

            if (!username.Equals(dto.Username))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                dto.UpdDate = DateTime.Now;

                var result = await _service.UpdateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        // POST: api/Accounts
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAccount([FromBody] AccountDto dto)
        {
            try
            {
                DateTime now = Common.GetCurrentDatetime();
                dto.InsDate = now;
                dto.UpdDate = now;
                dto.RoleId = AppConstant.EnvVar.BrandId;
                dto.Password = Common.EncodeToBase64(dto.Password);
                dto.IsActive = true;
                dto.Brand.InsDate = now;
                dto.Brand.UpdDate = now;
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }


        // DELETE: api/Accounts/5
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromQuery] string username)
        {
            if (username == null)
            {
                return BadRequest();
            }
            var result = await _service.DeleteUsernameAsync(username);
            if (result == false)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost]
        [Route("checkEmailExisting")]
        public async Task<IActionResult> CheckEmailExisting([FromBody] AccountDto user)
        {
            bool isExisting = false;
            var account = await _service.GetFirst(filter: el => el.Email == user.Email);
            if (account != null)
            {
                isExisting = true;
            }
            return Ok(isExisting);
        }
        [HttpPost]
        [Route("checkUserExisting")]
        public async Task<IActionResult> CheckUserExisting([FromBody] AccountDto user)
        {
            bool isExisting = false;
            var account = await _service.GetFirst(filter: el => el.Username == user.Username);
            if (account != null)
            {
                isExisting = true;
            }
            return Ok(isExisting);
        }
        [HttpPost]
        [Route("checkPhoneExisting")]
        public async Task<IActionResult> CheckPhoneExisting([FromBody] AccountDto user)
        {
            bool isExisting = false;
            var account = await _service.GetFirst(filter: el => el.PhoneNumber == user.PhoneNumber);
            if (account != null)
            {
                isExisting = true;
            }
            return Ok(isExisting);
        }
    }
}
