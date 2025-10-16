using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Customers.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Handlers;

public sealed class RegisterCustomerWithVehicleHandler : IRequestHandler<RegisterCustomerWithVehicleCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerWithVehicleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterCustomerWithVehicleCommand request, CancellationToken ct)
    {
        // Validar que el email no exista
        var customerExists = await _unitOfWork.Customer.GetByEmailAsync(request.Email, ct);
        if (customerExists != null)
        {
            throw new InvalidOperationException("Ya existe un cliente con este correo electrónico.");
        }

        // Validar que los VINs sean únicos
        var vins = request.Vehicles.Select(v => v.VIN).ToList();
        var vinsDuplicados = vins.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (vinsDuplicados.Any())
        {
            throw new InvalidOperationException($"Los siguientes VINs están duplicados: {string.Join(", ", vinsDuplicados)}");
        }

        // Verificar que los VINs no existan en la base de datos
        foreach (var vin in vins)
        {
            var vehicleExists = await _unitOfWork.Vehicles.GetByVinAsync(vin, ct);
            if (vehicleExists != null)
            {
                throw new InvalidOperationException($"Ya existe un vehículo con el VIN: {vin}");
            }
        }

        // Crear el cliente
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
        await _unitOfWork.SaveChangesAsync(ct); // Guardar para obtener el ID del cliente

        // Crear los vehículos asociados
        foreach (var vehicleDto in request.Vehicles)
        {
            var vehicle = new Vehicle
            {
                LicensePlate = vehicleDto.LicensePlate,
                Year = vehicleDto.Year,
                VIN = vehicleDto.VIN,
                Mileage = vehicleDto.Mileage,
                CustomerId = customer.Id,
                VehicleTypeId = vehicleDto.VehicleTypeId,
                VehicleBrandId = vehicleDto.VehicleBrandId,
                VehicleModelId = vehicleDto.VehicleModelId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Vehicles.AddAsync(vehicle, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}
