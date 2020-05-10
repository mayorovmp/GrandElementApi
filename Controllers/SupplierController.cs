using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GrandElementApi.DTOs;
using AutoMapper;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierController(ILogger<SupplierController> logger, ISupplierService supplierService, IMapper mapper)
        {
            _logger = logger;
            _supplierService = supplierService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> Add(SupplierOnAddDTO supplier)
        {
            try
            {
                var r = await _supplierService.AddSupplierAsync(_mapper.Map<Supplier>(supplier));
                return _mapper.Map<SupplierDTO>(r);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<SupplierDTO>>> Get()
        {
            try
            {
                var suppliers = await _supplierService.AllSuppliersAsync();
                return _mapper.Map<List<SupplierDTO>>(suppliers);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet("{productId}")]
        public async Task<ActionResult<List<SupplierDTO>>> GetByProd(int productId)
        {
            try
            {
                var suppliers = await _supplierService.SuppliersByProductIdAsync(productId);
                return _mapper.Map<List<SupplierDTO>>(suppliers);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<SupplierDTO>> Edit(SupplierOnEditDTO supplier)
        {
            try
            {
                var data = await _supplierService.EditSupplierAsync(_mapper.Map<Supplier>(supplier));
                return _mapper.Map<SupplierDTO>(data);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
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