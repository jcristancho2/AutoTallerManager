using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AutoTallerManager.Tests;

public class VehiculosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VehiculosEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetVehiculos_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/vehiculos");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetVehiculoById_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/vehiculos/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetVehiculosByCliente_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/vehiculos/cliente/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetVehiculoByVin_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/vehiculos/vin/TEST123");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehiculo_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var vehiculoRequest = new
        {
            Vin = "TEST123456789",
            Placa = "ABC123",
            ClienteId = 1,
            MarcaId = 1,
            ModeloId = 1,
            Año = 2020
        };
        var json = JsonSerializer.Serialize(vehiculoRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/vehiculos", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateVehiculo_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var vehiculoRequest = new
        {
            Id = 1,
            Vin = "UPDATED123456789",
            Placa = "XYZ789",
            ClienteId = 1,
            MarcaId = 1,
            ModeloId = 1,
            Año = 2021
        };
        var json = JsonSerializer.Serialize(vehiculoRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/vehiculos/1", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVehiculo_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/vehiculos/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
