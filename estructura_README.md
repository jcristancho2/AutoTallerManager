## 🌳 ÁRBOL COMPLETO DEL PROYECTO

```
AutoTallerManager/
├── src/
│   ├── AutoTallerManager.Domain/
│   │   ├── Entities/
│   │   │   ├── Cliente.cs
│   │   │   ├── Vehiculo.cs
│   │   │   ├── OrdenServicio.cs
│   │   │   ├── Repuesto.cs
│   │   │   ├── DetalleOrden.cs
│   │   │   ├── Usuario.cs
│   │   │   ├── Factura.cs
│   │   │   └── Auditoria.cs
│   │   ├── Enums/
│   │   │   ├── TipoServicio.cs
│   │   │   ├── EstadoOrden.cs
│   │   │   ├── RolUsuario.cs
│   │   │   └── TipoAccionAuditoria.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── Telefono.cs
│   │   │   ├── Precio.cs
│   │   │   └── Sku.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IClienteRepository.cs
│   │   │   │   ├── IVehiculoRepository.cs
│   │   │   │   ├── IOrdenServicioRepository.cs
│   │   │   │   ├── IRepuestoRepository.cs
│   │   │   │   ├── IUsuarioRepository.cs
│   │   │   │   ├── IFacturaRepository.cs
│   │   │   │   └── IGenericRepository.cs
│   │   │   └── Services/
│   │   │       ├── IEmailService.cs
│   │   │       ├── IHashService.cs
│   │   │       ├── IAuditoriaService.cs
│   │   │       └── IJwtTokenService.cs
│   │   └── Exceptions/
│   │       ├── ClienteNotFoundException.cs
│   │       ├── VehiculoEnUsoException.cs
│   │       ├── StockInsuficienteException.cs
│   │       └── OrdenNoEncontradaException.cs
│   │
│   ├── AutoTallerManager.Application/
│   │   ├── DTOs/
│   │   │   ├── ClienteDto.cs
│   │   │   ├── VehiculoDto.cs
│   │   │   ├── OrdenServicioDto.cs
│   │   │   ├── RepuestoDto.cs
│   │   │   ├── UsuarioDto.cs
│   │   │   ├── FacturaDto.cs
│   │   │   └── Request/
│   │   │       ├── CrearClienteRequest.cs
│   │   │       ├── CrearOrdenServicioRequest.cs
│   │   │       ├── ActualizarOrdenRequest.cs
│   │   │       └── LoginRequest.cs
│   │   ├── Services/
│   │   │   ├── ClienteService.cs
│   │   │   ├── VehiculoService.cs
│   │   │   ├── OrdenServicioService.cs
│   │   │   ├── RepuestoService.cs
│   │   │   ├── UsuarioService.cs
│   │   │   ├── FacturaService.cs
│   │   │   └── AuthService.cs
│   │   ├── Interfaces/
│   │   │   ├── IClienteService.cs
│   │   │   ├── IVehiculoService.cs
│   │   │   ├── IOrdenServicioService.cs
│   │   │   ├── IRepuestoService.cs
│   │   │   ├── IUsuarioService.cs
│   │   │   ├── IFacturaService.cs
│   │   │   └── IAuthService.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   ├── Validators/
│   │   │   ├── ClienteValidator.cs
│   │   │   ├── VehiculoValidator.cs
│   │   │   ├── OrdenServicioValidator.cs
│   │   │   └── UsuarioValidator.cs
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── PerformanceBehavior.cs
│   │   ├── Common/
│   │   │   ├── PaginatedResult.cs
│   │   │   ├── ServiceResult.cs
│   │   │   └── Constants.cs
│   │   └── Extensions/
│   │       └── ApplicationServiceExtensions.cs
│   │
│   ├── AutoTallerManager.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Persistence/
│   │   │   │   └── AutoTallerDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── ClienteConfiguration.cs
│   │   │   │   ├── VehiculoConfiguration.cs
│   │   │   │   ├── OrdenServicioConfiguration.cs
│   │   │   │   ├── RepuestoConfiguration.cs
│   │   │   │   ├── UsuarioConfiguration.cs
│   │   │   │   └── FacturaConfiguration.cs
│   │   │   └── Migrations/
│   │   │       ├── 20240101000001_InitialCreate.cs
│   │   │       └── 20240101000002_AddAuditoriaTable.cs
│   │   ├── Repositories/
│   │   │   ├── GenericRepository.cs
│   │   │   ├── ClienteRepository.cs
│   │   │   ├── VehiculoRepository.cs
│   │   │   ├── OrdenServicioRepository.cs
│   │   │   ├── RepuestoRepository.cs
│   │   │   ├── UsuarioRepository.cs
│   │   │   └── FacturaRepository.cs
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   ├── HashService.cs
│   │   │   ├── AuditoriaService.cs
│   │   │   └── JwtTokenService.cs
│   │   ├── UnitOfWork/
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── Abstractions/
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── IAuditableEntity.cs
│   │   └── Extensions/
│   │       ├── ServiceCollectionExtensions.cs
│   │       └── DbContextExtensions.cs
│   │
│   ├── AutoTallerManager.API/
│   │   ├── Controllers/
│   │   │   ├── ClientesController.cs
│   │   │   ├── VehiculosController.cs
│   │   │   ├── OrdenesServicioController.cs
│   │   │   ├── RepuestosController.cs
│   │   │   ├── UsuariosController.cs
│   │   │   ├── FacturasController.cs
│   │   │   └── AuthController.cs
│   │   ├── Middleware/
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   ├── RateLimitingMiddleware.cs
│   │   │   └── AuditoriaMiddleware.cs
│   │   ├── Filters/
│   │   │   ├── ValidationFilter.cs
│   │   │   ├── AuthorizationFilter.cs
│   │   │   └── AuditFilter.cs
│   │   ├── Configuration/
│   │   │   ├── SwaggerConfiguration.cs
│   │   │   ├── RateLimitingConfiguration.cs
│   │   │   └── JwtConfiguration.cs
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── AutoTallerManager.Shared/
│       ├── Helpers/
│       │   ├── PasswordHelper.cs
│       │   ├── ValidationHelper.cs
│       │   ├── DateHelper.cs
│       │   └── StringExtensions.cs
│       ├── Constants/
│       │   ├── ApiConstants.cs
│       │   ├── ErrorMessages.cs
│       │   └── ValidationMessages.cs
│       ├── Models/
│       │   ├── ApiResponse.cs
│       │   ├── PaginationMetadata.cs
│       │   └── ErrorDetails.cs
│       └── Attributes/
│           ├── RequiredRoleAttribute.cs
│           └── AuditAttribute.cs
│

```

