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
        public async Task<DataResponse<List<Client>>> Get()
        {
            try
            {
                var res = await _clientService.AllClientsAsync();
                return new DataResponse<List<Client>>() { Code = DataResponse<object>.OK, Success = true, Data = res };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<Client>>.DefaultError(e);
            }
        }

    }
}
