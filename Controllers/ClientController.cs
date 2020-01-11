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
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly ClientService _clientService;

        public ClientController(ILogger<ClientController> logger, ClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }
        // GET: Client
        [HttpGet]
        public async Task<ApiResponse> Get()
        {
            try
            {
                var res = await _clientService.AllClientsAsync();
                return new DataResponse<List<Client>>() { Code = ApiResponse.OK, Success = true, Data = res };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }

        // GET: Client/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: Client
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: Client/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
