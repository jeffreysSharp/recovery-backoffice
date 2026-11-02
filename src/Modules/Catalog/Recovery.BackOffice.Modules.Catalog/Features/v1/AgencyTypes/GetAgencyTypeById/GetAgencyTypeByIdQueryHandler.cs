using Mediator;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Recovery.BackOffice.Modules.Catalog.Contracts.Dtos;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
using Recovery.BackOffice.Modules.Catalog.Data;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.GetAgencyTypeById;
public sealed class GetAgencyTypeByIdQueryHandler(CountriesDbContext db) : IQueryHandler<GetAgencyTypeByIdQuery, AgencyTypeDto>
{
    public async ValueTask<AgencyTypeDto> Handle(GetAgencyTypeByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await db.AgencyTypes.AsNoTracking().Where(agencyType => agencyType.Id == query.AgencyTypeId)
            .Select(agencyType => new AgencyTypeDto(agencyType.Id, agencyType.Name, agencyType.Description, agencyType.IsActive))
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false) ?? throw new NotFoundException("Tipo de agência não encontrado.");
    }
}
