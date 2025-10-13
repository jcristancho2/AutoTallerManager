using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class UpdateClienteHandler : IRequestHandler<UpdateClienteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateClienteCommand request, CancellationToken ct)
    {
        // Obtener el cliente existente
        var clienteExistente = await _unitOfWork.Clientes.GetByIdAsync(request.Id, ct);
        if (clienteExistente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
        }

        // Validar que el email no esté en uso por otro cliente
        if (!string.IsNullOrEmpty(request.Correo) && request.Correo != clienteExistente.Email)
        {
            var emailEnUso = await _unitOfWork.Clientes.ExistsAsync(c => c.Email == request.Correo && c.Id != request.Id, ct);
            if (emailEnUso)
            {
                throw new InvalidOperationException("Ya existe un cliente con este correo electrónico.");
            }
        }

        // Actualizar las propiedades del cliente
        if (!string.IsNullOrEmpty(request.NombreCompleto))
            clienteExistente.NombreCompleto = request.NombreCompleto;
        
        if (!string.IsNullOrEmpty(request.Telefono))
            clienteExistente.Telefono = request.Telefono;
        
        if (!string.IsNullOrEmpty(request.Correo))
            clienteExistente.Email = request.Correo;
        
        if (request.TipoCliente_Id > 0)
            clienteExistente.TipoCliente_Id = request.TipoCliente_Id;
        
        if (request.Direccion_Id > 0)
            clienteExistente.Direccion_Id = request.Direccion_Id;

        clienteExistente.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Clientes.UpdateAsync(clienteExistente, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


