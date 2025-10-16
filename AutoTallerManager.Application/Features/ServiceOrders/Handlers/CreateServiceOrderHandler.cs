using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.OrdenesServicio.Handlers;

public sealed class CreateServiceOrderHandler : IRequestHandler<CreateServiceOrderCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleServiceAvailabilityValidator _validatorVehiculo;
    private readonly IDateCalculatorService _calculatorDates;

    public CreateServiceOrderHandler(
        IUnitOfWork unitOfWork, 
        IVehicleServiceAvailabilityValidator validatorVehiculo,
        IDateCalculatorService calculatorDates)
    {
        _unitOfWork = unitOfWork;
        _validatorVehiculo = validatorVehiculo;
        _calculatorDates = calculatorDates;
    }

    public async Task<int> Handle(CreateServiceOrderCommand request, CancellationToken ct)
    {
        // Validar que el vehículo existe
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId, ct, "Cliente");
        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehículo con ID {request.VehicleId} no encontrado.");
        }

        // Validar que el mecánico existe
        var mechanic = await _unitOfWork.User.GetByIdAsync(request.MechanicId, ct);
        if (mechanic == null)
        {
            throw new KeyNotFoundException($"Mecánico con ID {request.MechanicId} no encontrado.");
        }

        // Validar que el tipo de servicio existe
        var serviceType = await _unitOfWork.ServiceTypes.GetByIdAsync(request.ServiceTypeId, ct);
        if (serviceType == null)
        {
            throw new KeyNotFoundException($"Tipo de servicio con ID {request.ServiceTypeId} no encontrado.");
        }

        // Validar disponibilidad del vehículo
        var vehicleAvailable = await _validatorVehiculo.IsVehicleAvailableAsync(
            request.VehicleId,
            request.EntryDate, 
            null, 
            null, 
            ct);

        if (!vehicleAvailable)
        {
            throw new InvalidOperationException("El vehículo no está disponible en la fecha especificada. Ya tiene órdenes activas.");
        }

        // Validar stock de repuestos si se especifican
        if (request.RequiredSpare != null && request.RequiredSpare.Any())
        {
            foreach (var repuestoRequerido in request.RequiredSpare)
            {
                var spare = await _unitOfWork.Spares.GetByIdAsync(repuestoRequerido.SpareId, ct);
                if (spare == null)
                {
                    throw new KeyNotFoundException($"Repuesto con ID {repuestoRequerido.SpareId} no encontrado.");
                }

                if (spare.Stock < repuestoRequerido.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el repuesto {spare.Name}. Stock disponible: {spare.Stock}, Cantidad requerida: {repuestoRequerido.Quantity}");
                }
            }
        }

        // Calcular fecha estimada de entrega
        var complexity = _calculatorDates.CalculateServiceComplexity(serviceType);
        var estimatedDeliveryDate = _calculatorDates.CalculateEstimatedDeliveryDate(serviceType, complexity);

        // Crear la orden de servicio
        var serviceOrder = new ServiceOrder
        {
            VehicleId = request.VehicleId,
            MechanicId = request.MechanicId,
            ServiceTypeId = request.ServiceTypeId,
            EntryDate = request.EntryDate,
            EstimatedDeliveryDate = estimatedDeliveryDate,
            WorkDescription = request.WorkDescription,
            StatusId = 1, // Estado "Pendiente"
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ServiceOrders.AddAsync(serviceOrder, ct);
        await _unitOfWork.SaveChangesAsync(ct); // Guardar para obtener el ID

        // Crear detalles de orden y reservar repuestos si se especifican
        if (request.RequiredSpare != null && request.RequiredSpare.Any())
        {
            foreach (var repuestoRequerido in request.RequiredSpare)
            {
                var spare = await _unitOfWork.Spares.GetByIdAsync(repuestoRequerido.SpareId, ct);

                var orderDetail = new OrderDetail
                {
                    ServiceOrderId = serviceOrder.Id,
                    SpareId = repuestoRequerido.SpareId,
                    Quantity = repuestoRequerido.Quantity,
                    UnitPrice = spare!.UnitPrice,
                    LaborCost = 0m, // Se establecerá cuando se realice el trabajo
                    Description = $"Reserva de {spare.Name}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrderDetails.AddAsync(orderDetail, ct);

                // Reservar stock (descontar del inventario)
                spare.Stock -= repuestoRequerido.Quantity;
                spare.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Spares.UpdateAsync(spare, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return serviceOrder.Id;
    }
}


