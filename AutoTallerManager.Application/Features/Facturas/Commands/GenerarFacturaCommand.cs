using MediatR;

namespace AutoTallerManager.Application.Features.Facturas.Commands;

public record GenerarFacturaCommand : IRequest<GenerarFacturaResponse>
{
    public int OrdenServicioId { get; init; }
    public int TipoPagoId { get; init; }
    public string? Observaciones { get; init; }
}

public record GenerarFacturaResponse
{
    public int FacturaId { get; init; }
    public decimal Total { get; init; }
    public decimal SubtotalRepuestos { get; init; }
    public decimal SubtotalManoDeObra { get; init; }
    public DateTime FechaGeneracion { get; init; }
    public string NumeroFactura { get; init; } = string.Empty;
}


