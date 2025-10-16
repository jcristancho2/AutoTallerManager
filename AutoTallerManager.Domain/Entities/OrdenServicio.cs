using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Domain.Entities
{
    public class OrdenServicio : BaseEntity
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        
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

        public ICollection<DetalleOrdenServicio> Detalles { get; set; } = new List<DetalleOrdenServicio>();
    }
}