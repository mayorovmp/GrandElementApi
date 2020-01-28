using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using GrandElementApi.Responses;
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
        public async Task<DataResponse<Supplier>> Add(Supplier supplier)
        {
            try
            {
                supplier = await _supplierService.AddSupplierAsync(supplier);
                return new DataResponse<Supplier>(supplier);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Supplier>.DefaultError(e.Message);
            }
        }
        [HttpGet]
        public async Task<DataResponse<List<Supplier>>> Get()
        {
            try
            {
                var suppliers = await _supplierService.AllSuppliersAsync();
                return new DataResponse<List<Supplier>>(suppliers);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<Supplier>>.DefaultError(e.Message);
            }
        }
        [HttpPut]
        public async Task<DataResponse<Supplier>> Edit(Supplier supplier)
        {
            try
            {
                var data = await _supplierService.EditSupplierAsync(supplier);
                return new DataResponse<Supplier>(data);
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Supplier>.DefaultError();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Supplier>.DefaultError(e.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<DataResponse<object>> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
                return new DataResponse<object>();
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<object>.DefaultError();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<object>.DefaultError(e.Message);
            }
        }
    }
}