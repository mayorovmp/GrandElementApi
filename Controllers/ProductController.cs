using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.Data;
using GrandElementApi.DTOs;
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
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger, ProductService productService, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> Get()
        {
            try
            {
                var products = await _productService.AllProductsAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Add(ProductOnAddDTO product)
        {
            try
            {

                var newProduct = await _productService.AddProductAsync(_mapper.Map<Product>(product));
                return _mapper.Map<ProductDTO>(newProduct);
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
                await _productService.DeleteProductAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<ProductDTO>> Edit(ProductDTO product)
        {
            try
            {
                var res = await _productService.EditProductAsync(_mapper.Map<Product>(product));
                return _mapper.Map<ProductDTO>(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
    }
}