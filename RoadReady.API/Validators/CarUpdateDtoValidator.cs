using FluentValidation;
using RoadReady.API.DTOs;

namespace RoadReady.API.Validators
{
    public class CarUpdateDtoValidator : AbstractValidator<CarUpdateDto>
    {
        public CarUpdateDtoValidator()
        {
            RuleFor(x => x.Brand)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100);

            RuleFor(x => x.PricePerDay)
                .GreaterThan(0);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}