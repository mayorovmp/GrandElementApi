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
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> _logger;
        private readonly CarService _carService;

        public CarController(ILogger<CarController> logger, CarService carService)
        {
            _logger = logger;
            _carService = carService;
        }

        [HttpGet]
        public async Task<ApiResponse> Get()
        {
            try
            {
                var data = await _carService.AllCarsAsync();
                return new DataResponse<List<Car>>() { Code = ApiResponse.OK, Success = true, Data = data };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse> Add(Car car)
        {
            if(car.Owner == null)
                return ApiResponse.UserError("Укажите владельца");
            try
            {
                var data = await _carService.AddCarAsync(car);
                return new DataResponse<Car>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }

        [HttpPost("delete/{carId}")]
        public async Task<ApiResponse> Delete(int carId)
        {
            try
            {
                await _carService.DeleteCar(carId);
                return new DataResponse<Car>();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ApiResponse.DefaultError(e.Message);
            }
        }
    }
}
