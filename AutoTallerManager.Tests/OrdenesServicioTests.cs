using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.API.Controllers;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Infrastructure.Persistence.Context;
using AutoTallerManager.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AutoTallerManager.Tests;

public class OrdenesServicioTests
{
    private static IUnitOfWork CreateUow(out AppDbContext db)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        db = new AppDbContext(options);
        return new UnitOfWork(db);
    }

    [Fact]
    public async Task AddDetalle_DecreasesStock_WhenAddingRepuesto()
    {
        var uow = CreateUow(out var db);

        var cliente = new Cliente { NombreCompleto = "Cliente" };
    var vehiculo = new Vehiculo { Cliente = cliente, ClienteId = cliente.Id, Placa = "TEST-001", VIN = "VIN-TEST-001" };
        var orden = new OrdenServicio { Vehiculo = vehiculo, VehiculoId = vehiculo.Id, FechaIngreso = DateTime.UtcNow, FechaEstimadaEntrega = DateTime.UtcNow.AddDays(1) };
    var repuesto = new Repuesto { Codigo = "R1", NombreRepu = "Filtro", Descripcion = "Filtro de aceite", Stock = 5, PrecioUnitario = 10, CategoriaId = 1, TipoVehiculoId = 1, FabricanteId = 1 };

        db.Clientes.Add(cliente);
        db.Vehiculos.Add(vehiculo);
        db.OrdenesServicio.Add(orden);
        db.Repuestos.Add(repuesto);
        await db.SaveChangesAsync();

        var logger = Mock.Of<ILogger<OrdenesServicioController>>();
        var controller = new OrdenesServicioController(uow, logger);

        var detalle = new DetalleOrden { RepuestoId = repuesto.Id, Cantidad = 2, PrecioUnitario = 10, PrecioManoDeObra = 5 };
        var result = await controller.AddDetalle(orden.Id, detalle, CancellationToken.None);

        var updatedRepuesto = await db.Repuestos.FindAsync(repuesto.Id);
        Assert.NotNull(updatedRepuesto);
        Assert.Equal(3, updatedRepuesto!.Stock); // 5 - 2
        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task CerrarOrden_GeneratesFactura_WithTotal()
    {
        var uow = CreateUow(out var db);

        var cliente = new Cliente { NombreCompleto = "Cliente2" };
    var vehiculo = new Vehiculo { Cliente = cliente, ClienteId = cliente.Id, Placa = "TEST-002", VIN = "VIN-TEST-002" };
        var orden = new OrdenServicio { Vehiculo = vehiculo, VehiculoId = vehiculo.Id, FechaIngreso = DateTime.UtcNow, FechaEstimadaEntrega = DateTime.UtcNow.AddDays(1) };
    var repuesto = new Repuesto { Codigo = "R2", NombreRepu = "Aceite", Descripcion = "Aceite 5W-30", Stock = 10, PrecioUnitario = 20, CategoriaId = 1, TipoVehiculoId = 1, FabricanteId = 1 };

        db.Clientes.Add(cliente);
        db.Vehiculos.Add(vehiculo);
        db.OrdenesServicio.Add(orden);
        db.Repuestos.Add(repuesto);
        await db.SaveChangesAsync();

        // add detalle (consume 1 repuesto)
        var logger = Mock.Of<ILogger<OrdenesServicioController>>();
        var controller = new OrdenesServicioController(uow, logger);
        await controller.AddDetalle(orden.Id, new DetalleOrden { RepuestoId = repuesto.Id, Cantidad = 1, PrecioUnitario = 20, PrecioManoDeObra = 30 }, CancellationToken.None);

        var closeResult = await controller.CerrarOrden(orden.Id, new OrdenesServicioController.CerrarOrdenRequest { TipoPagoId = 1 }, CancellationToken.None) as OkObjectResult;
        Assert.NotNull(closeResult);

        var payload = closeResult!.Value as dynamic;
        int facturaId = payload.FacturaId;
        decimal total = payload.Total;

        Assert.True(facturaId > 0);
        Assert.Equal(50m, total); // 1*20 + 30 mano de obra
    }
}


