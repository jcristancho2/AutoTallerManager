using MediatR;
using System.Collections.Generic;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public sealed record SpareItem(int SpareId, int Quantity);

public sealed record AssignSpareCommand(int OrderId, IEnumerable<SpareItem> Spare) : IRequest<Unit>;
