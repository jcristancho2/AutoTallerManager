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
        private readonly Mock<IVehicleService> _mockVehicleService;
        private readonly Mock<IServiceUser> _mockUserService;
        private readonly Mock<IServiceTypeService> _mockServiceTypeService;
        private readonly Mock<IOrderServiceService> _mockOrderServiceService;
        private readonly Mock<ISpareService> _mockSpareService;
        private readonly Mock<IDetailOrderService> _mockDetailOrderService;
        private readonly Mock<IVehicleServiceAvailabilityValidator> _mockValidatorVehicle;
        private readonly Mock<IDateCalculatorService> _mockDateCalculator;
        private readonly CreateServiceOrderHandler _handler;

        public CrearOrdenServicioHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVehicleService = new Mock<IVehicleService>();
            _mockUserService = new Mock<IServiceUser>();
            _mockServiceTypeService = new Mock<IServiceTypeService>();
            _mockOrderServiceService = new Mock<IOrderServiceService>();
            _mockSpareService = new Mock<ISpareService>();
            _mockDetailOrderService = new Mock<IDetailOrderService>();
            _mockValidatorVehicle = new Mock<IVehicleServiceAvailabilityValidator>();
            _mockDateCalculator = new Mock<IDateCalculatorService>();

            _mockUnitOfWork.Setup(x => x.Vehicles).Returns(_mockVehicleService.Object);
            _mockUnitOfWork.Setup(x => x.User).Returns(_mockUserService.Object);
            _mockUnitOfWork.Setup(x => x.ServiceTypes).Returns(_mockServiceTypeService.Object);
            _mockUnitOfWork.Setup(x => x.ServiceOrders).Returns(_mockOrderServiceService.Object);
            _mockUnitOfWork.Setup(x => x.Spares).Returns(_mockSpareService.Object);
            _mockUnitOfWork.Setup(x => x.OrderDetails).Returns(_mockDetailOrderService.Object);

            _handler = new CreateServiceOrderHandler(
                _mockUnitOfWork.Object,
                _mockValidatorVehicle.Object,
                _mockDateCalculator.Object);
        }

        [Fact]
        public async Task Handle_VehiculoNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow
            };

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync((Vehicle?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MecanicoNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow
            };

            var vehicle = new Vehicle { Id = 1, CustomerId = 1 };
            var mechanic = (User?)null;

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehicle);

            _mockUserService
                .Setup(x => x.GetByIdAsync(command.MechanicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mechanic);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_TipoServicioNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow
            };

            var vehicle = new Vehicle { Id = 1, CustomerId = 1 };
            var mechanic = new User { Id = 1 };
            var serviceType = (ServiceType?)null;

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehicle);

            _mockUserService
                .Setup(x => x.GetByIdAsync(command.MechanicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mechanic);

            _mockServiceTypeService
                .Setup(x => x.GetByIdAsync(command.ServiceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceType);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_VehiculoNoDisponible_LanzaInvalidOperationException()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow
            };

            var vehicle = new Vehicle { Id = 1, CustomerId = 1 };
            var mechanic = new User { Id = 1 };
            var serviceType = new ServiceType { Id = 1, ServiceTypeName = "Reparación" };

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehicle);

            _mockUserService
                .Setup(x => x.GetByIdAsync(command.MechanicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mechanic);

            _mockServiceTypeService
                .Setup(x => x.GetByIdAsync(command.ServiceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceType);

            _mockValidatorVehicle
                .Setup(x => x.IsVehicleAvailableAsync(command.VehicleId, command.EntryDate, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_StockInsuficiente_LanzaInvalidOperationException()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow,
                SpareRequiredDto = new List<SpareRequiredDto>
                {
                    new SpareRequiredDto { SpareId = 1, Quantity = 10 }
                }
            };

            var vehicle = new Vehicle { Id = 1, CustomerId = 1 };
            var mechanic = new User { Id = 1 };
            var serviceType = new ServiceType { Id = 1, ServiceTypeName = "Reparación" };
            var spare = new Spare { Id = 1, Stock = 5, Name = "Filtro" };

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehicle);

            _mockUserService
                .Setup(x => x.GetByIdAsync(command.MechanicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mechanic);

            _mockServiceTypeService
                .Setup(x => x.GetByIdAsync(command.ServiceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceType);

            _mockValidatorVehicle
                .Setup(x => x.IsVehicleAvailableAsync(command.VehicleId, command.EntryDate, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockSpareService
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(spare);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComandoValido_CreaOrdenServicio()
        {
            // Arrange
            var command = new CreateServiceOrderCommand
            {
                VehicleId = 1,
                MechanicId = 1,
                ServiceTypeId = 1,
                EntryDate = DateTime.UtcNow,
                WorkDescription = "Reparación de motor"
            };

            var vehicle = new Vehicle { Id = 1, CustomerId = 1 };
            var mechanic = new User { Id = 1 };
            var serviceType = new ServiceType { Id = 1, ServiceTypeName = "Reparación" };

            _mockVehicleService
                .Setup(x => x.GetByIdAsync(command.VehicleId, It.IsAny<CancellationToken>(), It.IsAny<string>()))
                .ReturnsAsync(vehicle);

            _mockUserService
                .Setup(x => x.GetByIdAsync(command.MechanicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mechanic);

            _mockServiceTypeService
                .Setup(x => x.GetByIdAsync(command.ServiceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceType);

            _mockValidatorVehicle
                .Setup(x => x.IsVehicleAvailableAsync(command.VehicleId, command.EntryDate, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockDateCalculator
                .Setup(x => x.CalculateServiceComplexity(serviceType))
                .Returns(3);

            _mockDateCalculator
                .Setup(x => x.CalculateEstimatedDeliveryDate(serviceType, 3))
                .Returns(DateTime.UtcNow.AddDays(3));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado > 0);
            _mockOrderServiceService.Verify(x => x.AddAsync(It.IsAny<ServiceOrder>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
