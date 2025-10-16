using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Commands;

/// <summary>
/// Comando para crear un cliente con dirección completa
/// </summary>
public sealed record CreateFullCustomerCommand(
    string FullName,
    string Phone,
    string Email,
    int CustomerTypeId,
    string AddressDescription,
    int CountryId,
    int DepartmentId,
    int CityId
) : IRequest<CreateFullCustomerResponse>;

/// <summary>
/// Respuesta del comando de creación de cliente completo
/// </summary>
public sealed record CreateFullCustomerResponse(
    int CustomerId,
    string FullName,
    string Email,
    int AddressId,
    string DescripctionAddress,
    string CityName,
    string DepartmentName,
    string CountryName
);