---

## 🚀 COMANDOS PARA CREAR LA ESTRUCTURA COMPLETA

### 📁 Crear Directorio Principal y Estructura Base

```bash
# Crear directorio principal del proyecto
mkdir AutoTallerManager
cd AutoTallerManager

# Crear estructura de carpetas principales
mkdir -p src tests docs scripts

# Crear proyectos de solución
mkdir -p src/AutoTallerManager.Domain
mkdir -p src/AutoTallerManager.Application  
mkdir -p src/AutoTallerManager.Infrastructure
mkdir -p src/AutoTallerManager.API
mkdir -p src/AutoTallerManager.Shared
```

### 🏛️ Crear Estructura Domain

```bash
# Domain - Entidades y lógica de negocio
mkdir -p src/AutoTallerManager.Domain/Entities
mkdir -p src/AutoTallerManager.Domain/Enums
mkdir -p src/AutoTallerManager.Domain/ValueObjects
mkdir -p src/AutoTallerManager.Domain/Interfaces/Repositories
mkdir -p src/AutoTallerManager.Domain/Interfaces/Services
mkdir -p src/AutoTallerManager.Domain/Exceptions
```

### 🔧 Crear Estructura Application

```bash
# Application - Casos de uso y servicios
mkdir -p src/AutoTallerManager.Application/DTOs/Request
mkdir -p src/AutoTallerManager.Application/Services
mkdir -p src/AutoTallerManager.Application/Interfaces
mkdir -p src/AutoTallerManager.Application/Mappings
mkdir -p src/AutoTallerManager.Application/Validators
mkdir -p src/AutoTallerManager.Application/Behaviors
mkdir -p src/AutoTallerManager.Application/Common
mkdir -p src/AutoTallerManager.Application/Extensions
```

