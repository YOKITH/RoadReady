using AutoMapper;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository reservationRepository,ICarRepository carRepository,
            IMapper mapper,ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _carRepository = carRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            try
            {
                return await _reservationRepository.GetAllReservationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching all reservations.");

                throw;
            }
        }

        public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
        {
            try
            {
                var reservation =await _reservationRepository.GetReservationByIdAsync(
                            reservationId);

                if (reservation == null)
                    throw new KeyNotFoundException(
                        $"Reservation with ID {reservationId} not found.");

                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching reservation {ReservationId}",
                    reservationId);

                throw;
            }
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
        {
            try
            {
                return await _reservationRepository.GetReservationsByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching reservations for User {UserId}",
                    userId);

                throw;
            }
        }

        public async Task<ReservationResponseDto>CreateReservationAsync(Reservationdto dto)
        {
            try
            {
                var car =await _carRepository.GetCarByIdAsync(dto.CarId);

                if (car == null)
                    throw new KeyNotFoundException(
                        "Car not found.");

                if (!car.IsAvailable)
                    throw new InvalidOperationException(
                        "Car is currently unavailable.");

                if (dto.PickupDate < DateTime.UtcNow.Date)
                    throw new ArgumentException(
                        "Pickup date cannot be in the past.");

                if (dto.PickupDate >= dto.DropoffDate)
                    throw new ArgumentException(
                        "Invalid reservation dates.");

                bool alreadyBooked = await _reservationRepository.IsCarBookedAsync(dto.CarId,dto.PickupDate,dto.DropoffDate);

                if (alreadyBooked)
                    throw new InvalidOperationException(
                        "Car already booked for selected dates.");

                int totalDays = (dto.DropoffDate - dto.PickupDate).Days;
                var reservation = _mapper.Map<Reservation>(dto);

                reservation.TotalAmount = totalDays * car.PricePerDay;
                reservation.Status ="Pending";

                reservation.CreatedAt =DateTime.UtcNow;

                await _reservationRepository.AddReservationAsync(reservation);

                bool saved = await _reservationRepository.SaveChangesAsync();

                if (!saved)
                    throw new Exception("Failed to create reservation.");

                _logger.LogInformation(
                    "Reservation created successfully for User {UserId} and Car {CarId}",
                    dto.UserId,dto.CarId);

                return _mapper.Map<ReservationResponseDto>(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating reservation.");

                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);

                if (reservation == null)
                    throw new KeyNotFoundException(
                        $"Reservation with ID {reservationId} not found.");

                reservation.Status = "Cancelled";

                await _reservationRepository.UpdateReservationAsync(reservation);

                var result = await _reservationRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Reservation cancelled successfully. ReservationId: {ReservationId}",
                    reservationId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while cancelling reservation {ReservationId}",
                    reservationId);

                throw;
            }
        }

        public async Task<PagedResponse<Reservation>>GetPagedReservationsAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _reservationRepository.GetPagedReservationsAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching paged reservations.");

                throw;
            }
        }
    }
}