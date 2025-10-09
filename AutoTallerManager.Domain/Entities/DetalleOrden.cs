using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class DetalleOrden
    {
       public int DetalleOrdenId { get; set; }     // Parte 1 de la PK
        public int OrdenServicioId { get; set; }    // Parte 2 de la PK

        public int? RepuestoId { get; set; }        // NULL permitido en el DDL

        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioManoDeObra { get; set; }

        public OrdenServicio? OrdenServicio { get; set; }
        public Repuesto? Repuesto { get; set; } 
    }
}