### 🔌 Crear Estructura Infrastructure

```bash
# Infrastructure - Persistencia y servicios técnicos
mkdir -p src/AutoTallerManager.Infrastructure/Data/Persistence
mkdir -p src/AutoTallerManager.Infrastructure/Data/Configurations
mkdir -p src/AutoTallerManager.Infrastructure/Data/Migrations
mkdir -p src/AutoTallerManager.Infrastructure/Repositories
mkdir -p src/AutoTallerManager.Infrastructure/Services
mkdir -p src/AutoTallerManager.Infrastructure/UnitOfWork
mkdir -p src/AutoTallerManager.Infrastructure/Abstractions
mkdir -p src/AutoTallerManager.Infrastructure/Extensions
```

### 🌐 Crear Estructura API

```bash
# API - Controladores y middleware
mkdir -p src/AutoTallerManager.API/Controllers
mkdir -p src/AutoTallerManager.API/Middleware
mkdir -p src/AutoTallerManager.API/Filters
mkdir -p src/AutoTallerManager.API/Configuration
mkdir -p src/AutoTallerManager.API/Extensions
```

### 🔧 Crear Estructura Shared

```bash
# Shared - Utilidades comunes
mkdir -p src/AutoTallerManager.Shared/Helpers
mkdir -p src/AutoTallerManager.Shared/Constants
mkdir -p src/AutoTallerManager.Shared/Models
mkdir -p src/AutoTallerManager.Shared/Attributes
```

## 🎯 Estructura de Capas

### 1. 🏛️ **Domain** (Núcleo de Negocio)

**Ubicación:** `src/AutoTallerManager.Domain/`

```
Domain/
├── Entities/                    # Entidades de dominio
│   ├── Cliente.cs
│   ├── Vehiculo.cs
│   ├── OrdenServicio.cs
│   ├── Repuesto.cs
│   ├── DetalleOrden.cs
│   ├── Usuario.cs
│   ├── Factura.cs
│   └── Auditoria.cs
├── Enums/                       # Enumeraciones
│   ├── TipoServicio.cs
│   ├── EstadoOrden.cs
│   ├── RolUsuario.cs
│   └── TipoAccionAuditoria.cs
├── ValueObjects/                # Objetos de valor
│   ├── Email.cs
│   ├── Telefono.cs
│   └── Precio.cs
├── Interfaces/                  # Puertos (Interfaces)
│   ├── Repositories/
│   │   ├── IClienteRepository.cs
│   │   ├── IVehiculoRepository.cs
│   │   ├── IOrdenServicioRepository.cs
│   │   ├── IRepuestoRepository.cs
│   │   ├── IUsuarioRepository.cs
│   │   └── IFacturaRepository.cs
│   └── Services/
│       ├── IEmailService.cs
│       ├── IHashService.cs
│       └── IAuditoriaService.cs
└── Exceptions/                  # Excepciones de dominio
    ├── ClienteNotFoundException.cs
    ├── VehiculoEnUsoException.cs
    ├── StockInsuficienteException.cs
    └── OrdenNoEncontradaException.cs
```

**Responsabilidades:**

- ✅ Entidades de negocio con lógica de dominio
- ✅ Reglas de validación y restricciones
- ✅ Interfaces (Puertos) para servicios externos
- ✅ Excepciones específicas del dominio

---

### 2. 🔧 **Application** (Casos de Uso)

**Ubicación:** `src/AutoTallerManager.Application/`

