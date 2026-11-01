namespace Recovery.BackOffice.BuildingBlocks.Mailing.Services;

public interface IMailService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}