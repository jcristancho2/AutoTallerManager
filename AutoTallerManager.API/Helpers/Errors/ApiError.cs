namespace AutoTallerManager.API.Helpers.Errors;

public class ApiError
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }

    public ApiError()
    {
    }

    public ApiError(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}
