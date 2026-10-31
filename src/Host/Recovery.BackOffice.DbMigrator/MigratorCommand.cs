namespace Recovery.BackOffice.DbMigrator;

public sealed record MigratorCommand(bool Help, string[] ConfigurationArgs)
{
    public static MigratorCommand Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length == 0 || args is ["--help"] or ["-h"])
        {
            return new MigratorCommand(true, []);
        }

        if (!string.Equals(args[0], "apply", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Supported command: apply. Use --help for usage.", nameof(args));
        }

        return new MigratorCommand(false, args[1..]);
    }

    public const string HelpText = """
        Recovery BackOffice DbMigrator
        Usage: dotnet run --project src/Host/Recovery.BackOffice.DbMigrator -- apply [configuration arguments]
        Configure DatabaseOptions:ConnectionString and DatabaseOptions:Provider (MSSQL).
        Applies the tenant catalog, seeds the root tenant, then migrates Identity for every tenant.
        Exit codes: 0 success; 1 failure. No arguments or --help prints help without accessing a database.
        """;
}
