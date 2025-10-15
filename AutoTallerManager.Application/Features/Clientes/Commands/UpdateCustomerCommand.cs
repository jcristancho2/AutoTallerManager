using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record UpdateCustomerCommand(
    int CustomerId,
    string FullName,
    string Phone,
    string Email,
    int CustomerTypeId,
    int AddressId
) : IRequest<bool>;


