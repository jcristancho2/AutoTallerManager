using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AutoTallerManager.Tests;

public class RepuestosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RepuestosEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRepuestos_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/repuestos");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRepuestoById_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/repuestos/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRepuestoByCodigo_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/repuestos/codigo/ABC123");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRepuestosStockBajo_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/repuestos/stock/bajo");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRepuesto_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var repuesto = new
        {
            Nombre = "Test Repuesto",
            Codigo = "TEST001",
            Precio = 100.00m,
            Stock = 10
        };
        var json = JsonSerializer.Serialize(repuesto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/repuestos", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRepuesto_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var repuesto = new
        {
            Id = 1,
            Nombre = "Updated Repuesto",
            Codigo = "TEST001",
            Precio = 150.00m,
            Stock = 15
        };
        var json = JsonSerializer.Serialize(repuesto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/repuestos/1", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepuesto_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/repuestos/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
