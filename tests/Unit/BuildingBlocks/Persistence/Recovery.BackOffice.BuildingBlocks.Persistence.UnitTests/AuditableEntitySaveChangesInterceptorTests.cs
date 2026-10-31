using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.BuildingBlocks.Persistence.Interceptors;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class AuditableEntitySaveChangesInterceptorTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
    private static readonly DateTimeOffset Created = Now.AddDays(-1);

    [Fact]
    public async Task AddRecoveryDbContext_Should_ApplyAudit_When_ContextIsResolvedThroughContainer()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=AuditTests;Integrated Security=True",
            ["DatabaseOptions:MigrationsAssembly"] = typeof(AuditContext).Assembly.GetName().Name
        });
        var userId = Guid.NewGuid();
        builder.Services.AddSingleton<ICurrentUser>(new TestCurrentUser(userId, true));
        builder.Services.AddSingleton<TimeProvider>(new FixedTimeProvider());
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddScoped<ISaveChangesInterceptor, SuppressDatabaseSaveInterceptor>();
        builder.Services.AddRecoveryDbContext<AuditContext>();
        using var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        var entity = new AuditEntity { Id = 1 };
        context.Add(entity);

        // Act
        await context.SaveChangesAsync(acceptAllChangesOnSuccess: false);

        // Assert
        Assert.Equal(Now, entity.CreatedOnUtc);
        Assert.Equal(userId.ToString(), entity.CreatedBy);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task SavingChanges_Should_SetCreationAudit_When_EntityIsAdded(bool asynchronous, bool authenticated)
    {
        // Arrange
        var userId = Guid.NewGuid();
        using var context = CreateContext(userId, authenticated);
        var entity = new AuditEntity { Id = 1 };
        context.Add(entity);

        // Act
        await SaveWithoutDatabase(context, asynchronous);

        // Assert
        Assert.Equal(Now, entity.CreatedOnUtc);
        Assert.Equal(authenticated ? userId.ToString() : null, entity.CreatedBy);
        Assert.Null(entity.LastModifiedOnUtc);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavingChanges_Should_SetModificationAuditAndPreserveCreation_When_EntityIsModified(bool asynchronous)
    {
        var userId = Guid.NewGuid();
        using var context = CreateContext(userId, true);
        var entity = ExistingEntity();
        context.Attach(entity);
        entity.Name = "Changed";

        await SaveWithoutDatabase(context, asynchronous);

        Assert.Equal(Now, entity.LastModifiedOnUtc);
        Assert.Equal(userId.ToString(), entity.LastModifiedBy);
        Assert.Equal(Created, entity.CreatedOnUtc);
        Assert.Equal("creator", entity.CreatedBy);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavingChanges_Should_AuditOwner_When_OnlyOwnedReferenceChanges(bool asynchronous)
    {
        var userId = Guid.NewGuid();
        using var context = CreateContext(userId, true);
        var entity = ExistingEntity();
        context.Attach(entity);
        entity.Detail.Description = "Changed owned value";

        await SaveWithoutDatabase(context, asynchronous);

        Assert.Equal(Now, entity.LastModifiedOnUtc);
        Assert.Equal(userId.ToString(), entity.LastModifiedBy);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task SavingChanges_Should_SoftDeleteAndPreserveOwnedReference_When_EntityIsDeleted(
        bool asynchronous, bool authenticated)
    {
        var userId = Guid.NewGuid();
        using var context = CreateContext(userId, authenticated);
        var entity = ExistingEntity();
        context.Attach(entity);
        context.Remove(entity);

        await SaveWithoutDatabase(context, asynchronous);

        Assert.Equal(EntityState.Modified, context.Entry(entity).State);
        Assert.True(entity.IsDeleted);
        Assert.Equal(Now, entity.DeletedOnUtc);
        Assert.Equal(authenticated ? userId.ToString() : null, entity.DeletedBy);
        Assert.Equal(EntityState.Unchanged, context.Entry(entity.Detail).State);
        Assert.Equal("Original owned value", entity.Detail.Description);
        Assert.Equal(Created, entity.CreatedOnUtc);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavingChanges_Should_LeaveAuditUntouched_When_EntityIsUnchanged(bool asynchronous)
    {
        using var context = CreateContext(Guid.NewGuid(), true);
        var entity = ExistingEntity();
        context.Attach(entity);

        await SaveWithoutDatabase(context, asynchronous);

        Assert.Equal(Created, entity.CreatedOnUtc);
        Assert.Equal("creator", entity.CreatedBy);
        Assert.Null(entity.LastModifiedOnUtc);
        Assert.False(entity.IsDeleted);
        Assert.Equal(EntityState.Unchanged, context.Entry(entity).State);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavingChanges_Should_KeepPhysicalDeletion_When_EntityHasNoSoftDeleteContract(bool asynchronous)
    {
        using var context = CreateContext(Guid.NewGuid(), true);
        var entity = new PlainEntity { Id = 1 };
        context.Attach(entity);
        context.Remove(entity);

        await SaveWithoutDatabase(context, asynchronous);

        Assert.Equal(EntityState.Deleted, context.Entry(entity).State);
    }

    private static AuditEntity ExistingEntity() => new()
    {
        Id = 1,
        Name = "Original",
        CreatedOnUtc = Created,
        CreatedBy = "creator"
    };

    private static AuditContext CreateContext(Guid userId, bool authenticated)
    {
        var audit = new AuditableEntitySaveChangesInterceptor(
            new TestCurrentUser(userId, authenticated), new FixedTimeProvider());
        var options = new DbContextOptionsBuilder<AuditContext>()
            .UseSqlServer("Server=localhost;Database=AuditTests;Integrated Security=True")
            .AddInterceptors(audit, new SuppressDatabaseSaveInterceptor())
            .Options;
        return new AuditContext(options);
    }

    private static async Task SaveWithoutDatabase(DbContext context, bool asynchronous)
    {
        if (asynchronous)
        {
            await context.SaveChangesAsync(acceptAllChangesOnSuccess: false);
        }
        else
        {
            context.SaveChanges(acceptAllChangesOnSuccess: false);
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class SuppressDatabaseSaveInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result) =>
            InterceptionResult<int>.SuppressWithResult(0);

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(0));
    }

    private sealed class AuditContext(DbContextOptions<AuditContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEntity>().OwnsOne(entity => entity.Detail);
            modelBuilder.Entity<PlainEntity>();
        }
    }

    private sealed class AuditEntity : IAuditableEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedOnUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOnUtc { get; set; }
        public string? DeletedBy { get; set; }
        public OwnedDetail Detail { get; set; } = new();
    }

    private sealed class OwnedDetail
    {
        public string Description { get; set; } = "Original owned value";
    }

    private sealed class PlainEntity
    {
        public int Id { get; set; }
    }
}
