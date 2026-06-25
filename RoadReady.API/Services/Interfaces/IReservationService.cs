using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();

        Task<Reservation?> GetReservationByIdAsync(int reservationId);

        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);

        Task<ReservationResponseDto> CreateReservationAsync(Reservationdto dto);

        Task<bool> CancelReservationAsync(int reservationId);
        Task<PagedResponse<Reservation>> GetPagedReservationsAsync(PaginationParams paginationParams);
    }
}