using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Models;
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
        public async Task<ActionResult<List<Car>>> Get()
        {
            try
            {;
                var data = await _carService.AllCarsAsync();
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult<Car>> Add(Car car)
        {
            if(car.Owner == null)
                return ValidationProblem("Укажите владельца");
            if (car.CarCategory?.Id == null)
                return ValidationProblem("Укажите категорию");
            try
            {
                var data = await _carService.AddCarAsync(car);
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Car>> Edit(Car car)
        {
            try
            {
                var data = await _carService.EditCarAsync(car);
                return data;
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return Problem();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpDelete("{carId}")]
        public async Task<ActionResult<object>> Delete(int carId)
        {
            try
            {
                await _carService.DeleteCar(carId);
                return Ok();
            }
            catch (Npgsql.PostgresException e)
            {
                _logger.LogError(e.ToString());
                return Problem();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }
    }
}
