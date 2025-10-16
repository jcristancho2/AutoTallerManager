using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Entities.Auth;
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
        Roles = new RoleRepository(_context);
        
        // Repositorios de negocio
        // Clientes
        Customer = new CustomerRepository(_context);
        // Vehículos
        Vehicles = new VehicleRepository(_context);
        // Ordenes de servicio
        ServiceOrders = new OrderServiceRepository(_context);
        // Repuestos
        Spares = new SpareRepository(_context);
        // Facturas
        Invoices = new InvoiceRepository(_context);
        // Auditorías
        Audits = new AuditRepository(_context);
        // Detalle de orden
        OrderDetails = new DetalleOrdenRepository(_context);
        // User auth helpers
        User = new AutoTallerManager.Infrastructure.Repositories.Auth.UserRepository(_context);
        UserStatus = new UserStatusRepository(_context);
        // Tipos de servicio
        ServiceTypes = new ServiceTypeRepository(_context);
    }

    // Repositorios de Auth
    public IUserMemberService UserMembers { get; }
    public IUserMemberRolService UserMemberRoles { get; }
    public IRoleService Roles { get; }
    public IUserService User { get; }
    
    // Repositorios de negocio
    public ICustomerService Customer { get; }
    public IVehicleService Vehicles { get; }
    public IOrderServiceService ServiceOrders { get; }
    public ISpareService Spares { get; }
    public IInvoiceService Invoices { get; }
    public IAuditService Audits { get; }
    public IDetailOrderService OrderDetails { get; }
    public IUserStatusService UserStatus { get; }
    public IServiceTypeService ServiceTypes { get; }

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
