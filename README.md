# 🚀 AutoTallerManager Backend API

## Sistema de Gestión Integral de Taller Automotriz

![Lenguaje](https://img.shields.io/badge/Tecnología-ASP.NET%20Core%209.0+-512BD4?style=for-the-badge&logo=dotnet)
![Arquitectura](https://img.shields.io/badge/Arquitectura-Hexagonal%20(Ports%20&%20Adapters)-007ACC?style=for-the-badge)
![Base de Datos](https://img.shields.io/badge/Base%20de%20Datos-MySQL%20/%20EF%20Core-00758F?style=for-the-badge&logo=mysql)
![Estado](https://img.shields.io/badge/Estado-En%20Desarrollo-28A745?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-xUnit-FF6B6B?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-Disponible-2496ED?style=for-the-badge&logo=docker)

---

## 📋 Tabla de Contenidos

- [🚀 AutoTallerManager Backend API](#-autotallermanager-backend-api)
  - [📋 Tabla de Contenidos](#-tabla-de-contenidos)
  - [📖 Descripción](#-descripción)
  - [✨ Características Principales](#-características-principales)
  - [🏗️ Arquitectura](#️-arquitectura)
  - [🗄️ Modelo de Datos](#️-modelo-de-datos)
  - [🛠️ Tecnologías](#️-tecnologías)
  - [🚀 Inicio Rápido](#-inicio-rápido)
  - [🐳 Docker](#-docker)
  - [📚 Documentación de la API](#-documentación-de-la-api)
  - [🔐 Autenticación y Autorización](#-autenticación-y-autorización)
  - [🧪 Testing](#-testing)
  - [📁 Estructura del Proyecto](#-estructura-del-proyecto)
  - [🤝 Contribución](#-contribución)
  - [👥 Desarrolladores](#-desarrolladores)
  - [📄 Licencia](#-licencia)

---

## 📖 Descripción

**AutoTallerManager** es un sistema backend RESTful completo para la gestión integral de talleres automotrices. Desarrollado con **ASP.NET Core 9.0** siguiendo los principios de **Clean Architecture** y el patrón **Hexagonal (Ports & Adapters)**.

El sistema automatiza y centraliza todos los procesos críticos del taller:
- 👥 Gestión de clientes y contactos
- 🚗 Registro y seguimiento de vehículos
- 📋 Órdenes de servicio y seguimiento
- 📦 Control de inventario de repuestos
- 💰 Facturación automatizada
- 🔍 Auditoría completa de operaciones

---

## ✨ Características Principales

### 🔒 Seguridad Avanzada
- **Autenticación JWT** con refresh tokens
- **Autorización basada en roles** (Admin, Mecánico, Recepcionista)
- **Rate Limiting** personalizable por endpoint
- **Middleware de auditoría** para trazabilidad completa

### 🏗️ Arquitectura Robusta
- **Clean Architecture** con separación clara de responsabilidades
- **CQRS** con MediatR para commands y queries
- **Repository Pattern** con Unit of Work
- **Validation Pipeline** con FluentValidation

### 📊 Gestión Inteligente
- **Validación de disponibilidad** de vehículos en tiempo real
- **Cálculo automático** de fechas de entrega
- **Control de stock** antes de asignar repuestos
- **Generación automática** de facturas

### 🔧 Herramientas de Desarrollo
- **Swagger/OpenAPI** para documentación interactiva
- **Docker** ready con docker-compose
- **Tests automatizados** con cobertura completa
- **Hot reload** para desarrollo ágil

---

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture** con **patrón Hexagonal**:

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                            │
│  Controllers, Middleware, Authentication, Swagger      │
├─────────────────────────────────────────────────────────┤
│                 Application Layer                       │
│    Commands, Queries, DTOs, Validators, MediatR        │
├─────────────────────────────────────────────────────────┤
│                  Domain Layer                           │
│       Entities, Value Objects, Business Logic          │
├─────────────────────────────────────────────────────────┤
│              Infrastructure Layer                       │
│   EF Core, Repositories, External Services, Config     │
└─────────────────────────────────────────────────────────┘
```

### Principios Aplicados
- **Inversión de dependencias**: Las capas superiores no dependen de las inferiores
- **Separación de preocupaciones**: Cada capa tiene una responsabilidad específica
- **Testabilidad**: Interfaces mockeable en todos los niveles
- **Mantenibilidad**: Código limpio y bien estructurado

---

## 🗄️ Modelo de Datos

### Entidades Principales

| Entidad | Descripción | Campos Clave |
|---------|-------------|--------------|
| **Cliente** | Información del propietario | Nombre, Email, Teléfono, Dirección |
| **Vehículo** | Datos técnicos del vehículo | VIN, Placa, Marca, Modelo, Año |
| **OrdenServicio** | Solicitud de trabajo | Fecha, Estado, Descripción, Mecánico |
| **Repuesto** | Inventario de piezas | SKU, Nombre, Stock, Precio |
| **Factura** | Documento de cobro | Total, Impuestos, Fecha, Estado |

### Relaciones Clave
```
Cliente (1) ──→ (N) Vehículo
Cliente (1) ──→ (N) OrdenServicio
Vehículo (1) ──→ (N) OrdenServicio
OrdenServicio (1) ──→ (N) DetalleOrden
Repuesto (1) ──→ (N) DetalleOrden
OrdenServicio (1) ──→ (1) Factura
```

---

## 🛠️ Tecnologías

### Backend
- **ASP.NET Core 9.0** - Framework web
- **Entity Framework Core** - ORM
- **MediatR** - Patrón CQRS/Mediator
- **AutoMapper** - Mapeo objeto-objeto
- **FluentValidation** - Validaciones
- **Serilog** - Logging estructurado

### Base de Datos
- **MySQL 8.0+** - Base de datos principal
- **Entity Framework Migrations** - Control de versiones DB

### Seguridad
- **JWT Bearer Authentication** - Tokens de acceso
- **ASP.NET Core Identity** - Gestión de usuarios
- **Rate Limiting** - Control de tráfico

### Testing
- **xUnit** - Framework de pruebas
- **Moq** - Mocking
- **Microsoft.AspNetCore.Mvc.Testing** - Integration tests
- **Entity Framework InMemory** - Tests de BD

### DevOps
- **Docker** - Containerización
- **Docker Compose** - Orquestación local
- **Swagger/OpenAPI** - Documentación

---

## 🚀 Inicio Rápido

### Prerrequisitos

```bash
# Verificar versión de .NET
dotnet --version  # Requiere .NET 9.0+

# Verificar Docker (opcional)
docker --version
docker compose --version
```

### Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/jcristancho2/AutoTallerManager.git
   cd AutoTallerManager
   ```

2. **Configurar la base de datos**
   - Edita la cadena de conexión en el archivo `appsettings.json` ubicado en el proyecto `AutoTallerManager.API`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=AutoTallerManager;User=root;Password=tu-contraseña;"
     }
     ```

3. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

4. **Crear y aplicar migraciones**
   - Navega al directorio del proyecto API:
     ```bash
     cd AutoTallerManager.API
     ```
   - Crea las migraciones (si no existen):
     ```bash
     dotnet ef migrations add InitialCreate
     ```
   - Aplica las migraciones a la base de datos:
     ```bash
     dotnet ef database update
     ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a la API**
   - API: `https://localhost:7000`
   - Swagger: `https://localhost:7000/swagger`

---

## 🐳 Docker

### Desarrollo Local

```bash
# Levantar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f autotallermanager-api

# Detener servicios
docker-compose down
```

### Solo Base de Datos
```bash
# Solo MySQL para desarrollo local
docker-compose up -d mysql
```

---

## 📚 Documentación de la API

### Swagger/OpenAPI
La documentación interactiva está disponible en:
- **Desarrollo**: `https://localhost:7000/swagger`
- **Producción**: `https://tu-dominio.com/swagger`

### Endpoints Principales

| Recurso | Método | Endpoint | Descripción |
|---------|--------|----------|-------------|
| **Auth** | POST | `/api/auth/login` | Iniciar sesión |
| **Auth** | POST | `/api/auth/register` | Registrar usuario |
| **Clientes** | GET | `/api/clientes` | Listar clientes |
| **Clientes** | POST | `/api/clientes` | Crear cliente |
| **Vehículos** | GET | `/api/vehiculos` | Listar vehículos |
| **Órdenes** | GET | `/api/ordenesservicio` | Listar órdenes |
| **Órdenes** | POST | `/api/ordenesservicio` | Crear orden |
| **Repuestos** | GET | `/api/repuestos` | Inventario |
| **Facturas** | POST | `/api/facturas` | Generar factura |

### Ejemplo de Uso

```bash
# Obtener token
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@taller.com",
    "password": "Admin123!"
  }'

# Usar token en requests
curl -X GET "https://localhost:7000/api/clientes" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 🔐 Autenticación y Autorización

### Roles del Sistema

| Rol | Permisos | Endpoints Permitidos |
|-----|----------|---------------------|
| **Admin** | Control total del sistema | Todos los endpoints |
| **Mecánico** | Gestión de órdenes y facturas | `/ordenesservicio/*`, `/facturas/*` |
| **Recepcionista** | Atención al cliente | `/clientes/*`, `/vehiculos/*`, `/ordenesservicio` (lectura) |

### Flujo de Autenticación

1. **Login** → Obtener JWT token + Refresh token
2. **Request** → Incluir `Authorization: Bearer {token}`
3. **Refresh** → Renovar token antes de expiración
4. **Logout** → Invalidar tokens

### Configuración JWT

```json
{
  "JWT": {
    "Key": "tu-clave-secreta-super-segura",
    "Issuer": "AutoTallerManager",
    "Audience": "AutoTallerManager-Users",
    "DurationInMinutes": 60,
    "RefreshTokenDurationInDays": 7
  }
}
```

---

## 🧪 Testing

### Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Tests específicos
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=Unit"
```

### Tipos de Tests

- **Unit Tests**: Lógica de negocio y servicios
- **Integration Tests**: Endpoints y base de datos
- **Repository Tests**: Acceso a datos
- **Validation Tests**: Reglas de negocio

### Cobertura Actual
- **Controladores**: 85%+
- **Servicios**: 90%+
- **Repositorios**: 80%+
- **Validadores**: 95%+

---

## 📁 Estructura del Proyecto

```
AutoTallerManager/
├── 📁 AutoTallerManager.API/          # Capa de presentación
│   ├── 📁 Controllers/                # Controladores REST
│   ├── 📁 DTOs/                      # Data Transfer Objects
│   ├── 📁 Middleware/                # Middleware personalizado
│   ├── 📁 Extensions/                # Extensions de servicios
│   └── 📄 Program.cs                 # Punto de entrada
├── 📁 AutoTallerManager.Application/  # Capa de aplicación
│   ├── 📁 Features/                  # Commands y Queries (CQRS)
│   ├── 📁 Services/                  # Servicios de aplicación
│   ├── 📁 Validators/                # Validadores FluentValidation
│   └── 📁 Common/                    # DTOs y mapeos comunes
├── 📁 AutoTallerManager.Domain/       # Capa de dominio
│   ├── 📁 Entities/                  # Entidades de negocio
│   ├── 📁 ValueObjects/              # Objetos de valor
│   └── 📁 Enums/                     # Enumeraciones
├── 📁 AutoTallerManager.Infrastructure/ # Capa de infraestructura
│   ├── 📁 Persistence/               # Entity Framework
│   ├── 📁 Repositories/              # Implementación repositorios
│   ├── 📁 Configurations/            # Configuración EF
│   └── 📁 Migrations/                # Migraciones de BD
├── 📁 AutoTallerManager.Tests/        # Proyecto de tests
│   ├── 📁 Unit/                      # Tests unitarios
│   ├── 📁 Integration/               # Tests de integración
│   └── 📁 Helpers/                   # Utilities para tests
└── 📁 docker/                        # Configuración Docker
    └── 📄 docker-compose.yml
```

---

## 🤝 Contribución

### Proceso de Contribución

1. **Fork** del repositorio
2. **Crear branch** para tu feature: `git checkout -b feature/nueva-funcionalidad`
3. **Commit** de cambios: `git commit -m 'Add: nueva funcionalidad'`
4. **Push** al branch: `git push origin feature/nueva-funcionalidad`
5. **Crear Pull Request**

### Estándares de Código

- **Convención de nombres**: PascalCase para clases, camelCase para variables
- **Documentación**: XML comments en métodos públicos
- **Tests**: Cobertura mínima del 80%
- **Commits**: Conventional Commits format

### Issues y Bugs

Para reportar bugs o solicitar features, usa nuestro [sistema de issues](https://github.com/jcristancho2/AutoTallerManager/issues).

**Template de Bug:**
```markdown
## 🐛 Descripción del Bug
[Descripción clara del problema]

## 🔄 Pasos para Reproducir
1. [Paso 1]
2. [Paso 2]

## ✅ Comportamiento Esperado
[Lo que debería pasar]

## ❌ Comportamiento Actual
[Lo que está pasando]

## 🖥️ Entorno
- OS: [Ubuntu 22.04]
- .NET: [9.0]
- Browser: [Chrome 120]
```

---

## 👥 Desarrolladores

Agradecemos a todos los contribuidores que han hecho posible este proyecto:

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/LeidyJohanaVillegas">
        <img src="https://github.com/LeidyJohanaVillegas.png" width="100px;" alt="Leidy Johana"/>
        <br />
        <sub><b>Leidy Johana Niño</b></sub>
      </a>
      <br />
      <sub>Testing & Backend </sub>
    </td>
    <td align="center">
      <a href="https://github.com/EduardoCastellanosP">
        <img src="https://github.com/EduardoCastellanosP.png" width="100px;" alt="Eduardo"/>
        <br />
        <sub><b>Eduardo Castellanos</b></sub>
      </a>
      <br />
      <sub> Backend </sub>
    </td>
    <td align="center">
      <a href="https://github.com/sheyla08samur">
        <img src="https://github.com/sheyla08samur.png" width="100px;" alt="Esther"/>
        <br />
        <sub><b>Esther Samur</b></sub>
      </a>
      <br />
      <sub> Business Logic & Fullstack & UX </sub>
    </td>
    <td align="center">
      <a href="https://github.com/jcristancho2">
        <img src="https://github.com/jcristancho2.png" width="100px;" alt="Jorge"/>
        <br />
        <sub><b>Jorge Cristancho</b></sub>
      </a>
      <br />
      <sub> DevOps & Architecture</sub>
    </td>
  </tr>
</table>

### Estadísticas del Proyecto
- **Commits**: 500+
- **Horas de desarrollo**: 178+ hrs
- **Lines of Code**: 15,000+
- **Test Coverage**: 85%

---

## 📄 Licencia

Este proyecto está licenciado bajo la **MIT License** - ver el archivo [LICENSE](LICENSE) para más detalles.

```
MIT License - Copyright (c) 2025 AutoTallerManager Team
```

### ¿Por qué MIT?
- ✅ **Uso comercial** permitido
- ✅ **Modificación** permitida  
- ✅ **Distribución** permitida
- ✅ **Uso privado** permitido
- ❌ **Sin garantía** - uso bajo tu propio riesgo

---

## 📞 Soporte

¿Necesitas ayuda? Estamos aquí para apoyarte:

- 📧 **Email**: support@autotallermanager.com
- 🐛 **Issues**: [GitHub Issues](https://github.com/jcristancho2/AutoTallerManager/issues)
- 📖 **Wiki**: [Documentación completa](https://github.com/jcristancho2/AutoTallerManager/wiki)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/jcristancho2/AutoTallerManager/discussions)

---

<div align="center">

**⭐ Si este proyecto te ayuda, considera darle una estrella ⭐**

Hecho con ❤️ por el equipo J2

</div>
