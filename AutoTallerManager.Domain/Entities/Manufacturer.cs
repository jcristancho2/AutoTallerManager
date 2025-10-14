using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Manufacturer : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public ICollection<Spare>? Spares { get; set; }

    }
}