using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record CloseOrderServiceCommand : IRequest<CloseOrderServiceResponse>
{
    public int OrderId { get; init; }
    public int PaymentTypeId { get; init; }
    public string? InvoiceNotes { get; init; }
}

public record CloseOrderServiceResponse
{
    public int OrderId { get; init; }
    public int InvoiceId { get; init; }
    public decimal TotalInvoice { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime CloseDate { get; init; }
}


