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
        [HttpPost]
        public async Task<ApiResponse> Add(ProductShort product)
        {
            try
            {
                product = await _productService.AddProductAsync(product);
                return new DataResponse<ProductShort>() { Code = ApiResponse.OK, Success = true, Data = product };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return new DataResponse<ProductShort>() { Code = ApiResponse.OK, Success = true, Data = null };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
        [HttpPost("edit/{id}")]
        public async Task<ApiResponse> Edit(int id, ProductShort product)
        {
            try
            {
                var res = await _productService.EditProductAsync(product);
                return new DataResponse<ProductShort>() { Code = ApiResponse.OK, Success = true, Data = res };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
    }
}