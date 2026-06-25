using FluentValidation;
using RoadReady.API.DTOs;

namespace RoadReady.API.Validators
{
    public class ReservationDtoValidator : AbstractValidator<Reservationdto>
    {
        public ReservationDtoValidator()
        {
            RuleFor(x => x.CarId)
                .GreaterThan(0);

            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.PickupDate)
                .GreaterThanOrEqualTo(DateTime.Today);

            RuleFor(x => x.DropoffDate)
                .GreaterThan(x => x.PickupDate)
                .WithMessage("Dropoff date must be after pickup date.");
        }
    }
}