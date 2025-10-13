using MediatR;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

public record RegistrarClienteConVehiculoCommand : IRequest<int>
{
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int TipoCliente_Id { get; init; }
    public int Direccion_Id { get; init; }
    public List<VehiculoDto> Vehiculos { get; init; } = new();
}

public record VehiculoDto
{
    public string Placa { get; init; } = string.Empty;
    public int Anio { get; init; }
    public string VIN { get; init; } = string.Empty;
    public int Kilometraje { get; init; }
    public int TipoVehiculoId { get; init; }
    public int MarcaVehiculoId { get; init; }
    public int ModeloVehiculoId { get; init; }
}
