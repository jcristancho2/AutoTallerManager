using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Domain.Entities;
using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Handlers;

public sealed class RegistrarClienteConVehiculoHandler : IRequestHandler<RegistrarClienteConVehiculoCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarClienteConVehiculoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegistrarClienteConVehiculoCommand request, CancellationToken ct)
    {
        // Validar que el email no exista
        var clienteExistente = await _unitOfWork.Clientes.GetByEmailAsync(request.Email, ct);
        if (clienteExistente != null)
        {
            throw new InvalidOperationException("Ya existe un cliente con este correo electrónico.");
        }

        // Validar que los VINs sean únicos
        var vins = request.Vehiculos.Select(v => v.VIN).ToList();
        var vinsDuplicados = vins.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (vinsDuplicados.Any())
        {
            throw new InvalidOperationException($"Los siguientes VINs están duplicados: {string.Join(", ", vinsDuplicados)}");
        }

        // Verificar que los VINs no existan en la base de datos
        foreach (var vin in vins)
        {
            var vehiculoExistente = await _unitOfWork.Vehiculos.GetByVinAsync(vin, ct);
            if (vehiculoExistente != null)
            {
                throw new InvalidOperationException($"Ya existe un vehículo con el VIN: {vin}");
            }
        }

        // Crear el cliente
        var cliente = new Cliente
        {
            NombreCompleto = request.NombreCompleto,
            Telefono = request.Telefono,
            Email = request.Email,
            TipoCliente_Id = request.TipoCliente_Id,
            Direccion_Id = request.Direccion_Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Clientes.AddAsync(cliente, ct);
        await _unitOfWork.SaveChangesAsync(ct); // Guardar para obtener el ID del cliente

        // Crear los vehículos asociados
        foreach (var vehiculoDto in request.Vehiculos)
        {
            var vehiculo = new Vehiculo
            {
                Placa = vehiculoDto.Placa,
                Anio = vehiculoDto.Anio,
                VIN = vehiculoDto.VIN,
                Kilometraje = vehiculoDto.Kilometraje,
                ClienteId = cliente.Id,
                TipoVehiculoId = vehiculoDto.TipoVehiculoId,
                MarcaVehiculoId = vehiculoDto.MarcaVehiculoId,
                ModeloVehiculoId = vehiculoDto.ModeloVehiculoId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Vehiculos.AddAsync(vehiculo, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return cliente.Id;
    }
}
