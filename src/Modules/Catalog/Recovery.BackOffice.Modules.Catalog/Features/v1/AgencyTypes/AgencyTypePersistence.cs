using System.Net;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes;

internal static class AgencyTypePersistence
{
    internal static async Task SaveAsync(Data.CountriesDbContext db, CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct).ConfigureAwait(false); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new CustomException("Tipo de agência já cadastrado.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new CustomException("Não é possível excluir um tipo de agência vinculado a outros registros.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
    }
}
