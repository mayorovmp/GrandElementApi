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
        public async Task<DataResponse<List<Car>>> Get()
        {
            try
            {
                var data = await _carService.AllCarsAsync();
                return new DataResponse<List<Car>>() { Code = DataResponse<List<Car>>.OK, Success = true, Data = data };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<List<Car>>.DefaultError(e.Message);
            }
        }

        [HttpPost]
        public async Task<DataResponse<Car>> Add(Car car)
        {
            if(car.Owner == null)
                return DataResponse<Car>.UserError("Укажите владельца");
            try
            {
                var data = await _carService.AddCarAsync(car);
                return new DataResponse<Car>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Car>.DefaultError(e.Message);
            }
        }

        [HttpPut]
        public async Task<DataResponse<Car>> Edit(Car car)
        {
            try
            {
                var data = await _carService.EditCarAsync(car);
                return new DataResponse<Car>(data);
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Car>.DefaultError();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return DataResponse<Car>.DefaultError(e.Message);
            }
        }

        [HttpDelete("{carId}")]
        public async Task<DataResponse<object>> Delete(int carId)
        {
            try
            {
                await _carService.DeleteCar(carId);
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
