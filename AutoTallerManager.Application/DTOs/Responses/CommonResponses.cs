namespace AutoTallerManager.Application.DTOs.Responses
{
    /// <summary>
    /// DTO de respuesta para cliente
    /// </summary>
    public class ClienteResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int TipoClienteId { get; set; }
        public string? TipoClienteNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<VehiculoResponse>? Vehiculos { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para vehículo
    /// </summary>
    public class VehiculoResponse
    {
        public int Id { get; set; }
        public string Vin { get; set; } = string.Empty;
        public int Ano { get; set; }
        public int Kilometraje { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public int TipoVehiculoId { get; set; }
        public string? TipoVehiculoNombre { get; set; }
        public int MarcaId { get; set; }
        public string? MarcaNombre { get; set; }
        public int ModeloId { get; set; }
        public string? ModeloNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para orden de servicio
    /// </summary>
    public class OrdenServicioResponse
    {
        public int Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; }
        public string? DescripcionTrabajo { get; set; }
        public int VehiculoId { get; set; }
        public string? VehiculoVin { get; set; }
        public int MecanicoId { get; set; }
        public string? MecanicoNombre { get; set; }
        public int TipoServId { get; set; }
        public string? TipoServicioNombre { get; set; }
        public int EstadoId { get; set; }
        public string? EstadoNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DetalleOrdenResponse>? DetallesOrden { get; set; }
        public List<FacturaResponse>? Facturas { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para detalle de orden
    /// </summary>
    public class DetalleOrdenResponse
    {
        public int Id { get; set; }
        public int OrdenServicioId { get; set; }
        public int? RepuestoId { get; set; }
        public string? RepuestoNombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioManoDeObra { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario + PrecioManoDeObra;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para repuesto
    /// </summary>
    public class RepuestoResponse
    {
        public int Id { get; set; }
        public string CodigoRepuesto { get; set; } = string.Empty;
        public string NombreRepu { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int StockMinimo { get; set; }
        public bool StockBajo => Stock <= StockMinimo;
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public int FabricanteId { get; set; }
        public string? FabricanteNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para factura
    /// </summary>
    public class FacturaResponse
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public int OrdenServicioId { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public int TipoPagoId { get; set; }
        public string? TipoPagoNombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DetalleOrdenResponse>? DetallesOrden { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para usuario
    /// </summary>
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EstadoId { get; set; }
        public string? EstadoNombre { get; set; }
        public List<RolResponse>? Roles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para rol
    /// </summary>
    public class RolResponse
    {
        public int Id { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para login
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UsuarioResponse Usuario { get; set; } = new();
    }

    /// <summary>
    /// DTO de respuesta para cerrar orden
    /// </summary>
    public class CerrarOrdenServicioResponse
    {
        public int OrdenId { get; set; }
        public int FacturaId { get; set; }
        public decimal TotalFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaCierre { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para generar factura
    /// </summary>
    public class GenerarFacturaResponse
    {
        public int FacturaId { get; set; }
        public decimal Total { get; set; }
        public decimal SubtotalRepuestos { get; set; }
        public decimal SubtotalManoDeObra { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
    }
}
