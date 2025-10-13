using AutoTallerManager.Application.Services;
using AutoTallerManager.Domain.Entities;
using Xunit;
using System;

namespace AutoTallerManager.Tests.Services
{
    public class CalculadoraFechasServiceTests
    {
        private readonly CalculadoraFechasService _service;

        public CalculadoraFechasServiceTests()
        {
            _service = new CalculadoraFechasService();
        }

        [Fact]
        public void CalcularFechaEstimadaEntrega_MantenimientoPreventivo_Retorna1Dia()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Mantenimiento Preventivo"
            };

            // Act
            var fechaEstimada = _service.CalcularFechaEstimadaEntrega(tipoServicio);

            // Assert
            var diferenciaDias = (fechaEstimada - DateTime.UtcNow).Days;
            Assert.Equal(1, diferenciaDias);
        }

        [Fact]
        public void CalcularFechaEstimadaEntrega_Reparacion_Retorna3Dias()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Reparación"
            };

            // Act
            var fechaEstimada = _service.CalcularFechaEstimadaEntrega(tipoServicio);

            // Assert
            var diferenciaDias = (fechaEstimada - DateTime.UtcNow).Days;
            Assert.Equal(3, diferenciaDias);
        }

        [Fact]
        public void CalcularFechaEstimadaEntrega_Diagnostico_Retorna1Dia()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Diagnóstico"
            };

            // Act
            var fechaEstimada = _service.CalcularFechaEstimadaEntrega(tipoServicio);

            // Assert
            var diferenciaDias = (fechaEstimada - DateTime.UtcNow).Days;
            Assert.Equal(1, diferenciaDias);
        }

        [Fact]
        public void CalcularFechaEstimadaEntrega_TipoDesconocido_Retorna2Dias()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Tipo Desconocido"
            };

            // Act
            var fechaEstimada = _service.CalcularFechaEstimadaEntrega(tipoServicio);

            // Assert
            var diferenciaDias = (fechaEstimada - DateTime.UtcNow).Days;
            Assert.Equal(2, diferenciaDias);
        }

        [Fact]
        public void CalcularComplejidadServicio_MantenimientoPreventivo_Retorna1()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Mantenimiento Preventivo"
            };

            // Act
            var complejidad = _service.CalcularComplejidadServicio(tipoServicio);

            // Assert
            Assert.Equal(1, complejidad);
        }

        [Fact]
        public void CalcularComplejidadServicio_Reparacion_Retorna3()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Reparación"
            };

            // Act
            var complejidad = _service.CalcularComplejidadServicio(tipoServicio);

            // Assert
            Assert.Equal(3, complejidad);
        }

        [Fact]
        public void CalcularComplejidadServicio_Diagnostico_Retorna2()
        {
            // Arrange
            var tipoServicio = new TipoServicio
            {
                NombreTipoServ = "Diagnóstico"
            };

            // Act
            var complejidad = _service.CalcularComplejidadServicio(tipoServicio);

            // Assert
            Assert.Equal(2, complejidad);
        }
    }
}
