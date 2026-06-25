using Microsoft.Extensions.Logging;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILogger<CarService> _logger;

        public CarService(ICarRepository carRepository,ILogger<CarService> logger)
        {
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            try
            {
                return await _carRepository.GetAllCarsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching all cars.");

                throw;
            }
        }

        public async Task<Car?> GetCarByIdAsync(int carId)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(carId);

                if (car == null)
                    throw new KeyNotFoundException(
                        $"Car with ID {carId} not found.");

                return car;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching car with ID {CarId}",carId);

                throw;
            }
        }

        public async Task<IEnumerable<Car>> SearchCarsAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    throw new ArgumentException(
                        "Search keyword cannot be empty.");

                return await _carRepository.SearchCarsAsync(keyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while searching cars.");

                throw;
            }
        }

        public async Task<bool> AddCarAsync(Car car)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(car.Brand))
                {
                    throw new ArgumentException("Car brand is required.");
                }

                if (string.IsNullOrWhiteSpace(car.Model))
                    throw new ArgumentException("Car model is required.");

                if (car.PricePerDay <= 0)
                    throw new ArgumentException("Price per day must be greater than zero.");

                await _carRepository.AddCarAsync(car);

                var result = await _carRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Car added successfully. Brand: {Brand}, Model: {Model}",
                    car.Brand,
                    car.Model);

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a car.");

                throw;
            }
        }

        public async Task<bool> UpdateCarAsync(int carId,Car car)
        {
            try
            {
                var existingCar =await _carRepository.GetCarByIdAsync(carId);

                if (existingCar == null)
                {
                    throw new KeyNotFoundException($"Car with ID {carId} not found.");
                }

                existingCar.Brand = car.Brand;
                existingCar.Model = car.Model;
                existingCar.Year = car.Year;
                existingCar.PricePerDay = car.PricePerDay;
                existingCar.Location = car.Location;
                existingCar.IsAvailable = car.IsAvailable;
                existingCar.ImageUrl = car.ImageUrl;
                existingCar.Description = car.Description;

                await _carRepository.UpdateCarAsync(existingCar);

                var result = await _carRepository.SaveChangesAsync();

                _logger.LogInformation("Car updated successfully. CarId: {CarId}",carId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while updating car {CarId}",carId);

                throw;
            }
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(carId);

                if (car == null)
                    throw new KeyNotFoundException(
                        $"Car with ID {carId} not found.");

                await _carRepository.DeleteCarAsync(car);

                var result = await _carRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Car deleted successfully. CarId: {CarId}",
                    carId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while deleting car {CarId}",carId);

                throw;
            }
        }

        public async Task<PagedResponse<Car>> GetPagedCarsAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _carRepository.GetPagedCarsAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching paged cars.");

                throw;
            }
        }
    }
}