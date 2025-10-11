using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.API.Helpers.Errors;

namespace AutoTallerManager.API.Helpers.Errors;

public class ApiValidation : ApiResponse
{
        public ApiValidation() : base(400)
    {
    }
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
}
