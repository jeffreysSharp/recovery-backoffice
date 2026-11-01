using System.Net;
using Recovery.BackOffice.BuildingBlocks.Core.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries;

internal static class CountryPersistence
{
    internal static async Task SaveAsync(Data.CountriesDbContext db, CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct).ConfigureAwait(false); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new CustomException("Já existe um país com o código ISO 2 ou ISO 3 informado.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new CustomException("Não é possível excluir um país vinculado a outros registros.", (IEnumerable<string>?)null, HttpStatusCode.Conflict);
        }
    }
}
