using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities;

    public class Factura : BaseEntity
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        public int OrdenServicioId { get; set; }
        public OrdenServicio? OrdenServicio { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int TipoPagoId { get; set; }
        public TipoPago? TipoPago { get; set; }
    }