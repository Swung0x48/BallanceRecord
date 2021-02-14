using BallanceRecordApi.Contracts.V1.Requests;
using FluentValidation;

namespace BallanceRecordApi.Validators
{
    public class CreateRecordRequestValidator: AbstractValidator<CreateRecordRequest>
    {
        public CreateRecordRequestValidator()
        {
            RuleFor(x => x.MapHash)
                .Length(64);
            RuleFor(x => x.Time)
                .GreaterThanOrEqualTo(0);
        }
    }
}