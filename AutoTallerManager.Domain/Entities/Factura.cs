using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class Factura
    {
         public int FacturaId { get; set; }

        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public int OrdenServicioId { get; set; }
        public OrdenServicio? OrdenServicio { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int PagoId { get; set; }
        public TipoPago? TipoPago { get; set; }
    }
}