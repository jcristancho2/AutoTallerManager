using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Application.Abstractions.Interfaces;

namespace AutoTallerManager.Application.Abstractions;

public interface IUnitOfWork
{
    // Repositorios de Auth
    IUserMemberService UserMembers { get; }
    IUserMemberRolService UserMemberRoles { get; }
    IRolService Roles { get; }
    IUsuarioService Usuarios { get; }
    IEstadoUsuarioService EstadosUsuario { get; }
    
    //Repositorios de negocio
    IClienteService Clientes { get; }
    IVehiculoService Vehiculos { get; }
    IOrdenServicioService OrdenesServicio { get; }
    IRepuestoService Repuestos { get; }
    IFacturaService Facturas { get; }
    IAuditoriaService Auditorias { get; }
    IDetalleOrdenService DetallesOrden { get; }
    ITipoServicioService TiposServicio { get; }



    Task<int> SaveChanges(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}