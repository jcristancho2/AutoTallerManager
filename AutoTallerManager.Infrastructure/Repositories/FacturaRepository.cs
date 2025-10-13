using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoTallerManager.Infrastructure.Repositories;

public class FacturaRepository : IFacturaService
{
    private readonly AppDbContext _context;

    public FacturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Factura?> GetByIdAsync(int id, CancellationToken ct = default, params string[] includeProperties)
    {
        IQueryable<Factura> query = _context.Facturas;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IEnumerable<Factura>> GetAllAsync(
        Expression<Func<Factura, bool>>? filter = null,
        Func<IQueryable<Factura>, IOrderedQueryable<Factura>>? orderBy = null,
        string includeProperties = "",
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Factura> query = _context.Facturas;

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

    public async Task<int> CountAsync(Expression<Func<Factura, bool>>? filter = null, CancellationToken ct = default)
    {
        IQueryable<Factura> query = _context.Facturas;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Factura, bool>> filter, CancellationToken ct = default)
    {
        return await _context.Facturas.AnyAsync(filter, ct);
    }

    public async Task AddAsync(Factura factura, CancellationToken ct = default)
    {
        await _context.Facturas.AddAsync(factura, ct);
    }

    public Task UpdateAsync(Factura factura, CancellationToken ct = default)
    {
        _context.Facturas.Update(factura);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var factura = await _context.Facturas.FirstOrDefaultAsync(f => f.Id == id, ct);
        if (factura == null)
            return false;

        _context.Facturas.Remove(factura);
        return true;
    }

    public async Task<IEnumerable<Factura>> GetFacturasByClienteAsync(int clienteId, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Where(f => f.ClienteId == clienteId)
            .Include(f => f.OrdenServicio)
            .Include(f => f.TipoPago)
            .OrderByDescending(f => f.Fecha)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalIngresosAsync(DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Where(f => f.Fecha >= fechaDesde && f.Fecha <= fechaHasta)
            .SumAsync(f => f.Total, ct);
    }

    public async Task<Factura?> GetByOrdenServicioIdAsync(int ordenServicioId, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Include(f => f.OrdenServicio)
            .Include(f => f.Cliente)
            .Include(f => f.TipoPago)
            .FirstOrDefaultAsync(f => f.OrdenServicioId == ordenServicioId, ct);
    }

    public async Task<Factura?> GetUltimaFacturaAsync(CancellationToken ct = default)
    {
        return await _context.Facturas
            .OrderByDescending(f => f.Id)
            .FirstOrDefaultAsync(ct);
    }
}