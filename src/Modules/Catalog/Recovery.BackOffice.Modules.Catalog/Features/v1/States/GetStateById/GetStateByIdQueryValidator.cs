using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;
using Recovery.BackOffice.Modules.Catalog.Data;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.GetStateById;

public sealed class GetStateByIdQueryValidator : AbstractValidator<GetStateByIdQuery>
{
    public GetStateByIdQueryValidator()
    {
        RuleFor(x => x.StateId).NotEmpty().WithMessage("Identificador é obrigatório.");

    }
}