```
Application/
├── DTOs/                        # Data Transfer Objects
│   ├── ClienteDto.cs
│   ├── VehiculoDto.cs
│   ├── OrdenServicioDto.cs
│   ├── RepuestoDto.cs
│   ├── UsuarioDto.cs
│   ├── FacturaDto.cs
│   └── Request/
│       ├── CrearClienteRequest.cs
│       ├── CrearOrdenServicioRequest.cs
│       └── ActualizarOrdenRequest.cs
├── Services/                    # Servicios de aplicación (Casos de uso)
│   ├── ClienteService.cs
│   ├── VehiculoService.cs
│   ├── OrdenServicioService.cs
│   ├── RepuestoService.cs
│   ├── UsuarioService.cs
│   ├── FacturaService.cs
│   └── AuthService.cs
├── Interfaces/                  # Interfaces de servicios
│   ├── IClienteService.cs
│   ├── IVehiculoService.cs
│   ├── IOrdenServicioService.cs
│   ├── IRepuestoService.cs
│   ├── IUsuarioService.cs
│   ├── IFacturaService.cs
│   └── IAuthService.cs
├── Mappings/                    # Configuración AutoMapper
│   └── MappingProfile.cs
├── Validators/                  # Validaciones con FluentValidation
│   ├── ClienteValidator.cs
│   ├── VehiculoValidator.cs
│   ├── OrdenServicioValidator.cs
│   └── UsuarioValidator.cs
└── Common/                      # Utilidades comunes
    ├── PaginatedResult.cs
    ├── ServiceResult.cs
    └── Constants.cs
```

**Responsabilidades:**

- ✅ Casos de uso específicos del negocio
- ✅ DTOs para transferencia de datos
- ✅ Mapeo entre entidades y DTOs
- ✅ Validaciones de entrada
- ✅ Orquestación de operaciones complejas

---

### 3. 🔌 **Infrastructure** (Adaptadores)

**Ubicación:** `src/AutoTallerManager.Infrastructure/`

```
Infrastructure/
├── Data/                        # Persistencia de datos
│   ├── AutoTallerDbContext.cs
│   ├── Configurations/          # Configuraciones EF Core
│   │   ├── ClienteConfiguration.cs
│   │   ├── VehiculoConfiguration.cs
│   │   ├── OrdenServicioConfiguration.cs
│   │   ├── RepuestoConfiguration.cs
│   │   ├── UsuarioConfiguration.cs
│   │   └── FacturaConfiguration.cs
│   └── Migrations/              # Migraciones EF Core
│       ├── 20240101000001_InitialCreate.cs
│       └── 20240101000002_AddAuditoriaTable.cs
├── Repositories/                # Implementación de repositorios
│   ├── GenericRepository.cs
│   ├── ClienteRepository.cs
│   ├── VehiculoRepository.cs
│   ├── OrdenServicioRepository.cs
│   ├── RepuestoRepository.cs
│   ├── UsuarioRepository.cs
│   └── FacturaRepository.cs
├── Services/                    # Servicios de infraestructura
│   ├── EmailService.cs
│   ├── HashService.cs
│   ├── AuditoriaService.cs
│   └── JwtTokenService.cs
├── UnitOfWork/                  # Patrón Unit of Work
│   ├── UnitOfWork.cs
│   └── IUnitOfWork.cs
└── Extensions/                  # Extensiones de configuración
    ├── ServiceCollectionExtensions.cs
    └── DbContextExtensions.cs
```

**Responsabilidades:**

- ✅ Persistencia con Entity Framework Core
- ✅ Implementación de repositorios
- ✅ Servicios técnicos (Email, Hash, JWT)
- ✅ Configuración de base de datos
- ✅ Migraciones y esquemas

---

### 4. 🌐 **API** (Capa de Presentación)

**Ubicación:** `src/AutoTallerManager.API/`

