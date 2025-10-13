using AutoTallerManager.Application.Features.OrdenesServicio.Commands;
using AutoTallerManager.Application.Features.OrdenesServicio.Handlers;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Domain.Entities;
using Moq;
using Xunit;
using AutoTallerManager.Application.Abstractions.Interfaces;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Application.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoTallerManager.Tests.Handlers
{
    public class CrearOrdenServicioHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVehiculoService> _mockVehiculoService;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly Mock<ITipoServicioService> _mockTipoServicioService;
        private readonly Mock<IOrdenServicioService> _mockOrdenServicioService;
        private readonly Mock<IRepuestoService> _mockRepuestoService;
        private readonly Mock<IDetalleOrdenService> _mockDetalleOrdenService;
        private readonly Mock<IValidadorDisponibilidadVehiculoService> _mockValidadorVehiculo;
        private readonly Mock<ICalculadoraFechasService> _mockCalculadoraFechas;
        private readonly CrearOrdenServicioHandler _handler;

        public CrearOrdenServicioHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVehiculoService = new Mock<IVehiculoService>();
            _mockUsuarioService = new Mock<IUsuarioService>();
            _mockTipoServicioService = new Mock<ITipoServicioService>();
            _mockOrdenServicioService = new Mock<IOrdenServicioService>();
            _mockRepuestoService = new Mock<IRepuestoService>();
            _mockDetalleOrdenService = new Mock<IDetalleOrdenService>();
            _mockValidadorVehiculo = new Mock<IValidadorDisponibilidadVehiculoService>();
            _mockCalculadoraFechas = new Mock<ICalculadoraFechasService>();

            _mockUnitOfWork.Setup(x => x.Vehiculos).Returns(_mockVehiculoService.Object);
            _mockUnitOfWork.Setup(x => x.Usuarios).Returns(_mockUsuarioService.Object);
            _mockUnitOfWork.Setup(x => x.TiposServicio).Returns(_mockTipoServicioService.Object);
            _mockUnitOfWork.Setup(x => x.OrdenesServicio).Returns(_mockOrdenServicioService.Object);
            _mockUnitOfWork.Setup(x => x.Repuestos).Returns(_mockRepuestoService.Object);
            _mockUnitOfWork.Setup(x => x.DetallesOrden).Returns(_mockDetalleOrdenService.Object);

            _handler = new CrearOrdenServicioHandler(
                _mockUnitOfWork.Object,
                _mockValidadorVehiculo.Object,
                _mockCalculadoraFechas.Object);
        }

        [Fact]
        public async Task Handle_VehiculoNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow
            };

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync((Vehiculo?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MecanicoNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow
            };

            var vehiculo = new Vehiculo { Id = 1, ClienteId = 1 };
            var mecanico = (Usuario?)null;

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehiculo);

            _mockUsuarioService
                .Setup(x => x.GetByIdAsync(command.MecanicoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mecanico);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TipoServicioNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow
            };

            var vehiculo = new Vehiculo { Id = 1, ClienteId = 1 };
            var mecanico = new Usuario { Id = 1 };
            var tipoServicio = (TipoServicio?)null;

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehiculo);

            _mockUsuarioService
                .Setup(x => x.GetByIdAsync(command.MecanicoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mecanico);

            _mockTipoServicioService
                .Setup(x => x.GetByIdAsync(command.TipoServicioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoServicio);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_VehiculoNoDisponible_LanzaInvalidOperationException()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow
            };

            var vehiculo = new Vehiculo { Id = 1, ClienteId = 1 };
            var mecanico = new Usuario { Id = 1 };
            var tipoServicio = new TipoServicio { Id = 1, NombreTipoServ = "Reparación" };

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehiculo);

            _mockUsuarioService
                .Setup(x => x.GetByIdAsync(command.MecanicoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mecanico);

            _mockTipoServicioService
                .Setup(x => x.GetByIdAsync(command.TipoServicioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoServicio);

            _mockValidadorVehiculo
                .Setup(x => x.VehiculoDisponibleAsync(command.VehiculoId, command.FechaIngreso, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_StockInsuficiente_LanzaInvalidOperationException()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow,
                RepuestosRequeridos = new List<RepuestoRequeridoDto>
                {
                    new RepuestoRequeridoDto { RepuestoId = 1, Cantidad = 10 }
                }
            };

            var vehiculo = new Vehiculo { Id = 1, ClienteId = 1 };
            var mecanico = new Usuario { Id = 1 };
            var tipoServicio = new TipoServicio { Id = 1, NombreTipoServ = "Reparación" };
            var repuesto = new Repuesto { Id = 1, Stock = 5, NombreRepu = "Filtro" };

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehiculo);

            _mockUsuarioService
                .Setup(x => x.GetByIdAsync(command.MecanicoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mecanico);

            _mockTipoServicioService
                .Setup(x => x.GetByIdAsync(command.TipoServicioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoServicio);

            _mockValidadorVehiculo
                .Setup(x => x.VehiculoDisponibleAsync(command.VehiculoId, command.FechaIngreso, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepuestoService
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(repuesto);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComandoValido_CreaOrdenServicio()
        {
            // Arrange
            var command = new CrearOrdenServicioCommand
            {
                VehiculoId = 1,
                MecanicoId = 1,
                TipoServicioId = 1,
                FechaIngreso = DateTime.UtcNow,
                DescripcionTrabajo = "Reparación de motor"
            };

            var vehiculo = new Vehiculo { Id = 1, ClienteId = 1 };
            var mecanico = new Usuario { Id = 1 };
            var tipoServicio = new TipoServicio { Id = 1, NombreTipoServ = "Reparación" };

            _mockVehiculoService
                .Setup(x => x.GetByIdAsync(command.VehiculoId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehiculo);

            _mockUsuarioService
                .Setup(x => x.GetByIdAsync(command.MecanicoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mecanico);

            _mockTipoServicioService
                .Setup(x => x.GetByIdAsync(command.TipoServicioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoServicio);

            _mockValidadorVehiculo
                .Setup(x => x.VehiculoDisponibleAsync(command.VehiculoId, command.FechaIngreso, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockCalculadoraFechas
                .Setup(x => x.CalcularComplejidadServicio(tipoServicio))
                .Returns(3);

            _mockCalculadoraFechas
                .Setup(x => x.CalcularFechaEstimadaEntrega(tipoServicio, 3))
                .Returns(DateTime.UtcNow.AddDays(3));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado > 0);
            _mockOrdenServicioService.Verify(x => x.AddAsync(It.IsAny<OrdenServicio>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
