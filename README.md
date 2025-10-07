## 🚀 AutoTallerManager Backend API

## Sistema de Gestión Integral de Taller Automotriz

![Lenguaje](https://img.shields.io/badge/Tecnología-ASP.NET%20Core%206.0+-512BD4?style=for-the-badge&logo=dotnet)![Arquitectura](https://img.shields.io/badge/Arquitectura-Hexagonal%20(Ports%20&%20Adapters)-007ACC?style=for-the-badge)![Base de Datos](https://img.shields.io/badge/Base%20de%20Datos-MySQL%20/%20EF%20Core-00758F?style=for-the-badge&logo=mysql)![Estado](https://img.shields.io/badge/Estado-Pendiente-28A745?style=for-the-badge)![Horas-Proyecto](https://img.shields.io/badge/Tiempo%20Desarrollo-178%20Hrs-FE7A16?style=for-the-badge)

---

AutoTallerManager es un **backend RESTful** robusto diseñado para gestionar de forma integral las operaciones de un taller automotriz moderno. El sistema centraliza y automatiza procesos clave como la gestión de **clientes**, **vehículos**, **órdenes de servicio**, **inventario de repuestos** y **facturación**.

Implementado sobre **ASP.NET Core**, el proyecto sigue estrictamente el patrón de **Arquitectura Hexagonal (Ports & Adapters)**, garantizando una alta **mantenibilidad**, **escalabilidad** y una clara separación de preocupaciones.

### Características Sobresalientes

- **Seguridad por Roles:** Autenticación **JWT** con Autorización diferenciada para **Admin**, **Mecánico** y **Recepcionista**.
- **Control de Tráfico (Rate Limiting):** Reglas configuradas para proteger rutas críticas (e.g., 60 solicitudes/min en `/api/ordenesservicio`).
- **Trazabilidad:** Módulo de **Auditoría** para registrar todas las operaciones CRUD importantes.
- **Gestión de Inventario en Tiempo Real:** Validación de stock antes de asignar repuestos a una orden de servicio.
- **Documentación Interactiva:** **Swagger / OpenAPI** completamente configurado para testing e integración.

---

## 🧱 Arquitectura: Hexagonal (Ports & Adapters)

El diseño del proyecto se divide en cuatro capas lógicas para asegurar un alto *cohesion* y bajo *acoplamiento*:

1. **Capa de Dominio:** Contiene las **Entidades de Negocio** (Cliente, Vehículo, OrdenServicio, Repuesto) y la **Lógica de Negocio** pura (validaciones, cálculo de costos y fechas).
2. **Capa de Aplicación:** Define los **Casos de Uso** (servicios de aplicación) y utiliza **DTOs** para la transferencia de datos. Se emplea **AutoMapper** para la conversión entre Entidades y DTOs.
3. **Capa de Infraestructura:** Implementa los "Adaptadores" para la persistencia de datos. Utiliza **Entity Framework Core** con **Fluent API** para mapear a **MySQL**. Incluye el **Repository Pattern Genérico** y el **Unit of Work**.
4. **Capa de API:** El "Adaptador" de entrada que expone los **Controladores RESTful**, maneja la **Autenticación JWT** y el **Rate Limiting**.

---

## ⚙️ Entidades Principales

Las entidades reflejan los procesos centrales del taller:

| Entidad                 | Descripción                                               | Relaciones Clave                                |
| :---------------------- | :--------------------------------------------------------- | :---------------------------------------------- |
| **Cliente**       | Propietario con datos de contacto.                         | $\text{1 : N}$ con Vehículo y OrdenServicio. |
| **Vehículo**     | Datos técnicos (VIN, Marca, Kilometraje).                 | $\text{1 : N}$ con OrdenServicio.             |
| **OrdenServicio** | Solicitud de trabajo, registra mecánico, fechas y estado. | $\text{1 : N}$ con DetalleOrden.              |
| **Repuesto**      | Piezas en inventario (Stock, Precio).                      | Relación con DetalleOrden.                     |
| **Factura**       | Documento final generado al cerrar una orden.              | $\text{1 : 1}$ con OrdenServicio.             |

---

## 🛠️ Instalación y Puesta en Marcha

### Prerrequisitos

* [.NET SDK 6.0 o superior](https://dotnet.microsoft.com/download)
* **Postgres** (Configurado para la persistencia de datos).
* [Git](https://git-scm.com/)

### Pasos

1. **Clonar el Repositorio:**

   ```bash
   git clone [https://github.com/tu-usuario/AutoTallerManager.git](https://github.com/tu-usuario/AutoTallerManager.git)
   cd AutoTallerManager
   ```
2. **Configurar Conexión:**
   Actualiza la cadena de conexión de MySQL en el archivo `appsettings.json`.
3. **Aplicar Migraciones de Base de Datos:**
   Utiliza Entity Framework Core para crear el esquema:

   ```bash
   # Asegúrate de estar en el directorio del proyecto que contiene el .csproj
   dotnet ef database update
   ```

   *Nota: Las migraciones como `InitialCreate`, `AddRepuestosTable`, etc., ya están definidas con Fluent API para optimizar el esquema.*
4. **Ejecutar la API:**

   ```bash
   dotnet run
   ```

   La API estará disponible en **`http://localhost:5000`** (o el puerto configurado).

---

## 🔑 Autenticación, Autorización y Seguridad

La seguridad es gestionada por **JWT Bearer Token** y políticas de autorización por roles.

### Roles del Sistema

| Rol                     | Permisos Clave                                                                                    |
| :---------------------- | :------------------------------------------------------------------------------------------------ |
| **Admin**         | Acceso**total**. Gestión de usuarios, inventario (CRUD completo) y configuración.         |
| **Mecánico**     | Actualización de**órdenes** (avance de trabajo), generación de **facturas**.       |
| **Recepcionista** | Creación de**órdenes de servicio**, consulta y gestión de **clientes/vehículos**. |

### Documentación Swagger / OpenAPI

Accede a la documentación interactiva en:

$$
\text{http://localhost:5000/swagger}
$$

Utiliza el botón **Authorize** e introduce el token JWT (prefijo `Bearer `) para probar los endpoints protegidos por los roles.

---

## 👥 Desarrolladores Principales ✨

Agradecemos a los siguientes contribuidores por su trabajo en este proyecto:

* 👩‍💻 [LEIDY JOHANA NIÑO](https://github.com/LeidyJohanaVillegas)
* 👨‍💻 [EDUARDO ELIAS CASTELLANOS PICON](https://github.com/EduardoCastellanosP)
* 👩‍💼 [ESTHER SAMUR](https://github.com/sheyla08samur)
* 👨‍🔧 [JORGE ANDRES CRISTANCHO OLARTE](https://github.com/jcristancho2)

---

## 📜 Licencia

Este proyecto está liberado bajo la **Licencia MIT**. Consulta el archivo `LICENSE` para más detalles.
