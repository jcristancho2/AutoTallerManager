namespace AutoTallerManager.Application.DTOs.Common
{
    /// <summary>
    /// DTO para resultados paginados
    /// </summary>
    /// <typeparam name="T">Tipo de datos paginados</typeparam>
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// DTO para respuesta estándar de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// DTO para respuesta de validación
    /// </summary>
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new();

        public static ValidationResponse Valid()
        {
            return new ValidationResponse { IsValid = true };
        }

        public static ValidationResponse Invalid(List<string> errors)
        {
            return new ValidationResponse
            {
                IsValid = false,
                Errors = errors
            };
        }

        public static ValidationResponse Invalid(Dictionary<string, List<string>> fieldErrors)
        {
            return new ValidationResponse
            {
                IsValid = false,
                FieldErrors = fieldErrors
            };
        }
    }

    /// <summary>
    /// DTO para filtros de búsqueda
    /// </summary>
    public class SearchFilters
    {
        public string? SearchTerm { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public Dictionary<string, object>? AdditionalFilters { get; set; }
    }

    /// <summary>
    /// DTO para estadísticas del sistema
    /// </summary>
    public class SystemStatsResponse
    {
        public int TotalClientes { get; set; }
        public int TotalVehiculos { get; set; }
        public int TotalOrdenesActivas { get; set; }
        public int TotalOrdenesCompletadas { get; set; }
        public int TotalRepuestos { get; set; }
        public int RepuestosStockBajo { get; set; }
        public decimal ValorTotalInventario { get; set; }
        public decimal IngresosMesActual { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