```
API/
├── Controllers/                 # Controladores RESTful
│   ├── ClientesController.cs
│   ├── VehiculosController.cs
│   ├── OrdenesServicioController.cs
│   ├── RepuestosController.cs
│   ├── UsuariosController.cs
│   ├── FacturasController.cs
│   └── AuthController.cs
├── Middleware/                  # Middleware personalizado
│   ├── ErrorHandlingMiddleware.cs
│   ├── RateLimitingMiddleware.cs
│   └── AuditoriaMiddleware.cs
├── Filters/                     # Filtros de acción
│   ├── ValidationFilter.cs
│   ├── AuthorizationFilter.cs
│   └── AuditFilter.cs
├── Configuration/               # Configuración de servicios
│   ├── SwaggerConfiguration.cs
│   ├── RateLimitingConfiguration.cs
│   └── JwtConfiguration.cs
├── Extensions/                  # Extensiones de configuración
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
├── Program.cs                   # Punto de entrada
└── appsettings.json            # Configuración de la aplicación
```

**Responsabilidades:**

- ✅ Exposición de endpoints RESTful
- ✅ Autenticación y autorización JWT
- ✅ Rate Limiting y control de carga
- ✅ Documentación Swagger/OpenAPI
- ✅ Manejo de errores y middleware

---

### 5. 🔧 **Shared** (Utilidades Comunes)

**Ubicación:** `src/AutoTallerManager.Shared/`

```
Shared/
├── Helpers/                     # Utilidades comunes
│   ├── PasswordHelper.cs
│   ├── ValidationHelper.cs
│   ├── DateHelper.cs
│   └── StringExtensions.cs
├── Constants/                   # Constantes del sistema
│   ├── ApiConstants.cs
│   ├── ErrorMessages.cs
│   └── ValidationMessages.cs
├── Models/                      # Modelos compartidos
│   ├── ApiResponse.cs
│   ├── PaginationMetadata.cs
│   └── ErrorDetails.cs
└── Attributes/                  # Atributos personalizados
    ├── RequiredRoleAttribute.cs
    └── AuditAttribute.cs
```

**Responsabilidades:**

- ✅ Utilidades comunes reutilizables
- ✅ Constantes y mensajes
- ✅ Modelos compartidos entre capas
- ✅ Atributos personalizados

---

## 🔄 Flujo de Dependencias

```
API Layer
    ↓ (depende de)
Application Layer
    ↓ (depende de)
Domain Layer
    ↑ (implementado por)
Infrastructure Layer
    ↑ (utiliza)
Shared Layer
```

## 📦 Principios Aplicados

- **🔒 Inversión de Dependencias:** Las capas internas no dependen de las externas
- **🎯 Separación de Responsabilidades:** Cada capa tiene una responsabilidad específica
- **🔄 Ports & Adapters:** Interfaces (puertos) y implementaciones (adaptadores)
- **📈 Escalabilidad:** Arquitectura preparada para crecimiento

---

## 🚀 Tecnologías y Patrones

| Capa                     | Tecnologías                 | Patrones                       |
| ------------------------ | ---------------------------- | ------------------------------ |
| **Domain**         | C# POCOs, Enums              | Domain Entities, Value Objects |
| **Application**    | AutoMapper, FluentValidation | CQRS, DTOs, Services           |
| **Infrastructure** | EF Core, MySQL, JWT          | Repository, Unit of Work       |
| **API**            | ASP.NET Core, Swagger        | RESTful, Middleware            |
| **Shared**         | C# Extensions                | Helpers, Constants             |



---
---
---



``` bash
# NOTA: Asegúrate de estar en el directorio de la solución o del proyecto.

# 1. Navegar al directorio del proyecto API
cd AutoTallerManager.API

# 2. ELIMINAR la dependencia de MySQL
dotnet remove package Pomelo.EntityFrameworkCore.MySql

# 3. AGREGAR la dependencia de PostgreSQL (Npgsql)
# Usando la versión compatible con .NET 8.
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4

# 4. AGREGAR el resto de paquetes que necesitabas:

# AutoMapper para mapeo de objetos
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# JWT Authentication para seguridad de la API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Rate Limiting para proteger la API
dotnet add package AspNetCoreRateLimit

# Swagger / OpenAPI para documentación
dotnet add package Swashbuckle.AspNetCore

```

``` bash
# infraestructura 
cd ../AutoTallerManager.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```