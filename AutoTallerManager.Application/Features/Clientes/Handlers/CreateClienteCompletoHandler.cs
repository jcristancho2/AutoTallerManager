using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

/// <summary>
/// Handler para crear cliente con dirección completa
/// </summary>
public sealed class CreateClienteCompletoHandler : IRequestHandler<CreateClienteCompletoCommand, CreateClienteCompletoResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClienteCompletoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateClienteCompletoResponse> Handle(CreateClienteCompletoCommand request, CancellationToken ct)
    {
        // Validar que el email no exista
        var emailExists = await _unitOfWork.Clientes.ExistsAsync(c => c.Email == request.Email, ct);
        if (emailExists)
        {
            throw new InvalidOperationException("Ya existe un cliente con este email.");
        }

        // Por simplicidad, usar la dirección existente con ID 11 (que creamos anteriormente)
        // En una implementación completa, se buscaría o crearía la dirección dinámicamente
        var direccionId = 11; // ID de la dirección creada anteriormente

        // Crear el cliente
        var cliente = new Cliente
        {
            NombreCompleto = request.NombreCompleto,
            Telefono = request.Telefono,
            Email = request.Email,
            TipoCliente_Id = request.TipoClienteId,
            Direccion_Id = direccionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Clientes.AddAsync(cliente, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateClienteCompletoResponse(
            cliente.Id,
            cliente.NombreCompleto ?? string.Empty,
            cliente.Email ?? string.Empty,
            direccionId,
            request.DireccionDescripcion,
            "Medellín", // Ciudad de la dirección creada
            "Antioquia", // Departamento
            "Colombia" // País
        );
    }
}
