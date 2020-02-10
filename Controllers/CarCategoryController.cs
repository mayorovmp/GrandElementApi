using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarCategoryController : ControllerBase
    {
        private readonly ILogger<CarCategoryController> _logger;
        private readonly ICarCategoryService _carCategoryService;

        public CarCategoryController(ILogger<CarCategoryController> logger, ICarCategoryService carCategoryService)
        {
            _logger = logger;
            _carCategoryService = carCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarCategory>>> Get()
        {
            try
            {
                var categories = await _carCategoryService.AllCategoriesAsync();
                return categories;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CarCategory>> Add(CarCategory categoryReq)
        {
            try
            {
                var category = await _carCategoryService.AddCategoryAsync(categoryReq.Name);
                return category;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<CarCategory>> Edit(CarCategory categoryReq)
        {
            try
            {
                var category = await _carCategoryService.EditCategoryAsync(categoryReq);
                return category;
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
                await _carCategoryService.DeleteCategoryAsync(id);
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