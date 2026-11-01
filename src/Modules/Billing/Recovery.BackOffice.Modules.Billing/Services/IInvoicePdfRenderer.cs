using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;

namespace Recovery.BackOffice.Modules.Billing.Services;

/// <summary>Renders an invoice to a self-contained PDF document (on-demand, no stored artifact).</summary>
public interface IInvoicePdfRenderer
{
    byte[] Render(InvoiceDto invoice);
}
