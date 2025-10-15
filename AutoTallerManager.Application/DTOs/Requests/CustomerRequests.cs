using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Requests
{
    /// <summary>
    /// DTO para crear un nuevo cliente
    /// </summary>
    public class CreateCustomerRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FullName { get; set; } = string.Empty;

        

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Phone{ get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El tipo de cliente es obligatorio")]
        public int CustomerTypeId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un cliente existente
    /// </summary>
    public class UpdateCustomerRequest
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FullName { get; set; } = string.Empty;


        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El tipo de cliente es obligatorio")]
        public int CustomerTypeId { get; set; }
    }

    /// <summary>
    /// DTO para registrar cliente con vehículos
    /// </summary>
    public class RegisterCustomerWithVehicleRequest
    {
        [Required(ErrorMessage = "Los datos del cliente son obligatorios")]
        public CreateCustomerRequest Customer { get; set; } = new();

        [Required(ErrorMessage = "Al menos un vehículo es obligatorio")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un vehículo")]
        public List<CreateVehiculoRequest> Vehicles { get; set; } = new();
    }

    /// <summary>
    /// DTO para crear un nuevo vehículo
    /// </summary>
    public class CreateVehiculoRequest
    {
        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "La placa debe tener exactamente 17 caracteres")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año debe estar entre 1900 y 2030")]
        public int Year { get; set; }

        [Required(ErrorMessage = "El kilometraje es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El kilometraje debe ser mayor o igual a 0")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "El tipo de vehículo es obligatorio")]
        public int VehicleTypeId { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio")]
        public int ModeloId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un vehículo existente
    /// </summary>
    public class UpdateVehiculoRequest
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "La placa debe tener exactamente 17 caracteres")]
        public string LicensePlate { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año debe estar entre 1900 y 2030")]
        public int Year { get; set; }

        [Required(ErrorMessage = "El kilometraje es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El kilometraje debe ser mayor o igual a 0")]
        public int Mileage{ get; set; }

        [Required(ErrorMessage = "El tipo de vehículo es obligatorio")]
        public int VehicleTypeId { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        public int VehicleBrandId { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio")]
        public int VehicleModelId { get; set; }
    }
}
