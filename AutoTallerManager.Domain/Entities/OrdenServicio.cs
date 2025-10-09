using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class OrdenServicio
    {
        public int OrdenServicioId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; }

        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        
        public int TipoServId { get; set; }
        public TipoServicio? TipoServicio { get; set; }

        public int EstadoId { get; set; }
        public EstadoServ? Estado { get; set; }

        public Factura? Factura { get; set; }

        public ICollection<Repuesto>? Repuestos { get; set; }
        public ICollection<Factura>? Facturas { get; set; }
        public ICollection<DetalleOrden>? DetallesOrden { get; set; }
    }
}