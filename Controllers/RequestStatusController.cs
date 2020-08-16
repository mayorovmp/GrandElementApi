using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.DTOs;
using GrandElementApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RequestStatusController : ControllerBase
    {
        private readonly ILogger<RequestStatusController> _logger;
        private readonly RequestStatusService _requestStatusService;
        private readonly IMapper _mapper;

        public RequestStatusController(ILogger<RequestStatusController> logger, IMapper mapper, RequestStatusService service)
        {
            _logger = logger; 
            _requestStatusService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequestStatusDTO>>> Get() {

            try
            {
                var res = await _requestStatusService.Get();
                return _mapper.Map<List<RequestStatusDTO>>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
    }
}
