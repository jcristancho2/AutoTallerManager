using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.DTOs.Auth
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoolName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}