using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Recovery.BackOffice.DbMigrator;

try
{
    var command = MigratorCommand.Parse(args);
    if (command.Help)
    {
        await Console.Out.WriteLineAsync(MigratorCommand.HelpText);
        return 0;
    }

    var builder = Host.CreateApplicationBuilder(command.ConfigurationArgs);
    builder.Services.AddSingleton(MigratorConfiguration.Read(builder.Configuration));
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<MigrationDbContextFactory>();
    builder.Services.AddSingleton<IMigrationOperations, EfMigrationOperations>();
    builder.Services.AddSingleton<MigrationRunner>();
    using var host = builder.Build();
    await host.StartAsync();
    try
    {
        var cancellationToken = host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;
        await host.Services.GetRequiredService<MigrationRunner>().RunAsync(cancellationToken);
        return 0;
    }
    catch (Exception exception)
    {
        host.Services.GetRequiredService<ILogger<MigrationRunner>>()
            .LogError(exception, "Database migration failed.");
        return 1;
    }
    finally
    {
        await host.StopAsync();
    }
}
catch (Exception exception)
{
    await Console.Error.WriteLineAsync(exception.Message);
    return 1;
}
