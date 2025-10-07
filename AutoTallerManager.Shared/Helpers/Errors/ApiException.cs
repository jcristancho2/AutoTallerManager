using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Shared.Helpers.Errors;

    public class ApiException
    {
       public string? Details { get; set; }
        public ApiException(int statusCode, string? message = null, string? details = null)
                    : base(statusCode, message)
    {
        Details = details;
    } 
    }
