using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoServicio : BaseEntity
    {
        public string? NombreTipoServ { get; set; }
        public ICollection<OrdenServicio>? OrdenesServicio { get; set; }
        

    }
}