using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.Helpers
{
    public class JWT
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public double DurationInMinutes { get; set; }
        public double ExpirationInMinutes { get; set; }
    }
}