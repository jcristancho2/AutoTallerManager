namespace AutoTallerManager.Application.DTOs.Responses
{
    /// <summary>
    /// DTO de respuesta para cliente
    /// </summary>
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CustomerTypeId { get; set; }
        
        public DateTime CreatedAt { get; set; }


        public DateTime UpdatedAt { get; set; }
        public List<VehicleResponse>? Vehicles { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para vehículo
    /// </summary>
    public class VehicleResponse
    {
        public int VehicleId { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Mileage { get; set; }
        public int CustomerId { get; set; }
        public int VehicleTypeId { get; set; }
        public int VehicleBrandId { get; set; }
        public int VehiculeModelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para orden de servicio
    /// </summary>
    public class ServiceOrderResponse
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string? WorkDescription { get; set; }
        public int VehicleId { get; set; }
        public int MechanicId { get; set; }
        public int ServiceTypeId { get; set; }
        public int EstatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderDetailResponse>? OrderDetails { get; set; }
        public List<InvoiceResponse>? Invoices { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para detalle de orden
    /// </summary>
    public class OrderDetailResponse
    {
        public int DetailOrderId { get; set; }
        public int ServiceOrderId { get; set; }
        public int? SpareId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LaborCost { get; set; }
        public decimal Subtotal => Quantity * UnitPrice + LaborCost;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para repuesto
    /// </summary>
    public class SpareResponse
    {
        public int SpareId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockMin { get; set; }
        public bool IsLowStock => Stock <= StockMin;
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para factura
    /// </summary>
    public class InvoiceResponse
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public string? Observations { get; set; }
        public int ServiceOrderId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderDetailResponse>? OrderDetails { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para usuario
    /// </summary>
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int EstatusId { get; set; }
        public List<RolResponse>? Rols { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para rol
    /// </summary>
    public class RolResponse
    {
        public int RolId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para login
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserResponse User { get; set; } = new();
    }

    /// <summary>
    /// DTO de respuesta para cerrar orden
    /// </summary>
    public class CloseOrderServiceResponse
    {
        public int OrderId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime CloseDate { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para generar factura
    /// </summary>
    public class GenerateInvoiceResponse
    {
        public int InvoiceId { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotalspare { get; set; }
        public decimal SubtotalLabor { get; set; }
        public DateTime DateGeneration { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}
