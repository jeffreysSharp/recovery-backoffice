using FluentValidation;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.DeleteAgencyType;
public sealed class DeleteAgencyTypeCommandValidator : AbstractValidator<DeleteAgencyTypeCommand>
{
    public DeleteAgencyTypeCommandValidator() => RuleFor(request => request.AgencyTypeId).NotEmpty();
}
