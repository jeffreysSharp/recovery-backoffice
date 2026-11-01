using Recovery.BackOffice.BuildingBlocks.Mailing;
using Recovery.BackOffice.BuildingBlocks.Mailing.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Recovery.BackOffice.Framework.Tests.Mailing;

public sealed class MailingExtensionsTests
{
    private static ServiceProvider BuildProvider(bool useSendGrid)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MailOptions:UseSendGrid"] = useSendGrid ? "true" : "false",
                ["MailOptions:From"] = "noreply@x.com",
                ["MailOptions:Smtp:Host"] = "localhost",
                ["MailOptions:Smtp:Port"] = "587",
                ["MailOptions:SendGrid:ApiKey"] = "sg-key",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddRecoveryMailing();
        return services.BuildServiceProvider();
    }

    #region Service selection

    [Fact]
    public void AddRecoveryMailing_Should_RegisterSmtpService_When_UseSendGridFalse()
    {
        // Arrange
        using var provider = BuildProvider(useSendGrid: false);

        // Act
        var mailService = provider.GetRequiredService<IMailService>();

        // Assert
        mailService.ShouldBeOfType<SmtpMailService>();
    }

    [Fact]
    public void AddRecoveryMailing_Should_RegisterSendGridService_When_UseSendGridTrue()
    {
        // Arrange
        using var provider = BuildProvider(useSendGrid: true);

        // Act
        var mailService = provider.GetRequiredService<IMailService>();

        // Assert
        mailService.ShouldBeOfType<SendGridMailService>();
    }

    [Fact]
    public void AddRecoveryMailing_Should_ReturnSameServiceCollection_When_Chained()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        // Act
        var result = services.AddRecoveryMailing();

        // Assert
        result.ShouldBeSameAs(services);
    }

    #endregion
}
