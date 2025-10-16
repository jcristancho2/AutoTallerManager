using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        // Uniqueness validation by mail
        var exists = await _unitOfWork.Customer.ExistsAsync(c => c.Email == request.Email, ct);
        if (exists)
        {
            throw new InvalidOperationException("Ya existe un cliente con este correo.");
        }

        var customer = new Customer
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            CustomerTypeId = request.CustomerTypeId,
            AddressId = request.AddressId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customer.AddAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}


