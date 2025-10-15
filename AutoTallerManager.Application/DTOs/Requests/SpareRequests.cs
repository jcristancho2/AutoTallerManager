using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Requests
{
    /// <summary>
    /// DTO para crear un nuevo repuesto
    /// </summary>
    public class CreateSpareRequest
    {
        [Required(ErrorMessage = "El código del repuesto es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del repuesto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripction { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
        public int StockMin { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El fabricante es obligatorio")]
        public int ManufacturerId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un repuesto existente
    /// </summary>
    public class UpdateSpareRequest
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int SpareId { get; set; }

        [Required(ErrorMessage = "El código del repuesto es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
        public string Code{ get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del repuesto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripction { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
        public int StockMin { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El fabricante es obligatorio")]
        public int ManufacturerId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar stock de repuesto
    /// </summary>
    public class UpdateStockSpareRequest
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int SpareId { get; set; }

        [Required(ErrorMessage = "El nuevo stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }
    }
}
