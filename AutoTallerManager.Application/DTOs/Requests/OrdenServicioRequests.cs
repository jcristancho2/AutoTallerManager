using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Requests
{
    /// <summary>
    /// DTO para crear una nueva orden de servicio
    /// </summary>
    public class CreateOrdenServicioRequest
    {
        [Required(ErrorMessage = "El vehículo es obligatorio")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "El mecánico es obligatorio")]
        public int MecanicoId { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        public int TipoServicioId { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        public DateTime FechaIngreso { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? DescripcionTrabajo { get; set; }

        public List<RepuestoRequeridoRequest>? RepuestosRequeridos { get; set; }
    }

    /// <summary>
    /// DTO para repuesto requerido en una orden
    /// </summary>
    public class RepuestoRequeridoRequest
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio")]
        public int RepuestoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para actualizar una orden con trabajo realizado
    /// </summary>
    public class ActualizarOrdenConTrabajoRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrdenId { get; set; }

        [StringLength(500, ErrorMessage = "La descripción del trabajo no puede exceder 500 caracteres")]
        public string? DescripcionTrabajoRealizado { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La mano de obra debe ser mayor o igual a 0")]
        public decimal ManoDeObra { get; set; }

        public List<RepuestoUtilizadoRequest>? RepuestosUtilizados { get; set; }
    }

    /// <summary>
    /// DTO para repuesto utilizado en trabajo realizado
    /// </summary>
    public class RepuestoUtilizadoRequest
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio")]
        public int RepuestoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a 0")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
        public string? Descripcion { get; set; }
    }

    /// <summary>
    /// DTO para cerrar una orden de servicio
    /// </summary>
    public class CerrarOrdenServicioRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El tipo de pago es obligatorio")]
        public int TipoPagoId { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? ObservacionesFactura { get; set; }
    }

    /// <summary>
    /// DTO para asignar repuestos a una orden
    /// </summary>
    public class AsignarRepuestosRequest
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "Los repuestos son obligatorios")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un repuesto")]
        public List<RepuestoAsignacionRequest> Repuestos { get; set; } = new();
    }

    /// <summary>
    /// DTO para asignación de repuesto
    /// </summary>
    public class RepuestoAsignacionRequest
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio")]
        public int RepuestoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
