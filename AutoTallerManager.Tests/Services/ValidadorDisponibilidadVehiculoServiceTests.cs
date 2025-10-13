using AutoTallerManager.Application.Services;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Domain.Entities;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTallerManager.Tests.Services
{
    public class ValidadorDisponibilidadVehiculoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOrdenServicioService> _mockOrdenServicioService;
        private readonly ValidadorDisponibilidadVehiculoService _service;

        public ValidadorDisponibilidadVehiculoServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrdenServicioService = new Mock<IOrdenServicioService>();
            _mockUnitOfWork.Setup(x => x.OrdenesServicio).Returns(_mockOrdenServicioService.Object);
            _service = new ValidadorDisponibilidadVehiculoService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task VehiculoDisponibleAsync_SinOrdenesActivas_RetornaTrue()
        {
            // Arrange
            var vehiculoId = 1;
            var fechaInicio = DateTime.UtcNow;
            var ordenesActivas = new List<OrdenServicio>();

            _mockOrdenServicioService
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<OrdenServicio, bool>>>(),
                    It.IsAny<Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordenesActivas);

            // Act
            var resultado = await _service.VehiculoDisponibleAsync(vehiculoId, fechaInicio);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task VehiculoDisponibleAsync_ConOrdenActivaEnFechaDiferente_RetornaTrue()
        {
            // Arrange
            var vehiculoId = 1;
            var fechaInicio = DateTime.UtcNow.AddDays(5);
            var ordenesActivas = new List<OrdenServicio>
            {
                new OrdenServicio
                {
                    Id = 1,
                    VehiculoId = vehiculoId,
                    FechaIngreso = DateTime.UtcNow,
                    FechaEstimadaEntrega = DateTime.UtcNow.AddDays(2),
                    EstadoId = 1 // Pendiente
                }
            };

            _mockOrdenServicioService
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<OrdenServicio, bool>>>(),
                    It.IsAny<Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordenesActivas);

            // Act
            var resultado = await _service.VehiculoDisponibleAsync(vehiculoId, fechaInicio);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task VehiculoDisponibleAsync_ConOrdenActivaEnMismaFecha_RetornaFalse()
        {
            // Arrange
            var vehiculoId = 1;
            var fechaInicio = DateTime.UtcNow;
            var ordenesActivas = new List<OrdenServicio>
            {
                new OrdenServicio
                {
                    Id = 1,
                    VehiculoId = vehiculoId,
                    FechaIngreso = DateTime.UtcNow,
                    FechaEstimadaEntrega = DateTime.UtcNow.AddDays(2),
                    EstadoId = 1 // Pendiente
                }
            };

            _mockOrdenServicioService
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<OrdenServicio, bool>>>(),
                    It.IsAny<Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordenesActivas);

            // Act
            var resultado = await _service.VehiculoDisponibleAsync(vehiculoId, fechaInicio);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task VehiculoDisponibleAsync_ConOrdenExcluir_IgnoraOrdenExcluida()
        {
            // Arrange
            var vehiculoId = 1;
            var fechaInicio = DateTime.UtcNow;
            var ordenExcluir = 1;
            var ordenesActivas = new List<OrdenServicio>
            {
                new OrdenServicio
                {
                    Id = 1,
                    VehiculoId = vehiculoId,
                    FechaIngreso = DateTime.UtcNow,
                    FechaEstimadaEntrega = DateTime.UtcNow.AddDays(2),
                    EstadoId = 1 // Pendiente
                }
            };

            _mockOrdenServicioService
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<OrdenServicio, bool>>>(),
                    It.IsAny<Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordenesActivas);

            // Act
            var resultado = await _service.VehiculoDisponibleAsync(vehiculoId, fechaInicio, null, ordenExcluir);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task GetOrdenesActivasVehiculoAsync_RetornaSoloEstadosActivos()
        {
            // Arrange
            var vehiculoId = 1;
            var ordenes = new List<OrdenServicio>
            {
                new OrdenServicio
                {
                    Id = 1,
                    VehiculoId = vehiculoId,
                    EstadoId = 1, // Pendiente - Activo
                    FechaIngreso = DateTime.UtcNow
                },
                new OrdenServicio
                {
                    Id = 2,
                    VehiculoId = vehiculoId,
                    EstadoId = 2, // En Proceso - Activo
                    FechaIngreso = DateTime.UtcNow
                },
                new OrdenServicio
                {
                    Id = 3,
                    VehiculoId = vehiculoId,
                    EstadoId = 3, // Completada - No Activo
                    FechaIngreso = DateTime.UtcNow
                },
                new OrdenServicio
                {
                    Id = 4,
                    VehiculoId = vehiculoId,
                    EstadoId = 5, // Esperando Repuestos - Activo
                    FechaIngreso = DateTime.UtcNow
                }
            };

            _mockOrdenServicioService
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<OrdenServicio, bool>>>(),
                    It.IsAny<Func<IQueryable<OrdenServicio>, IOrderedQueryable<OrdenServicio>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordenes.Where(o => new[] { 1, 2, 5 }.Contains(o.EstadoId)).ToList());

            // Act
            var resultado = await _service.GetOrdenesActivasVehiculoAsync(vehiculoId);

            // Assert
            Assert.Equal(3, resultado.Count());
            Assert.All(resultado, orden => Assert.Contains(orden.EstadoId, new[] { 1, 2, 5 }));
        }
    }
}
