using Recovery.BackOffice.BuildingBlocks.Web.Modules;
using System.Runtime.CompilerServices;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Modules.Identity.IdentityModule), 100)]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Identity.Tests")]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Integration.Tests")]