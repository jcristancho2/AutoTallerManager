using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Commands;

public sealed record DeleteCustomerCommand(int Id) : IRequest<bool>;


