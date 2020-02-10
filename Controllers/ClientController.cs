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
        public async Task<ActionResult<List<Client>>> Get()
        {
            try
            {
                var res = await _clientService.AllClientsAsync();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<Client>> Edit(Client client)
        {
            try
            {
                var data = await _clientService.EditClientAsync(client);
                return data;
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
        [HttpPost]
        public async Task<ActionResult<Client>> Add(Client client)
        {
            try
            {
                var data = await _clientService.AddClient(client);
                return data;
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
                await _clientService.DeleteClient(id);
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
