using RoadReady.API.Models;
using RoadReady.API.DTOs;
using RoadReady.API.Pagination;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();

        Task<Car?> GetCarByIdAsync(int carId);

        Task<IEnumerable<Car>> SearchCarsAsync(string keyword);

        Task AddCarAsync(Car car);

        Task UpdateCarAsync(Car car);

        Task DeleteCarAsync(Car car);

        Task<bool> SaveChangesAsync();

        Task<PagedResponse<Car>>GetPagedCarsAsync(PaginationParams paginationParams);
}
}