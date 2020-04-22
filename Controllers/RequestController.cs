using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.Data;
using GrandElementApi.DTOs;
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
        private readonly IMapper _mapper;

        public RequestController(ILogger<RequestController> logger, RequestService requestService, IMapper mapper)
        {
            _logger = logger;
            _requestService = requestService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<RequestDTO>>> Get()
        {
            try
            {
                var requests = await _requestService.AllRequestsAsync();
                return _mapper.Map<List<RequestDTO>>(requests);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet("last")]
        public async Task<ActionResult<RequestDTO>> Get(
            [FromQuery(Name ="clientId")]int? clientId, 
            [FromQuery(Name = "addressId")]int? addressId,
            [FromQuery(Name = "productId")]int? productId,
            [FromQuery(Name = "supplierId")]int? supplierId,
            [FromQuery(Name = "carId")]int? carId)
        {
            try
            {
                var requests = await _requestService.GetLastRequest(clientId, addressId, productId, supplierId, carId);
                return _mapper.Map<RequestDTO>(requests);
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
        public async Task<ActionResult<List<RequestDTO>>> GetByDate(DateTime date)
        {
            try
            {
                var requests = await _requestService.GetRequestsAsync(date);
                return _mapper.Map<List<RequestDTO>>(requests);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<RequestDTO>> Add(RequestOnAddDTO r)
        {
            try
            {
                var res = await _requestService.Add(_mapper.Map<Request>(r));
                return _mapper.Map<RequestDTO>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPost("{parentId}")]
        public async Task<ActionResult> AddPart(Request r, int parentId)
        {
            try
            {
                r = await _requestService.Add(r);
                await _requestService.LinkRequest(parentId, r.Id);
                return Ok(r);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPost("complete/{id}")]
        public async Task<ActionResult> Complete(int id)
        {
            try
            {
                var res = await _requestService.Complete(id);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Edit(RequestOnEditDTO r)
        {
            try
            {
                await _requestService.EditAsync(_mapper.Map<Request>(r));
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
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
    }
}