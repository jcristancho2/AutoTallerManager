using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Linq.Expressions;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("Repuestos")]
public class RepuestosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RepuestosController> _logger;

    public RepuestosController(IUnitOfWork unitOfWork, ILogger<RepuestosController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Repuesto>>> GetRepuestos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nombre = null,
        [FromQuery] string? codigo = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] int? fabricanteId = null,
        CancellationToken ct = default)
    {
        try
        {
            Expression<Func<Repuesto, bool>>? filter = r =>
                (string.IsNullOrEmpty(nombre) || (r.NombreRepu != null && r.NombreRepu.Contains(nombre))) &&
                (string.IsNullOrEmpty(codigo) || (r.Codigo != null && r.Codigo.Contains(codigo))) &&
                (!categoriaId.HasValue || r.CategoriaId == categoriaId) &&
                (!fabricanteId.HasValue || r.FabricanteId == fabricanteId);

            var repuestos = await _unitOfWork.Repuestos.GetAllAsync(
                filter: filter,
                orderBy: q => q.OrderBy(r => r.NombreRepu),
                includeProperties: "Categoria,TipoVehiculo,Fabricante",
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                ct: ct);

            var total = await _unitOfWork.Repuestos.CountAsync(filter, ct);

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(repuestos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener repuestos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Repuesto>> GetRepuesto(int id, CancellationToken ct = default)
    {
        try
        {
            var repuesto = await _unitOfWork.Repuestos.GetByIdAsync(id, ct, "Categoria,TipoVehiculo,Fabricante");
            if (repuesto == null)
                return NotFound($"Repuesto con ID {id} no encontrado");

            return Ok(repuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener repuesto {RepuestoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<ActionResult<Repuesto>> GetRepuestoByCodigo(string codigo, CancellationToken ct = default)
    {
        try
        {
            var repuesto = await _unitOfWork.Repuestos.GetByCodigoAsync(codigo, ct);
            if (repuesto == null)
                return NotFound($"Repuesto con código {codigo} no encontrado");

            return Ok(repuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar repuesto por código {Codigo}", codigo);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("stock/bajo")]
    public async Task<ActionResult<IEnumerable<Repuesto>>> GetRepuestosStockBajo([FromQuery] int stockMinimo = 5, CancellationToken ct = default)
    {
        try
        {
            var repuestos = await _unitOfWork.Repuestos.GetRepuestosStockBajoAsync(stockMinimo, ct);
            return Ok(repuestos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener repuestos con stock bajo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Repuesto>> CreateRepuesto(Repuesto repuesto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar relaciones: Categoria, TipoVehiculo, Fabricante
            // Si la aplicación tiene un servicio para Categorias/Fabricantes/TipoVehiculo, usarlo aquí para validar existence.

            // Evitar duplicados por código
            if (!string.IsNullOrEmpty(repuesto.Codigo))
            {
                var codigoExists = await _unitOfWork.Repuestos.ExistsAsync(r => r.Codigo == repuesto.Codigo, ct);
                if (codigoExists)
                    return BadRequest("Ya existe un repuesto con este código");
            }

            await _unitOfWork.Repuestos.AddAsync(repuesto, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Repuesto creado: {RepuestoId}", repuesto.Id);
            return CreatedAtAction(nameof(GetRepuesto), new { id = repuesto.Id }, repuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear repuesto");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Repuesto>> UpdateRepuesto(int id, Repuesto repuesto, CancellationToken ct = default)
    {
        try
        {
            if (id != repuesto.Id)
                return BadRequest("El ID del repuesto no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unitOfWork.Repuestos.GetByIdAsync(id, ct);
            if (existing == null)
                return NotFound($"Repuesto con ID {id} no encontrado");

            // Evitar duplicados por código
            if (!string.IsNullOrEmpty(repuesto.Codigo))
            {
                var codigoExists = await _unitOfWork.Repuestos.ExistsAsync(r => r.Codigo == repuesto.Codigo && r.Id != id, ct);
                if (codigoExists)
                    return BadRequest("Ya existe otro repuesto con este código");
            }

            // Actualizar campos
            existing.Codigo = repuesto.Codigo;
            existing.NombreRepu = repuesto.NombreRepu;
            existing.Descripcion = repuesto.Descripcion;
            existing.Stock = repuesto.Stock;
            existing.PrecioUnitario = repuesto.PrecioUnitario;
            existing.CategoriaId = repuesto.CategoriaId;
            existing.TipoVehiculoId = repuesto.TipoVehiculoId;
            existing.FabricanteId = repuesto.FabricanteId;

            await _unitOfWork.Repuestos.UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Repuesto actualizado: {RepuestoId}", id);
            return Ok(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar repuesto {RepuestoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRepuesto(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _unitOfWork.Repuestos.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound($"Repuesto con ID {id} no encontrado");

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Repuesto eliminado: {RepuestoId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar repuesto {RepuestoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}