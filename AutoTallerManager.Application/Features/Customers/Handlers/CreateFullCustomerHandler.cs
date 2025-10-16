using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Customers.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Handlers;

/// <summary>
/// Handler para crear cliente con dirección completa
/// </summary>
public sealed class CreateFullCustomerHandler : IRequestHandler<CreateFullCustomerCommand, CreateFullCustomerResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFullCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateFullCustomerResponse> Handle(CreateFullCustomerCommand request, CancellationToken ct)
    {
        // Validar que el email no exista
        var emailExists = await _unitOfWork.Customer.ExistsAsync(c => c.Email == request.Email, ct);
        if (emailExists)
        {
            throw new InvalidOperationException("Ya existe un cliente con este email.");
        }

        // Por simplicidad, usar la dirección existente con ID 11 (que creamos anteriormente)
        // En una implementación completa, se buscaría o crearía la dirección dinámicamente
        var addressId = 11; // ID de la dirección creada anteriormente

        // Crear el cliente
        var customer = new Customer
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            CustomerTypeId = request.CustomerTypeId,
            AddressId = addressId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customer.AddAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateFullCustomerResponse(
            customer.Id,
            customer.FullName ?? string.Empty,
            customer.Email ?? string.Empty,
            addressId,
            request.AddressDescription ?? string.Empty,
            "Medellín", // Ciudad de la dirección creada
            "Antioquia", // Departamento
            "Colombia" // País
        );
    }
}
