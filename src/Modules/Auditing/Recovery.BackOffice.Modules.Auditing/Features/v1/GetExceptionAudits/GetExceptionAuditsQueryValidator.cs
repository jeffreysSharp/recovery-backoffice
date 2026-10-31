using FluentValidation;
using Recovery.BackOffice.Modules.Auditing.Contracts.v1.GetExceptionAudits;

namespace Recovery.BackOffice.Modules.Auditing.Features.v1.GetExceptionAudits;

public sealed class GetExceptionAuditsQueryValidator : AbstractValidator<GetExceptionAuditsQuery>
{
    public GetExceptionAuditsQueryValidator()
    {
        RuleFor(q => q)
            .Must(q => !q.FromUtc.HasValue || !q.ToUtc.HasValue || q.FromUtc <= q.ToUtc)
            .WithMessage("FromUtc must be less than or equal to ToUtc.");
    }
}