using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class CreateClienteHandler : IRequestHandler<CreateClienteCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateClienteCommand request, CancellationToken ct)
    {
        // Uniqueness validation by mail
        var exists = await _unitOfWork.Clientes.ExistsAsync(c => c.Email == request.Correo, ct);
        if (exists)
        {
            throw new InvalidOperationException("Ya existe un cliente con este correo.");
        }

        var cliente = new Cliente
        {
            NombreCompleto = request.NombreCompleto,
            Telefono = request.Telefono,
            Email = request.Correo,
            TipoCliente_Id = request.TipoCliente_Id,
            Direccion_Id = request.Direccion_Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Clientes.AddAsync(cliente, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return cliente.Id;
    }
}


