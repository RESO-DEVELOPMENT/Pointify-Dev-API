using ApplicationCore.Services;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Controllers
{
    [Route("api/condition-rules")]
    [ApiController]
    //[Authorize]
    public class ConditionRulesController : ControllerBase
    {
        private readonly IConditionRuleService _service;

        public ConditionRulesController(IConditionRuleService service)
        {
            _service = service;
        }

        // GET: api/ConditionRules
        [HttpGet]
        public async Task<IActionResult> GetConditionRule([FromQuery] PagingRequestParam param, Guid BrandId)
        {
            try
            {
                var conditionRules = await _service.GetAsync(
               pageIndex: param.page,
               pageSize: param.size,
               filter: el => !el.DelFlg && el.BrandId.Equals(BrandId),
               orderBy: el => el.OrderByDescending(b => b.InsDate)
               , includeProperties: "ConditionGroup," +
                                    "ConditionGroup.ProductCondition," +
                                    "ConditionGroup.OrderCondition," +
                                    "PromotionTier");
                GenericRespones<ConditionRuleResponse> result = new GenericRespones<ConditionRuleResponse>(await _service.ReorderResult(conditionRules.Items), conditionRules.Size, conditionRules.Page, conditionRules.Total, conditionRules.TotalPages);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        // GET: api/ConditionRules/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountConditionRule()
        {
            return Ok(await _service.CountAsync(el => !el.DelFlg));
        }

        // GET: api/ConditionRules/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConditionRule([FromRoute] Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorObj((int)HttpStatusCode.BadRequest, AppConstant.ErrMessage.Bad_Request));
            }
            try
            {
                var rule = await _service.GetFirst(filter: el => el.ConditionRuleId.Equals(id) && !el.DelFlg,
                                    includeProperties: "ConditionGroup,ConditionGroup.ProductCondition,ConditionGroup.OrderCondition,"
                                    + "Brand");
                return Ok(await _service.ReorderResult(rule));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // PUT: api/ConditionRules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConditionRule([FromRoute] Guid id, [FromBody] UpdateConditionDto dto)
        {
            try
            {
                var result = await _service.UpdateRule(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpPost]
        public async Task<IActionResult> PostConditionRule(
            [FromBody] ConditionRuleDtoModel parammodel)
        {
            var param = new ConditionRuleDto()
            {
                BrandId = parammodel.BrandId,
                RuleName = parammodel.RuleName,
                Description = parammodel.Description,
                ConditionGroup = parammodel.ConditionGroup
            };
            try
            {
                var result = await _service.InsertConditionRule(param);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // DELETE: api/ConditionRules/5
        [HttpDelete]
        [Route("{conditionRuleId}")]
        public async Task<IActionResult> DeleteConditionRule([FromRoute] Guid conditionRuleId)
        {
            if (conditionRuleId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.Delete(conditionRuleId));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

        [HttpGet]
        [Route("for-voucher/{brandId}")]
        public async Task<IActionResult> GetConditionForCreateVoucher([FromRoute] Guid brandId)
        {
            if (brandId.Equals(Guid.Empty))
            {
                return StatusCode(statusCode: (int)HttpStatusCode.BadRequest, new ErrorResponse().BadRequest);
            }
            try
            {
                return Ok(await _service.GetAsync(filter: o => o.BrandId.Equals(brandId) && !o.DelFlg));
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }

        }

    }
}