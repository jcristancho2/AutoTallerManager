using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record CerrarOrdenServicioCommand : IRequest<CerrarOrdenServicioResponse>
{
    public int OrdenId { get; init; }
    public int TipoPagoId { get; init; }
    public string? ObservacionesFactura { get; init; }
}

public record CerrarOrdenServicioResponse
{
    public int OrdenId { get; init; }
    public int FacturaId { get; init; }
    public decimal TotalFactura { get; init; }
    public string NumeroFactura { get; init; } = string.Empty;
    public DateTime FechaCierre { get; init; }
}


