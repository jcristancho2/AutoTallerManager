using MediatR;
using System.Collections.Generic;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Commands;

public sealed record RepuestoItem(int RepuestoId, int Cantidad);

public sealed record AsignarRepuestosCommand(int OrdenId, IEnumerable<RepuestoItem> Repuestos) : IRequest<Unit>;
