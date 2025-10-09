using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class TipoCliente
    {
        public int Id { get; private set;}
        public string? Nombre {get; private set;} //persona natural o empresa
        private TipoCliente() { }
        public TipoCliente(string nombre)
        {
            Nombre = nombre;
        }

        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}