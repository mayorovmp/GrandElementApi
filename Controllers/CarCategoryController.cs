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
        public async Task<DataResponse<List<CarCategory>>> Get()
        {
            try
            {
                var categories = await _carCategoryService.AllCategoriesAsync();
                return new DataResponse<List<CarCategory>>() { Code = DataResponse<List<CarCategory>>.OK, Success = true, Data = categories };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<CarCategory>>.DefaultError();
            }
        }

        [HttpPost]
        public async Task<DataResponse<CarCategory>> Add(CarCategory categoryReq)
        {
            try
            {
                var category = await _carCategoryService.AddCategoryAsync(categoryReq.Name);
                return new DataResponse<CarCategory>() { Code = DataResponse<CarCategory>.OK, Success = true, Data = category };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<CarCategory>.DefaultError(e.Message);
            }
        }

        [HttpPut]
        public async Task<DataResponse<CarCategory>> Edit(CarCategory categoryReq)
        {
            try
            {
                var category = await _carCategoryService.EditCategoryAsync(categoryReq);
                return new DataResponse<CarCategory>() { Code = DataResponse<CarCategory>.OK, Success = true, Data = category };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<CarCategory>.DefaultError(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<DataResponse<object>> Delete(int id)
        {
            try
            {
                await _carCategoryService.DeleteCategoryAsync(id);
                return new DataResponse<object>(null) ;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<object>.DefaultError(e.Message);
            }
        }
    }
}