using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record ActualizarOrdenConTrabajoRealizadoCommand : IRequest<bool>
{
    public int OrdenId { get; init; }
    public string DescripcionTrabajo { get; init; } = string.Empty;
    public List<RepuestoUtilizadoDto> RepuestosUtilizados { get; init; } = new();
    public decimal ManoDeObra { get; init; }
    public int? NuevoEstadoId { get; init; }
}

public record RepuestoUtilizadoDto
{
    public int RepuestoId { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public string Descripcion { get; init; } = string.Empty;
}
