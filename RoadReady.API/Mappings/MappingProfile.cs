using AutoMapper;
using RoadReady.API.DTOs;
using RoadReady.API.Models;

namespace RoadReady.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===========================
            // Register User
            // ===========================
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            // ===========================
            // Update User
            // ===========================
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            // ===========================
            // Create Car
            // ===========================
            CreateMap<CarCreateDto, Car>()
    .ForMember(dest => dest.CarId,
        opt => opt.Ignore())
    .ForMember(dest => dest.Reservations,
        opt => opt.Ignore())
    .ForMember(dest => dest.Reviews,
        opt => opt.Ignore())
    .ForMember(dest => dest.MaintenanceReports,
        opt => opt.Ignore())
    .ForMember(dest => dest.Status,
        opt => opt.Ignore());

            // ===========================
            // Update Car
            // ===========================
            CreateMap<CarUpdateDto, Car>()
    .ForMember(dest => dest.CarId,
        opt => opt.Ignore())

    .ForMember(dest => dest.Reservations,
        opt => opt.Ignore())

    .ForMember(dest => dest.Reviews,
        opt => opt.Ignore())

    .ForMember(dest => dest.Status,
        opt => opt.Ignore())

    .ForMember(dest => dest.MaintenanceReports,
        opt => opt.Ignore());

            // ===========================
            // Reservation
            // ===========================
            CreateMap<Reservationdto, Reservation>()
                .ForMember(dest => dest.ReservationId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Car, opt => opt.Ignore())
                .ForMember(dest => dest.Payment, opt => opt.Ignore());

            CreateMap<Reservation, ReservationResponseDto>();

            // ===========================
            // Payment
            // ===========================
            CreateMap<RazorpayPaymentDto, Payment>()
    .ForMember(dest => dest.PaymentId,
        opt => opt.Ignore())
    .ForMember(dest => dest.User,
        opt => opt.Ignore())
    .ForMember(dest => dest.Reservation,
        opt => opt.Ignore())
    .ForMember(dest => dest.PaymentDate,
        opt => opt.Ignore())
    .ForMember(dest => dest.Amount,
        opt => opt.Ignore())
    .ForMember(dest => dest.UserId,
        opt => opt.Ignore())
    .ForMember(dest => dest.PaymentStatus,
        opt => opt.Ignore());

            // ===========================
            // Review
            // ===========================
            CreateMap<ReviewDto, Review>()
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Car, opt => opt.Ignore());

            CreateMap<MaintenanceDto, MaintenanceReport>()
    .ForMember(dest => dest.ReportId,
        opt => opt.Ignore())
    .ForMember(dest => dest.ReportedBy,
        opt => opt.Ignore())
    .ForMember(dest => dest.ReportedDate,
        opt => opt.Ignore())
    .ForMember(dest => dest.ResolvedDate,
        opt => opt.Ignore())
    .ForMember(dest => dest.Status,
        opt => opt.Ignore())
    .ForMember(dest => dest.Car,
        opt => opt.Ignore())
    .ForMember(dest => dest.ReportedByUser,
        opt => opt.Ignore());
        }
    }
}