namespace Recovery.BackOffice.Integration.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class RecoveryCollectionDefinition : ICollectionFixture<RecoveryWebApplicationFactory>
{
    public const string Name = "RecoveryIntegration";
}
