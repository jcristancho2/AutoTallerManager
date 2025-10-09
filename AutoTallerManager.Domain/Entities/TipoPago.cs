using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoPago
    {
        public int PagoId { get; set; }
        public string? NombreTipoPag { get; set; }
        public ICollection<Factura>? Facturas { get; set; }
    }
}