using System.Runtime.CompilerServices;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Modules.Chat.ChatModule), 800)]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Chat.Tests")]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Integration.Tests")]
