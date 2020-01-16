using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Models;
using GrandElementApi.Responses;
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
        public async Task<DataResponse<List<Request>>> Get()
        {
            try
            {
                var requests = await _requestService.AllRequestsAsync();
                return new DataResponse<List<Request>>(requests);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<Request>>.DefaultError(e.Message);
            }
        }
    }
}