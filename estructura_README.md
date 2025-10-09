

AutoTallerManager/
├── AutoTallerManager.sln

├── AutoTallerManager.API/
│   ├── Controllers/
│   │   ├── ClientesController.cs
│   │   ├── VehiculosController.cs
│   │   ├── RepuestosController.cs
│   │   ├── OrdenesServicioController.cs
│   │   ├── FacturasController.cs
│   │   ├── AuditoriasController.cs
│   │   └── AuthController.cs
│   ├── Extensions/
│   │   ├── ApplicationServiceExtensions.cs
│   │   ├── SwaggerServiceExtensions.cs
│   │   ├── JwtServiceExtensions.cs
│   │   └── CorsServiceExtensions.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── AutoTallerManager.API.csproj
│   └── Properties/
│       └── launchSettings.json

├── AutoTallerManager.Application/
│   ├── Abstractions/
│   │   ├── IUnitOfWork.cs
│   │   ├── IRepository.cs
│   │   └── IJwtService.cs
│   ├── DTOs/
│   │   ├── Request/
│   │   │   ├── ClienteRequest.cs
│   │   │   ├── VehiculoRequest.cs
│   │   │   ├── RepuestoRequest.cs
│   │   │   ├── OrdenServicioRequest.cs
│   │   │   └── FacturaRequest.cs
│   │   └── Response/
│   │       ├── ClienteResponse.cs
│   │       ├── VehiculoResponse.cs
│   │       ├── RepuestoResponse.cs
│   │       ├── OrdenServicioResponse.cs
│   │       └── FacturaResponse.cs
│   ├── Features/
│   │   ├── Clientes/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateClienteCommand.cs
│   │   │   │   ├── UpdateClienteCommand.cs
│   │   │   │   └── DeleteClienteCommand.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetClienteByIdQuery.cs
│   │   │   │   └── GetAllClientesQuery.cs
│   │   │   ├── Handlers/
│   │   │   │   ├── CreateClienteHandler.cs
│   │   │   │   └── GetAllClientesHandler.cs
│   │   │   └── Validators/
│   │   │       └── ClienteValidator.cs
│   │   ├── Vehiculos/
│   │   ├── Repuestos/
│   │   ├── OrdenesServicio/
│   │   ├── Facturas/
│   │   └── Auditorias/
│   ├── Common/
│   │   └── Behaviors/
│   │       ├── ValidationBehavior.cs
│   │       └── LoggingBehavior.cs
│   ├── Services/
│   │   └── JwtService.cs
│   └── AutoTallerManager.Application.csproj

├── AutoTallerManager.Domain/
│   ├── Entities/
│   │   ├── Pais.cs
│   │   ├── Departamento.cs
│   │   ├── Ciudad.cs
│   │   ├── Direccion.cs
│   │   ├── TipoCliente.cs
│   │   ├── Cliente.cs
│   │   ├── Marca.cs
│   │   ├── TipoVehiculo.cs
│   │   ├── Vehiculo.cs
│   │   ├── Repuesto.cs
│   │   ├── OrdenServicio.cs
│   │   ├── DetalleOrden.cs
│   │   ├── Factura.cs
│   │   ├── Usuario.cs
│   │   └── Auditoria.cs
│   ├── ValueObjects/
│   │   ├── Email.cs
│   │   ├── Documento.cs
│   │   ├── Dinero.cs
│   │   └── Telefono.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── IClienteRepository.cs
│   │   │   ├── IVehiculoRepository.cs
│   │   │   ├── IRepuestoRepository.cs
│   │   │   ├── IOrdenServicioRepository.cs
│   │   │   ├── IFacturaRepository.cs
│   │   │   └── IAuditoriaRepository.cs
│   │   └── Services/
│   │       └── IAuditoriaService.cs
│   ├── Exceptions/
│   │   └── DomainException.cs
│   └── AutoTallerManager.Domain.csproj

├── AutoTallerManager.Infrastructure/
│   ├── Persistence/
│   │   ├── Context/
│   │   │   └── AppDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── ClienteConfiguration.cs
│   │   │   ├── VehiculoConfiguration.cs
│   │   │   ├── RepuestoConfiguration.cs
│   │   │   ├── OrdenServicioConfiguration.cs
│   │   │   ├── DetalleOrdenConfiguration.cs
│   │   │   ├── FacturaConfiguration.cs
│   │   │   ├── UsuarioConfiguration.cs
│   │   │   └── AuditoriaConfiguration.cs
│   │   └── Scripts/
│   │       └── AutoTallerDb.sql
│   ├── Repositories/
│   │   ├── ClienteRepository.cs
│   │   ├── VehiculoRepository.cs
│   │   ├── RepuestoRepository.cs
│   │   ├── OrdenServicioRepository.cs
│   │   ├── FacturaRepository.cs
│   │   └── AuditoriaRepository.cs
│   ├── UnitOfWork/
│   │   └── UnitOfWork.cs
│   ├── Extensions/
│   │   └── InfrastructureServiceExtensions.cs
│   └── AutoTallerManager.Infrastructure.csproj

├── AutoTallerManager.Shared/
│   ├── Constants/
│   │   ├── Roles.cs
│   │   ├── EstadosOrden.cs
│   │   └── TiposCliente.cs
│   ├── Helpers/
│   │   ├── HashHelper.cs
│   │   └── Errors/
│   │       ├── ApiException.cs
│   │       ├── ApiResponse.cs
│   │       ├── ApiValidation.cs
│   │       └── ExceptionMiddleware.cs
│   └── AutoTallerManager.Shared.csproj

├── docker/
│   ├── docker-compose.yml
│   └── Api.Dockerfile

├── docs/
│   ├── AutoTallerDb_ERD.png
│   ├── ArquitecturaHexagonal.png
│   └── README_ARQ.md

├── tests/
│   ├── AutoTallerManager.Tests/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── API/
│   └── AutoTallerManager.IntegrationTests/
│       ├── Repository/
│       └── API/

├── README.md
└── LICENSE


```bash
//migracion final

cd AutoTallerManager.API
dotnet ef migrations add InitialCreate --project ../AutoTallerManager.Infrastructure --startup-project .
dotnet ef database update --project ../AutoTallerManager.Infrastructure --startup-project .

```