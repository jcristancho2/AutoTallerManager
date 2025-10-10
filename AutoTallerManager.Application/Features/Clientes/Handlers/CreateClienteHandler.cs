using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class CreateClienteHandler : IRequestHandler<CreateClienteCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateClienteCommand request, CancellationToken ct)
    {
        // Uniqueness validation by mail
        var exists = await _unitOfWork.Clientes.ExistsAsync(c => c.Correo == request.Correo, ct);
        if (exists)
        {
            throw new InvalidOperationException("Ya existe un cliente con este correo.");
        }

        var cliente = new Cliente(
            nombreCompleto: request.NombreCompleto,
            telefono: request.Telefono,
            correo: request.Correo,
            tipoCliente_Id: request.TipoCliente_Id,
            direccion_Id: request.Direccion_Id
        );

        await _unitOfWork.Clientes.AddAsync(cliente, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return cliente.Id;
    }
}


