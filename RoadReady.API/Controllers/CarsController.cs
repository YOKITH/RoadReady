using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarsControllers : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;
        public CarsControllers(ICarService carService,IMapper mapper)
        {
            _carService = carService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);

            if (car == null)
                return NotFound("Car not found");

            return Ok(car);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCars(string keyword)
        {
            var cars = await _carService.SearchCarsAsync(keyword);

            return Ok(cars);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] CarCreateDto cardto)
        {
            Car car = _mapper.Map<Car>(cardto);
            var result = await _carService.AddCarAsync(car);

            if (result==false)
            {
                return BadRequest();
            }

            return Ok("Car added successfully");
        }



        [Authorize (Roles = "Admin") ]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id,CarUpdateDto cardto)
        {
            var car = _mapper.Map<Car>(cardto);
            var result = await _carService.UpdateCarAsync(id, car);

            if (result == false)
            {
                return NotFound();
            }

            return Ok( "Car updated successfully");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);

            if (result == false)
                return NotFound();

            return Ok( "Car deleted successfully");
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedCars([FromQuery]PaginationParams paginationParams)
        {
            var result =await _carService.GetPagedCarsAsync(paginationParams);

            return Ok(result);
        }
    }
}