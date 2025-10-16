using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public record UpdateOrderWithWorkDoneCommand : IRequest<bool>
{
    public int OrderId { get; init; }
    public string WorkDescription { get; init; } = string.Empty;
    public List<SpareUsedDto> UsedSpare { get; init; } = new();
    public decimal LaborCost { get; init; }
    public int? NewStatusId { get; init; }
}

public record SpareUsedDto
{
    public int SpareId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string Description { get; init; } = string.Empty;
}
