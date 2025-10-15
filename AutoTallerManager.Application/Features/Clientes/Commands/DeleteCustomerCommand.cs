using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record DeleteCustomerCommand(int Id) : IRequest<bool>;


