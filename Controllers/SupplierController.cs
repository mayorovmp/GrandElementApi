using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierService _supplierService;

        public SupplierController(ILogger<SupplierController> logger, ISupplierService supplierService)
        {
            _logger = logger;
            _supplierService = supplierService;
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> Add(Supplier supplier)
        {
            try
            {
                supplier = await _supplierService.AddSupplierAsync(supplier);
                return supplier;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<Supplier>>> Get()
        {
            try
            {
                var suppliers = await _supplierService.AllSuppliersAsync();
                return suppliers;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpGet("{productId}")]
        public async Task<ActionResult<List<Supplier>>> GetByProd(int productId)
        {
            try
            {
                var suppliers = await _supplierService.SuppliersByProductIdAsync(productId);
                return suppliers;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<Supplier>> Edit(Supplier supplier)
        {
            try
            {
                var data = await _supplierService.EditSupplierAsync(supplier);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
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