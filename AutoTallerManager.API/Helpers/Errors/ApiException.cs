using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.API.Helpers.Errors;



namespace AutoTallerManager.API.Helpers.Errors;

    public class ApiException : ApiResponse
{
    public string? Details { get; set; }
    public ApiException(int statusCode, string? message = null, string? details = null)
                    : base(statusCode, message)
    {
        Details = details;
    }

}

