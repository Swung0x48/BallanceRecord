using System;
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
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.RoomId)
                .Custom((s, context) =>
                {
                    if (string.IsNullOrEmpty(s)) return;
                    try
                    {
                        Guid.Parse(s);
                    }
                    catch (FormatException e)
                    {
                        context.AddFailure(e.Message);
                    }
                });
        }
    }
}