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
        public async Task<ApiResponse> Get()
        {
            try
            {
                var categories = await _carCategoryService.AllCategoriesAsync();
                return new DataResponse<List<CarCategory>>() { Code = ApiResponse.OK, Success = true, Data = categories };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError();
            }
        }

        [HttpPost]
        public async Task<ApiResponse> Add(CarCategory categoryReq)
        {
            try
            {
                var category = await _carCategoryService.AddCategoryAsync(categoryReq.Name);
                return new DataResponse<CarCategory>() { Code = ApiResponse.OK, Success = true, Data = category };
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
                await _carCategoryService.DeleteCategoryAsync(id);
                return new DataResponse<CarCategory>() { Code = ApiResponse.OK, Success = true, Data = null };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
    }
}