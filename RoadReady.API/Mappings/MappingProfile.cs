using AutoMapper;
using RoadReady.API.DTOs;
using RoadReady.API.Models;

namespace RoadReady.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CarCreateDto, Car>()
                .ForMember(dest => dest.CarId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservations,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reviews,
                    opt => opt.Ignore());

            CreateMap<CarUpdateDto, Car>()
                .ForMember(dest => dest.CarId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservations,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reviews,
                    opt => opt.Ignore());

            CreateMap<Reservationdto, Reservation>()
                .ForMember(dest => dest.ReservationId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Car,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Payment,
                    opt => opt.Ignore());

            CreateMap<Reservation, ReservationResponseDto>();

            CreateMap<UserUpdateDto, User>()
                 .ForMember(dest => dest.UserId,
        opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash,
        opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt,
        opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt,
        opt => opt.Ignore())
              .ForMember(dest => dest.Reservations, opt => opt.Ignore())
           .ForMember(dest => dest.Payments,
        opt => opt.Ignore())
             .ForMember(dest => dest.Reviews,opt => opt.Ignore());

            CreateMap<PaymentDto, Payment>()
                .ForMember(dest => dest.PaymentId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservation,
                    opt => opt.Ignore());

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IsActive,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservations,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Payments,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reviews,
                    opt => opt.Ignore());
        }
    }
}