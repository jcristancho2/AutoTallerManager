using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceService
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Invoice> query = _context.Invoices;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(
        Expression<Func<Invoice, bool>>? filter = null,
        Func<IQueryable<Invoice>, IOrderedQueryable<Invoice>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Invoice> query = _context.Invoices;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<Invoice, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Invoice> query = _context.Invoices;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Invoice, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Invoices.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        await _context.Invoices.AddAsync(invoice, ct);
    }

    public Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
    {
        _context.Invoices.Update(invoice);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(f => f.Id == id, ct);
        if (invoice == null)
            return false;

        _context.Invoices.Remove(invoice);
        return true;
    }

    public async Task<IEnumerable<Invoice>> GetFacturasByClienteAsync(int clienteId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Where(f => f.CustomerId == clienteId)
            .Include(f => f.ServiceOrder)
            .Include(f => f.PaymentType)
            .OrderByDescending(f => f.InvoiceDate)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalIngresosAsync(DateTime DateFrom, DateTime DateTo, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Where(f => f.InvoiceDate >= DateFrom && f.InvoiceDate <= DateTo)
            .SumAsync(f => f.Total, ct);
    }

    public async Task<Invoice?> GetByServiceOrderIdAsync(int serviceOrderId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(f => f.ServiceOrder)
            .Include(f => f.Customer)
            .Include(f => f.PaymentType)
            .FirstOrDefaultAsync(f => f.ServiceOrderId == serviceOrderId, ct);
    }

    public async Task<Invoice?> GetLastInvoiceAsync(CancellationToken ct = default)
    {
        return await _context.Invoices
            .OrderByDescending(f => f.Id)
            .FirstOrDefaultAsync(ct);
    }
}