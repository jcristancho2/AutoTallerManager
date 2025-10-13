using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Domain.Entities
{
    public class OrdenServicio : BaseEntity
    {
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; }
        public string? DescripcionTrabajo { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public int MecanicoId { get; set; }  // Referencia a Usuario
        public Usuario? Mecanico { get; set; }

        public int TipoServId { get; set; }
        public TipoServicio? TipoServicio { get; set; }

        public int EstadoId { get; set; }
        public EstadoServ? Estado { get; set; }
        // Relaciones correctas
        public ICollection<DetalleOrden>? DetallesOrden { get; set; }
        public ICollection<Factura>? Facturas { get; set; }
        //public Factura? Factura { get; set; }
    }
}