using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Infrastructure.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);

}
