using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record DeleteClienteCommand(int Id) : IRequest<bool>;


