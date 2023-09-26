using ApplicationCore.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers.Customer
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService customerService)
        {
            _service = customerService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomer([FromQuery] PagingRequestParam param)
        {
            try
            {
                var result = await _service.GetAsync(
                               pageIndex: param.page,
                               pageSize: param.size,
                               filter: el => (bool)!el.Active,
                               orderBy: el => el.OrderByDescending(b => b.StartDate),
                               includeProperties: "Brand");
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCustomer([FromRoute] Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }



        // POST: api/customers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] CustomerDto dto)
        {

            try
            {
                dto.Id = Guid.NewGuid();
                dto.StartDate = DateTime.Now;
                //dto.Brand.InsDate = DateTime.Now;
                //dto.Brand.UpdDate = DateTime.Now;
                dto.Active = false;
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ErrorObj e)
            {
                return StatusCode(statusCode: e.Code, e);
            }
        }

        //PATCH: api/customers/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] Guid id, [FromBody] CustomerDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var result = await _service.UpdateAsync(dto);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer([FromQuery] string username)
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
    }
}

