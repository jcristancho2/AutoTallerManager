using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public sealed record UpdateClienteCommand(
    int Id,
    string NombreCompleto,
    string Telefono,
    string Correo,
    int TipoCliente_Id,
    int Direccion_Id
) : IRequest<bool>;


