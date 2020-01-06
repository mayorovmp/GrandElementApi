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
        public async Task<ApiResponse> Add(Supplier supplier)
        {
            try
            {
                var suppliers = await _supplierService.AllSuppliersAsync();
                return new DataResponse<Supplier>() { Code = ApiResponse.OK, Success = true, Data = supplier };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
        [HttpGet]
        public async Task<ApiResponse> Get()
        {
            try
            {
                var suppliers = await _supplierService.AllSuppliersAsync();
                return new DataResponse<List<Supplier>>() { Code = ApiResponse.OK, Success = true, Data = suppliers };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
    }
}