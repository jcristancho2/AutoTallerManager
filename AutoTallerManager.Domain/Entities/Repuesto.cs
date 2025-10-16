using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Domain.Entities
{
    public class Repuesto : BaseEntity
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; }
        public string? NombreRepu { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public decimal PrecioUnitario { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int TipoVehiculoId { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }

        public int FabricanteId { get; set; }
        public Fabricante? Fabricante { get; set; }
        public ICollection<DetalleOrden>? DetallesOrden { get; set; }
    }
}