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
        Customers = new CustomerRepository(_context);
        Vehicles = new VehicleRepository(_context);
        OrderService = new OrderServiceRepository(_context);
        Spare = new SpareRepository(_context);
        Invoice = new InvoiceRepository(_context);
        Audits = new AuditRepository(_context);
        DetailOrder = new DetalleOrdenRepository(_context);
        Users = new UserMemberRepository(_context);
        UserStatus = new UserStatusRepository(_context);
        ServiceTypes = new ServiceTypeRepository(_context);
    }

    // Repositorios de Auth
    public IUserMemberService UserMembers { get; }
    public IUserMemberRolService UserMemberRoles { get; }
    public IRoleService Roles { get; }
    
    // Repositorios de negocio
    public ICustomerService Customers { get; }
    public IVehicleService Vehicles { get; }
    public IOrderServiceService OrderService { get; }
    public ISpareService Spare { get; }
    public IInvoiceService Invoice { get; }
    public IAuditService Audits { get; }
    public IDetailOrderService DetailOrder { get; }
    public IUserMemberService Users { get; }
    public IUserStatusService UserStatus { get; }
    public IUserStatusService StatusService { get; }

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
