using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Implementations
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int carId)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
        }

        public async Task<IEnumerable<Car>> SearchCarsAsync(string keyword)
        {
            return await _context.Cars
                .Where(c =>
                    c.Brand.Contains(keyword) || c.Model.Contains(keyword) ||
                    c.Location.Contains(keyword))
                .ToListAsync();
        }

        public async Task AddCarAsync(Car car)
        {
             _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            return Task.CompletedTask;
        }

        public Task DeleteCarAsync(Car car)
        {
            _context.Cars.Remove(car);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<PagedResponse<Car>>GetPagedCarsAsync(PaginationParams paginationParams)
        {
            var query = _context.Cars.AsQueryable();

            var totalRecords = await query.CountAsync();

            var cars = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResponse<Car>
            {
                Data = cars,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationParams.PageSize)
            };
        }
    }
}