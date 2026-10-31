using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using Recovery.BackOffice.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddModules(typeof(IdentityModule).Assembly);

var app = builder.Build();

app.UseModuleMiddlewares();
app.MapModules();

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program
{
}
