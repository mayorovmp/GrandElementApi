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
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        [HttpGet]
        public async Task<ApiResponse> Get()
        {
            try
            {
                var products = await _productService.AllProductsAsync();
                return new DataResponse<List<ProductShort>>() { Code = ApiResponse.OK, Success = true, Data = products };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
    }
}