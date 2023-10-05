﻿using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.DTOs.MemberAction;
using Infrastructure.DTOs.MemberActionType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.DTOs.Request;
using static Infrastructure.Helper.AppConstant.EnvVar;

namespace WebAPI.Controllers.MemberAction
{
    [Route("api")]
    [ApiController]
    public class MemberActionController : ControllerBase
    {
        private readonly IMemberActionService _service;

        public MemberActionController(IMemberActionService service)
        {
            _service = service;
        }

        // GET: api/v1/member-actions
        [HttpGet("member-actions")]
        public async Task<IActionResult> GetMemberAction([FromQuery] PagingRequestParam param)
        {
            var result = await _service.GetAsync(
                pageIndex: param.page,
                pageSize: param.size,
                filter: el => (bool) !el.DelFlag);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //GET: api/member-actions/{id}
        [HttpGet("member-actions/{id}")]
        public async Task<IActionResult> GetMemberAction([FromRoute] Guid id)
        {
            var result = await _service.GetFirst(filter: el => el.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //done
        // POST : api/member-action
        [HttpPost]
        [Route("member-action")]
        public async Task<IActionResult> MemberAction(MemberActionRequest request)
        {
            try
            {
                return Ok(await _service.CreateMemberAction(request));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //PATCH: api/member-actions/{id}
        [HttpPatch("member-actions/{id}")]
        public async Task<IActionResult> UpdateMemberAction([FromRoute] Guid id, [FromBody] MemberActionDto dto)
        {
            //check MemberAction
            var result = await _service.GetFirst(filter: el => el.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            var newMemberAction = await _service.UpdateAsync(dto);
            return Ok(newMemberAction);
        }
    }
}