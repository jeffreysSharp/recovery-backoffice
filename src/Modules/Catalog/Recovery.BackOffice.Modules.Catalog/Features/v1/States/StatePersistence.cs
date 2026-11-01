using System.Net;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States;

internal static class StatePersistence
{
    internal static async Task SaveAsync(Data.CountriesDbContext db, CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct).ConfigureAwait(false); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new CustomException("Sigla ou código IBGE já cadastrado para este país.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new CustomException("Operação impedida por vínculo com país ou registros dependentes.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
    }
}
