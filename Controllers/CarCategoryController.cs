using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.Data;
using GrandElementApi.DTOs;
using GrandElementApi.Interfaces;
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
        private readonly IMapper _mapper;

        public CarCategoryController(ILogger<CarCategoryController> logger, ICarCategoryService carCategoryService, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _carCategoryService = carCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarCategoryDTO>>> Get()
        {
            try
            {
                var categories = await _carCategoryService.AllCategoriesAsync();
                return _mapper.Map<List<CarCategoryDTO>>(categories);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CarCategoryDTO>> Add(CarCategoryOnAddDTO categoryReq)
        {
            try
            {
                var category = await _carCategoryService.AddCategoryAsync(_mapper.Map<CarCategory>(categoryReq));
                return _mapper.Map<CarCategoryDTO>(category);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Problem(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<CarCategoryDTO>> Edit(CarCategoryDTO categoryReq)
        {
            try
            {
                var editedCat = _mapper.Map<CarCategory>(categoryReq);
                var category = await _carCategoryService.EditCategoryAsync(editedCat);
                return _mapper.Map<CarCategoryDTO>(category);
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