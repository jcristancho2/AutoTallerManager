using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class DeleteClienteHandler : IRequestHandler<DeleteClienteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClienteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteClienteCommand request, CancellationToken ct)
    {
        // Obtener el cliente existente
        var clienteExistente = await _unitOfWork.Clientes.GetByIdAsync(request.Id, ct, new[] { "Vehiculos" });
        if (clienteExistente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
        }

        // Verificar si el cliente tiene vehículos con órdenes de servicio activas
        if (clienteExistente.Vehiculos != null && clienteExistente.Vehiculos.Any())
        {
            foreach (var vehiculo in clienteExistente.Vehiculos)
            {
                var ordenesActivas = await _unitOfWork.OrdenesServicio.GetAllAsync(
                    filter: o => o.VehiculoId == vehiculo.Id && o.EstadoId != 3 && o.EstadoId != 4, // No completada ni cancelada
                    ct: ct);

                if (ordenesActivas.Any())
                {
                    throw new InvalidOperationException($"No se puede eliminar el cliente porque tiene vehículos con órdenes de servicio activas.");
                }
            }
        }

        // Eliminar el cliente
        await _unitOfWork.Clientes.DeleteAsync(request.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}


