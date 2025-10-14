using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record CrearOrdenServicioCommand : IRequest<int>
{
    public int VehiculoId { get; init; }
    public int MecanicoId { get; init; }
    public int TipoServicioId { get; init; }
    public DateTime FechaIngreso { get; init; }
    public string? DescripcionTrabajo { get; init; }
    public List<RepuestoRequeridoDto>? RepuestosRequeridos { get; init; }
}

public record RepuestoRequeridoDto
{
    public int RepuestoId { get; init; }
    public int Cantidad { get; init; }
}


