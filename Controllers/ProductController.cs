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
        public async Task<DataResponse<List<ProductShort>>> Get()
        {
            try
            {
                var products = await _productService.AllProductsAsync();
                return new DataResponse<List<ProductShort>>(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<ProductShort>>.DefaultError(e.Message);
            }
        }
        [HttpPost]
        public async Task<DataResponse<ProductShort>> Add(ProductShort product)
        {
            try
            {
                product = await _productService.AddProductAsync(product);
                return new DataResponse<ProductShort>(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<ProductShort>.DefaultError(e.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<DataResponse<object>> Delete(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return new DataResponse<object>(null);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<object>.DefaultError(e.Message);
            }
        }
        [HttpPut]
        public async Task<DataResponse<ProductShort>> Edit(ProductShort product)
        {
            try
            {
                var res = await _productService.EditProductAsync(product);
                return new DataResponse<ProductShort>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<ProductShort>.DefaultError(e.Message);
            }
        }
    }
}