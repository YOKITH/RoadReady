using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.DTOs;

namespace RoadReady.API.Services.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();

        Task<Car?> GetCarByIdAsync(int carId);

        Task<IEnumerable<Car>> SearchCarsAsync(string keyword);

        Task<bool> AddCarAsync(Car car);

        Task<bool> UpdateCarAsync(int carId, Car car);

        Task<bool> DeleteCarAsync(int carId);
        Task<PagedResponse<Car>>GetPagedCarsAsync(PaginationParams paginationParams);
    }
}