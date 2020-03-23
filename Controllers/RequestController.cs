using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Models;
using GrandElementApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly ILogger<RequestController> _logger;
        private readonly RequestService _requestService;

        public RequestController(ILogger<RequestController> logger, RequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Request>>> Get()
        {
            try
            {
                var requests = await _requestService.AllRequestsAsync();
                return requests;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpGet("excel/{date}")]
        public async Task<IActionResult> GetExcelByDate(DateTime date)
        {
            var result = new byte[0];
            try
            {
                result = await _requestService.ExcelGetRequestsAsync(date);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return File(result, "application/ms-excel", $"Employee.xlsx");
        }

        [HttpGet("{date}")]
        public async Task<ActionResult<List<Request>>> GetByDate(DateTime date)
        {
            try
            {
                var requests = await _requestService.GetRequestsAsync(date);
                return requests;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(Request r)
        {
            try
            {
                await _requestService.Add(r);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Edit(Request r)
        {
            try
            {
                await _requestService.EditAsync(r);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                await _requestService.Delete(id);
                return Ok();
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return Problem();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
    }
}