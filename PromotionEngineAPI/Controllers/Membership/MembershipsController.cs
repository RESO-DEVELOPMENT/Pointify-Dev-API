﻿using System;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Services;
using System.Threading.Tasks;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/memberships")]
    [ApiController]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _service;

        public MembershipsController(IMembershipService service)
        {
            _service = service;
        }

        // GET: api/Memberships
        [HttpGet]
        // api/Memberships?pageIndex=...&pageSize=...
        public async Task<IActionResult> GetMembership([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size, filter: el => !el.DelFlg
             ,includeProperties: "MemberLevel,MemberProgram,MemberWallet,MembershipCard");
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet("apiKey")]
        // api/Memberships?pageIndex=...&pageSize=...&apiKey=...
        public async Task<IActionResult> GetMembershipbyApiKey([FromQuery] PagingRequestParam param, [FromQuery] Guid apiKey)
        {
            var result = await _service.GetAsync(pageIndex: param.page, pageSize: param.size, filter: el => !el.DelFlg 
            && el.MemberProgram.BrandId.Equals(apiKey)
             , includeProperties: "MemberLevel,MemberProgram,MemberWallet,MembershipCard");
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/Memberships/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountMembership()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

        // GET: api/Memberships/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembership([FromRoute] Guid id)
        {
            var result = await _service.GetMembershipById(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        // GET: api/Memberships/5
        [HttpGet("apiKey/{id}")]
        public async Task<IActionResult> GetMembershipByApiKey([FromRoute] Guid id,[FromQuery] Guid apiKey)
        {
            var result = await _service.GetMembershipByIdKey(id, apiKey);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/Memberships/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembership([FromRoute] Guid id,[FromQuery]Guid apiKey, [FromBody] UpMembership dto)
        {

            var result = await _service.UpdateMemberShip(id, dto, apiKey);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //done
        // POST: api/Memberships
        //tạo hàm create new member có đầu vào là apikey và dto
        [HttpPost]
        public async Task<IActionResult> CreateNewMember([FromQuery] Guid apiKey, [FromBody] MembershipDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var result = await _service.CreateNewMember(apiKey, dto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // DELETE: api/Memberships/5
        [HttpDelete]
        public async Task<IActionResult> DeleteMembership([FromQuery] Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            string result = await _service.DeleteMembership(id);
            return Ok(result);
        }
    }
}