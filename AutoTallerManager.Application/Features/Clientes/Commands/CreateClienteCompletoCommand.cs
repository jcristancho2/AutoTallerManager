using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

/// <summary>
/// Comando para crear un cliente con dirección completa
/// </summary>
public sealed record CreateClienteCompletoCommand(
    string NombreCompleto,
    string Telefono,
    string Email,
    int TipoClienteId,
    string DireccionDescripcion,
    int PaisId,
    int DepartamentoId,
    int CiudadId
) : IRequest<CreateClienteCompletoResponse>;

/// <summary>
/// Respuesta del comando de creación de cliente completo
/// </summary>
public sealed record CreateClienteCompletoResponse(
    int ClienteId,
    string NombreCompleto,
    string Email,
    int DireccionId,
    string DireccionDescripcion,
    string CiudadNombre,
    string DepartamentoNombre,
    string PaisNombre
);
