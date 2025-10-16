using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record CreateServiceOrderCommand : IRequest<int>
{
    public int VehicleId { get; init; }
    public int MechanicId { get; init; }
    public int ServiceTypeId { get; init; }
    public DateTime EntryDate { get; init; }
    public string? WorkDescription { get; init; }
    public List<SpareRequiredDto>? RequiredSpare { get; init; }
}

public record SpareRequiredDto
{
    public int SpareId { get; init; }
    public int Quantity { get; init; }
}


