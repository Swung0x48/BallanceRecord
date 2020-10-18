using BallanceRecordApi.Contracts.V1.Requests;
using FluentValidation;

namespace BallanceRecordApi.Validators
{
    public class CreateRecordRequestValidator: AbstractValidator<CreateRecordRequest>
    {
        public CreateRecordRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}