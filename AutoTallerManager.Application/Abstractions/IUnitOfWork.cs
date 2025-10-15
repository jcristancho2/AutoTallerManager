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
    IServiceUser User { get; }
    IUserStatusService UserStatus { get; }
    
    //Repositorios de negocio
    IServiceCustomer Customer { get; }
    IVehicleService Vehicles { get; }
    IOrderServiceService ServiceOrders { get; }
    ISpareService Spares { get; }
    IInvoiceService Invoices { get; }
    IAuditService Audits { get; }
    IDetailOrderService OrderDetails { get; }
    IServiceTypeService ServiceTypes { get; }

    Task<int> SaveChanges(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}