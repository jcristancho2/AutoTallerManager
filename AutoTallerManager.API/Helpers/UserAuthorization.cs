using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.Helpers
{
    public class UserAuthorization
    {
        public enum Roles
        {
            Administrador,
            Cliente,
            Mecanico,
            Proveedor
        }

        public const Roles rol_default = Roles.Cliente;
    }
}