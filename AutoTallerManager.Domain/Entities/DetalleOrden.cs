using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class DetalleOrden : BaseEntity
    {
        public int DetalleOrdenId { get; set; }
        public int OrdenServicioId { get; set; }
        public int? RepuestoId { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioManoDeObra { get; set; }
        public OrdenServicio? OrdenServicio { get; set; }
        public Repuesto? Repuesto { get; set; }
    }
}