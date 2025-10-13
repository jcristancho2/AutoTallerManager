using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Infrastructure.Persistence.Context;
using AutoTallerManager.Infrastructure.Repositories;
using AutoTallerManager.Infrastructure.Repositories.Auth;

namespace AutoTallerManager.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        // Repositorios de Auth
        UserMembers = new UserMemberRepository(_context);
        UserMemberRoles = new UserMemberRolRepository(_context);
        Roles = new RolRepository(_context);
        
        // Repositorios de negocio
        Clientes = new ClienteRepository(_context);
        Vehiculos = new VehiculoRepository(_context);
        OrdenesServicio = new OrdenServicioRepository(_context);
        Repuestos = new RepuestoRepository(_context);
        Facturas = new FacturaRepository(_context);
        Auditorias = new AuditoriaRepository(_context);
        DetallesOrden = new DetalleOrdenRepository(_context);
        Usuarios = new UsuarioRepository(_context);
        EstadosUsuario = new EstadoUsuarioRepository(_context);
        TiposServicio = new TipoServicioRepository(_context);
    }

    // Repositorios de Auth
    public IUserMemberService UserMembers { get; }
    public IUserMemberRolService UserMemberRoles { get; }
    public IRolService Roles { get; }
    
    // Repositorios de negocio
    public IClienteService Clientes { get; }
    public IVehiculoService Vehiculos { get; }
    public IOrdenServicioService OrdenesServicio { get; }
    public IRepuestoService Repuestos { get; }
    public IFacturaService Facturas { get; }
    public IAuditoriaService Auditorias { get; }
    public IDetalleOrdenService DetallesOrden { get; }
    public IUsuarioService Usuarios { get; }
    public IEstadoUsuarioService EstadosUsuario { get; }
    public ITipoServicioService TiposServicio { get; }

    public Task<int> SaveChanges(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
