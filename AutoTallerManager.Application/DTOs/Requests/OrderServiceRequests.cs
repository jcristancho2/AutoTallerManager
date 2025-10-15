using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Requests
{
    /// <summary>
    /// DTO para crear una nueva orden de servicio
    /// </summary>
    public class CreateServiceOrderRequest
    {
        [Required(ErrorMessage = "El vehículo es obligatorio")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "El mecánico es obligatorio")]
        public int MechanicId { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        public DateTime EntryDate { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? WorkDescription { get; set; }

        public List<SpareRequiredRequest>? RepuestosRequeridos { get; set; }
    }

    /// <summary>
    /// DTO para repuesto requerido en una orden
    /// </summary>
    public class SpareRequiredRequest
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio")]
        public int SpareId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para actualizar una orden con trabajo realizado
    /// </summary>
    public class UpdateOrderWithWorkRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrderId { get; set; }

        [StringLength(500, ErrorMessage = "La descripción del trabajo no puede exceder 500 caracteres")]
        public string? WorkDescription { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La mano de obra debe ser mayor o igual a 0")]
        public decimal LaborCost { get; set; }

        public List<SpareUsedRequest>? SpareUseds{ get; set; }
    }

    /// <summary>
    /// DTO para repuesto utilizado en trabajo realizado
    /// </summary>
    public class SpareUsedRequest
    {
        [Required(ErrorMessage = "El Codigo del repuesto es obligatorio")]
        public int Code { get; set; }

        [Required(ErrorMessage = "El nombre del repuesto es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a 0")]
        public decimal UnitPrice { get; set; }

        // [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        // public string? Description { get; set; }
    }

    /// <summary>
    /// DTO para cerrar una orden de servicio
    /// </summary>
    public class CloseOrderServiceRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "El tipo de pago es obligatorio")]
        public int PaymentTypeId { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? InvoiceNotes { get; set; }
    }

    /// <summary>
    /// DTO para asignar repuestos a una orden
    /// </summary>
    public class AssignPartsRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Los repuestos son obligatorios")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un repuesto")]
        public List<SpareAssignmentRequest> Spares { get; set; } = new();
    }

    /// <summary>
    /// DTO para asignación de repuesto
    /// </summary>
    public class SpareAssignmentRequest
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio")]
        public int SpareId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
