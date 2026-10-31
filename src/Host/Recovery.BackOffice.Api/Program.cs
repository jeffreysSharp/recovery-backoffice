using Recovery.BackOffice.Identity;

var builder = WebApplication.CreateBuilder(args);

var identityModule = new IdentityModule();
identityModule.ConfigureServices(builder);

var app = builder.Build();

identityModule.ConfigureMiddleware(app);
identityModule.MapEndpoints(app);

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program
{
}
