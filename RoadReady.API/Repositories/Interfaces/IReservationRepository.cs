using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();

        Task<Reservation?> GetReservationByIdAsync(int reservationId);

        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);

        Task AddReservationAsync(Reservation reservation);

        Task UpdateReservationAsync(Reservation reservation);
        Task<bool> IsCarBookedAsync(int carId, DateTime pickupDate, DateTime dropoffDate);

        Task DeleteReservationAsync(Reservation reservation);

        Task<bool> SaveChangesAsync();
        Task<PagedResponse<Reservation>> GetPagedReservationsAsync (PaginationParams paginationParams);
    }
}