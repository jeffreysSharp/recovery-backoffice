namespace Recovery.BackOffice.Modules.Auditing.Persistence;

/// <summary>SQL Server JSON scalar extraction, independent of JSON whitespace.</summary>
public static class AuditJsonFunctions
{
    public static string Value(string payload, string path) =>
        throw new InvalidOperationException("JSON_VALUE is only supported inside database queries.");
}
