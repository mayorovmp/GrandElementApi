using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.Data;
using GrandElementApi.DTOs;
using GrandElementApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> _logger;
        private readonly CarService _carService;
        private readonly IMapper _mapper;

        public CarController(ILogger<CarController> logger, CarService carService, IMapper mapper)
        {
            _logger = logger;
            _carService = carService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> Get()
        {
            try
            {
                var data = await _carService.AllCarsAsync();
                return _mapper.Map<List<CarDTO>>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.ToString());
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<CarDTO>>> Search(
            [FromQuery(Name = "name")][BindRequired] string name,
            [FromQuery(Name = "limit")][BindRequired] int limit,
            [FromQuery(Name = "offset")][BindRequired] int offset)
        {
            try
            {
                var data = await _carService.Search(name, limit, offset);
                return _mapper.Map<List<CarDTO>>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.ToString());
            }
        }

        [HttpGet("favorite")]
        public async Task<ActionResult<List<CarDTO>>> Favorite([FromQuery] int lastDays, [FromQuery] int limit) {
            try
            {
                var data = await _carService.Favorite(lastDays, limit);
                return _mapper.Map<List<CarDTO>>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        public async Task<ActionResult<CarDTO>> Add(CarOnAddDTO car)
        {
            if(car.Owner == null)
                return ValidationProblem("Укажите владельца");
            if (car.CarCategory?.Id == null)
                return ValidationProblem("Укажите категорию");
            try
            {
                var data = await _carService.AddCarAsync(_mapper.Map<Car>(car));
                return _mapper.Map<CarDTO>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<CarDTO>> Edit(CarDTO car)
        {
            try
            {
                var data = await _carService.EditCarAsync(_mapper.Map<Car>(car));
                return _mapper.Map<CarDTO>(data);
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
        public async Task<ActionResult> Delete(int carId)
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
