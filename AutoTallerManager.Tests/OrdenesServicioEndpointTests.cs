using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AutoTallerManager.Tests;

public class OrdenesServicioEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdenesServicioEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetOrdenesServicio_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/ordenesservicio");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrdenServicioById_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/ordenesservicio/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrdenServicio_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var ordenServicio = new
        {
            ClienteId = 1,
            VehiculoId = 1,
            Descripcion = "Test Service Order",
            FechaInicio = System.DateTime.Now
        };
        var json = JsonSerializer.Serialize(ordenServicio);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/ordenesservicio", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrdenServicio_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var ordenServicio = new
        {
            Id = 1,
            ClienteId = 1,
            VehiculoId = 1,
            Descripcion = "Updated Service Order",
            FechaInicio = System.DateTime.Now
        };
        var json = JsonSerializer.Serialize(ordenServicio);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/ordenesservicio/1", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOrdenServicio_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/ordenesservicio/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddDetalleOrden_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var detalle = new
        {
            RepuestoId = 1,
            Cantidad = 2,
            PrecioUnitario = 50.00m,
            Descripcion = "Test Detail"
        };
        var json = JsonSerializer.Serialize(detalle);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/ordenesservicio/1/detalles", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDetalleOrden_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var detalle = new
        {
            Id = 1,
            RepuestoId = 1,
            Cantidad = 3,
            PrecioUnitario = 60.00m,
            Descripcion = "Updated Detail"
        };
        var json = JsonSerializer.Serialize(detalle);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/ordenesservicio/1/detalles/1", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDetalleOrden_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/ordenesservicio/1/detalles/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEstadoOrden_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var estadoRequest = new
        {
            Estado = "EnProceso"
        };
        var json = JsonSerializer.Serialize(estadoRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/ordenesservicio/1/estado", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CerrarOrden_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var cerrarRequest = new
        {
            Observaciones = "Orden cerrada"
        };
        var json = JsonSerializer.Serialize(cerrarRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/ordenesservicio/1/cerrar", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
