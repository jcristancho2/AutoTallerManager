using MediatR;

namespace AutoTallerManager.Application.Features.Customers.Commands;

public record RegisterCustomerWithVehicleCommand : IRequest<int>
{
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int CustomerTypeId { get; init; }
    public int AddressId { get; init; }
    public List<VehicleDto> Vehicles { get; init; } = new();
}

public record VehicleDto
{
    public string LicensePlate { get; init; } = string.Empty;
    public int Year { get; init; }
    public string VIN { get; init; } = string.Empty;
    public int Mileage { get; init; }
    public int VehicleTypeId { get; init; }
    public int VehicleBrandId { get; init; }
    public int VehicleModelId { get; init; }
}
