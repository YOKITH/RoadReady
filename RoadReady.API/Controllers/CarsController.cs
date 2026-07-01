using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly RoadReadyDbContext _context;

        public CarsController(RoadReadyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars(
            [FromQuery] string? search,
            [FromQuery] string? type,
            [FromQuery] string? transmission,
            [FromQuery] int? seats,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? isAvailable)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(c => c.Make.ToLower().Contains(lowerSearch) || c.Model.ToLower().Contains(lowerSearch) || c.Description.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(c => c.Type == type);
            }

            if (!string.IsNullOrEmpty(transmission))
            {
                query = query.Where(c => c.Transmission == transmission);
            }

            if (seats.HasValue)
            {
                query = query.Where(c => c.Seats == seats.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(c => c.IsAvailable == isAvailable.Value);
            }

            var cars = await query.ToListAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CarId == id)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    User = new
                    {
                        r.User!.Id,
                        r.User.FullName
                    }
                })
                .ToListAsync();

            return Ok(new
            {
                car.Id,
                car.Make,
                car.Model,
                car.Year,
                car.Type,
                car.Transmission,
                car.FuelType,
                car.Seats,
                car.PricePerDay,
                car.IsAvailable,
                car.ImageUrl,
                car.Description,
                Reviews = reviews
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCar([FromBody] Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car carDto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            car.Make = carDto.Make;
            car.Model = carDto.Model;
            car.Year = carDto.Year;
            car.Type = carDto.Type;
            car.Transmission = carDto.Transmission;
            car.FuelType = carDto.FuelType;
            car.Seats = carDto.Seats;
            car.PricePerDay = carDto.PricePerDay;
            car.IsAvailable = carDto.IsAvailable;
            car.ImageUrl = carDto.ImageUrl;
            car.Description = carDto.Description;

            await _context.SaveChangesAsync();
            return Ok(car);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Car deleted successfully." });
        }
    }
}
