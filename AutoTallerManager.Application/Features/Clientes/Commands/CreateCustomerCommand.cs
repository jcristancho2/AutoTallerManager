using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record CreateCustomerCommand(
    string FullName,
    string Phone,
    string Email,
    int CustomerTypeId,
    int AddressId
) : IRequest<int>;


