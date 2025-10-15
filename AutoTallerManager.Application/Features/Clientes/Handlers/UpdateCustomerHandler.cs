using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        // Obtener el cliente existente
        var customerExists = await _unitOfWork.Customer.GetByIdAsync(request.CustomerId, ct);
        if (customerExists == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.CustomerId} no encontrado.");
        }

        // Validar que el email no esté en uso por otro cliente
        if (!string.IsNullOrEmpty(request.Email) && request.Email != customerExists.Email)
        {
            var emailInUse = await _unitOfWork.Customer.ExistsAsync(c => c.Email == request.Email && c.Id != request.CustomerId, ct);
            if (emailInUse)
            {
                throw new InvalidOperationException("Ya existe un cliente con este correo electrónico.");
            }
        }

        // Actualizar las propiedades del cliente
        if (!string.IsNullOrEmpty(request.FullName))
            customerExists.FullName = request.FullName;

        if (!string.IsNullOrEmpty(request.Phone))
            customerExists.Phone = request.Phone;

        if (!string.IsNullOrEmpty(request.Email))
            customerExists.Email = request.Email;

        if (request.CustomerTypeId > 0)
            customerExists.CustomerTypeId = request.CustomerTypeId;

        if (request.AddressId > 0)
            customerExists.AddressId = request.AddressId;

        customerExists.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Customer.UpdateAsync(customerExists, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


