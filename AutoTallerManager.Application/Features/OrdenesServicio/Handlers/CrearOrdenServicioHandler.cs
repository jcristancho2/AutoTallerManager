using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class CrearOrdenServicioHandler : IRequestHandler<CrearOrdenServicioCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidadorDisponibilidadVehiculoService _validadorVehiculo;
    private readonly ICalculadoraFechasService _calculadoraFechas;

    public CrearOrdenServicioHandler(
        IUnitOfWork unitOfWork, 
        IValidadorDisponibilidadVehiculoService validadorVehiculo,
        ICalculadoraFechasService calculadoraFechas)
    {
        _unitOfWork = unitOfWork;
        _validadorVehiculo = validadorVehiculo;
        _calculadoraFechas = calculadoraFechas;
    }

    public async Task<int> Handle(CrearOrdenServicioCommand request, CancellationToken ct)
    {
        // Validar que el vehículo existe
        var vehiculo = await _unitOfWork.Vehiculos.GetByIdAsync(request.VehiculoId, ct, "Cliente");
        if (vehiculo == null)
        {
            throw new KeyNotFoundException($"Vehículo con ID {request.VehiculoId} no encontrado.");
        }

        // Validar que el mecánico existe
        var mecanico = await _unitOfWork.Usuarios.GetByIdAsync(request.MecanicoId, ct);
        if (mecanico == null)
        {
            throw new KeyNotFoundException($"Mecánico con ID {request.MecanicoId} no encontrado.");
        }

        // Validar que el tipo de servicio existe
        var tipoServicio = await _unitOfWork.TiposServicio.GetByIdAsync(request.TipoServicioId, ct);
        if (tipoServicio == null)
        {
            throw new KeyNotFoundException($"Tipo de servicio con ID {request.TipoServicioId} no encontrado.");
        }

        // Validar disponibilidad del vehículo
        var vehiculoDisponible = await _validadorVehiculo.VehiculoDisponibleAsync(
            request.VehiculoId, 
            request.FechaIngreso, 
            null, 
            null, 
            ct);

        if (!vehiculoDisponible)
        {
            throw new InvalidOperationException("El vehículo no está disponible en la fecha especificada. Ya tiene órdenes activas.");
        }

        // Validar stock de repuestos si se especifican
        if (request.RepuestosRequeridos != null && request.RepuestosRequeridos.Any())
        {
            foreach (var repuestoRequerido in request.RepuestosRequeridos)
            {
                var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(repuestoRequerido.RepuestoId, ct);
                if (repuesto == null)
                {
                    throw new KeyNotFoundException($"Repuesto con ID {repuestoRequerido.RepuestoId} no encontrado.");
                }

                if (repuesto.Stock < repuestoRequerido.Cantidad)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el repuesto {repuesto.NombreRepu}. Stock disponible: {repuesto.Stock}, Cantidad requerida: {repuestoRequerido.Cantidad}");
                }
            }
        }

        // Calcular fecha estimada de entrega
        var complejidad = _calculadoraFechas.CalcularComplejidadServicio(tipoServicio);
        var fechaEstimadaEntrega = _calculadoraFechas.CalcularFechaEstimadaEntrega(tipoServicio, complejidad);

        // Crear la orden de servicio
        var ordenServicio = new OrdenServicio
        {
            VehiculoId = request.VehiculoId,
            MecanicoId = request.MecanicoId,
            TipoServId = request.TipoServicioId,
            FechaIngreso = request.FechaIngreso,
            FechaEstimadaEntrega = fechaEstimadaEntrega,
            DescripcionTrabajo = request.DescripcionTrabajo,
            EstadoId = 1, // Estado "Pendiente"
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.OrdenesServicio.AddAsync(ordenServicio, ct);
        await _unitOfWork.SaveChangesAsync(ct); // Guardar para obtener el ID

        // Crear detalles de orden y reservar repuestos si se especifican
        if (request.RepuestosRequeridos != null && request.RepuestosRequeridos.Any())
        {
            foreach (var repuestoRequerido in request.RepuestosRequeridos)
            {
                var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(repuestoRequerido.RepuestoId, ct);
                
                var detalleOrden = new DetalleOrden
                {
                    OrdenServicioId = ordenServicio.Id,
                    RepuestoId = repuestoRequerido.RepuestoId,
                    Cantidad = repuestoRequerido.Cantidad,
                    PrecioUnitario = repuesto!.PrecioUnitario,
                    PrecioManoDeObra = 0m, // Se establecerá cuando se realice el trabajo
                    Descripcion = $"Reserva de {repuesto.NombreRepu}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.DetallesOrden.AddAsync(detalleOrden, ct);

                // Reservar stock (descontar del inventario)
                repuesto.Stock -= repuestoRequerido.Cantidad;
                repuesto.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repuestos.UpdateAsync(repuesto, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return ordenServicio.Id;
    }
}


