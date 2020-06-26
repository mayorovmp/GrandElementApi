using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.Data;
using GrandElementApi.DTOs;
using GrandElementApi.Services;
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
        private readonly IMapper _mapper;

        public ClientController(ILogger<ClientController> logger, ClientService clientService, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _clientService = clientService;
        }
        // GET: Client
        [HttpGet]
        public async Task<ActionResult<List<ClientDTO>>> Get()
        {
            try
            {
                var res = await _clientService.AllClientsAsync();
                return _mapper.Map<List<ClientDTO>>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<ClientDTO>>> Get(string name)
        {
            try
            {
                var res = await _clientService.SearchClientsAsync(name);
                return _mapper.Map<List<ClientDTO>>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<ClientDTO>> Edit(ClientOnEditDTO client)
        {
            try
            {
                var data = await _clientService.EditClientAsync(_mapper.Map<Client>(client));
                return _mapper.Map<ClientDTO>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<ClientDTO>> Add(ClientOnAddDTO client)
        {
            try
            {
                var data = await _clientService.AddClient(_mapper.Map<Client>(client));
                return _mapper.Map<ClientDTO>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _clientService.DeleteClient(id);
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
