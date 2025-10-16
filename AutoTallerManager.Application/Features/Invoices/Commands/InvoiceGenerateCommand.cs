using MediatR;

namespace AutoTallerManager.Application.Features.Facturas.Commands;

public record InvoiceGenerateCommand : IRequest<InvoiceGenerateResponse>
{
    public int ServiceOrderId { get; init; }
    public int PaymentTypeId { get; init; }
    public string? Observations { get; init; }
}

public record InvoiceGenerateResponse
{
    public int InvoiceId { get; init; }
    public decimal Total { get; init; }

    public DateTime InvoiceDate { get; init; }
    public decimal SubtotalSpare { get; init; }
    public decimal SubtotalLabor { get; init; }
    public DateTime GenerationDate { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
}


