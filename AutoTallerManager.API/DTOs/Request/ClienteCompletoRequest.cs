using System.ComponentModel.DataAnnotations;
using AutoTallerManager.Application.DTOs.Requests;

namespace AutoTallerManager.API.DTOs.Request;

/// <summary>
/// DTO para crear un cliente con dirección completa
/// </summary>
public class CreateClienteCompletoDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de cliente es obligatorio")]
    public int TipoClienteId { get; set; }

    [Required(ErrorMessage = "Los datos de dirección son obligatorios")]
    public DireccionCompletaDto Direccion { get; set; } = new();
}

/// <summary>
/// DTO para dirección completa con ubicación geográfica
/// </summary>
public class DireccionCompletaDto
{
    [Required(ErrorMessage = "La descripción de la dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El país es obligatorio")]
    public int PaisId { get; set; }

    [Required(ErrorMessage = "El departamento es obligatorio")]
    public int DepartamentoId { get; set; }

    [Required(ErrorMessage = "La ciudad es obligatoria")]
    public int CiudadId { get; set; }
}

/// <summary>
/// DTO para registrar cliente con vehículos y dirección completa
/// </summary>
public class RegistrarClienteCompletoConVehiculoDto
{
    [Required(ErrorMessage = "Los datos del cliente son obligatorios")]
    public CreateClienteCompletoDto Cliente { get; set; } = new();

    public List<CreateVehiculoRequest> Vehiculos { get; set; } = new();
}
