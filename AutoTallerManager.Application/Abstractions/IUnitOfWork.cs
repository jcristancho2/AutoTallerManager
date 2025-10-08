using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Application.Abstractions
{
    public interface IUnitOfWork
    {
        //IProductRepository Products { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
    }
}