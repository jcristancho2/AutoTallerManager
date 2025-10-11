using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record DeleteClienteCommand(Guid Id) : IRequest<bool>;


