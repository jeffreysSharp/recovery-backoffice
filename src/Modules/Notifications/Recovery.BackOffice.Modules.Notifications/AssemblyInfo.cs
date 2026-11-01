using System.Runtime.CompilerServices;
using Recovery.BackOffice.BuildingBlocks.Web.Modules;

[assembly: RecoveryModule(typeof(Recovery.BackOffice.Modules.Notifications.NotificationsModule), 750)]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Notifications.Tests")]
[assembly: InternalsVisibleTo("Recovery.BackOffice.Integration.Tests")]
