# Proyecto #1


## Sistema de Gestión de Taller Automotriz Backend

### DESCRIPCION

    AutoTallerManager es un backend RESTful diseñado para cubrir de forma integral las operaciones de un taller automotriz moderno. El propósito es centralizar y automatizar procesos clave como la gestión de clientes, vehículos, órdenes de servicio, repuestos y facturación, garantizando la trazabilidad de cada actividad y optimizando el flujo de trabajo de mecánicos, recepcionistas y administradores.


    AutoTallerManager se implementa sobre ASP.NET Core, aprovechando la robustez y eficiencia de este framework para ofrecer servicios web de alto rendimiento. La arquitectura interna sigue el patrón de arquitectura hexagonal (Ports & Adapters), con cuatro capas diferenciadas:

    Capa de Dominio

        Define las entidades esenciales del negocio:
        Cliente: propietario de uno o varios vehículos, con datos de contacto (nombre, teléfono, correo).
        Vehículo: asociado a un cliente; registra datos como marca, modelo, año, número de serie (VIN) y kilometraje.
        OrdenServicio: representa una solicitud de trabajo que incluye referencia al vehículo, tipo de servicio (mantenimiento preventivo, reparación, diagnóstico), mecánico asignado, fecha de ingreso y fecha estimada de entrega.
        Repuesto: piezas o insumos necesarios para ejecutar una orden; contiene código, descripción, cantidad en stock y precio unitario.
        DetalleOrden: relación entre OrdenServicio y Repuesto, indicando cantidades y costo de cada pieza utilizada.
        Usuario: persona que utiliza el sistema (con roles como “Admin”, “Mecánico” o “Recepcionista”); almacena credenciales (correo y contraseña hasheada) y rol asignado.
        Factura: documento generado al cerrar una orden, con resumen de servicios, repuestos utilizados, mano de obra y monto total.
        Dentro de esta capa reside la lógica de negocio, como validar que un vehículo no esté agendado en dos órdenes simultáneas, calcular fechas estimadas según tipo de servicio y reglas de inventario que impiden usar repuestos fuera de stock.

    Capa de Aplicación

        Contiene DTOs (Data Transfer Objects) para cada entidad: ClienteDto, VehiculoDto, OrdenServicioDto, RepuestoDto, DetalleOrdenDto, FacturaDto y UsuarioDto. Estos DTOs aseguran que solo los campos necesarios se expongan, ocultando información sensible (por ejemplo, contraseñas) y agregando campos calculados (por ejemplo, monto total de factura).
        Incluye los casos de uso (servicios de aplicación) que orquestan operaciones como:
        RegistrarClienteConVehiculo: crea un cliente y simultáneamente uno o varios vehículos asociados.
        CrearOrdenServicio: genera una nueva orden, asigna mecánico, reserva repuestos si están disponibles y calcula fecha estimada de entrega basado en la complejidad.
        ActualizarOrdenConTrabajoRealizado: permite al mecánico registrar el avance, actualizar el estado de la orden y descontar repuestos del inventario.
        GenerarFactura: al cerrar la orden, calcula mano de obra más repuestos y crea la factura correspondiente.
        Se utiliza AutoMapper para mapear automáticamente entre entidades de dominio y DTOs, minimizando código repetitivo y evitando errores manuales.

    Capa de Infraestructura

        Se basa en Entity Framework Core para persistencia en MySQL.
        El DbContext (AutoTallerDbContext) se configura con Fluent API en OnModelCreating para definir tablas, claves primarias, relaciones (uno a muchos entre Cliente–Vehículo, OrdenServicio–DetalleOrden, Vehículo–OrdenServicio), longitudes máximas de cadenas (p. ej., 100 caracteres para nombres), índices únicos (VIN de vehículo, código de repuesto) y comportamientos de borrado (por ejemplo, al eliminar una orden no eliminar el vehículo).
        Se implementa el Repository Pattern genérico (GenericRepository<T>) con métodos CRUD, búsqueda con filtros dinámicos y paginación (Skip/Take).
        El Unit of Work (IUnitOfWork) agrupa todos los repositorios: Clientes, Vehiculos, OrdenesServicio, Repuestos, DetalleOrdenes, Facturas, Usuarios. Con una sola llamada a CommitAsync() se mantienen múltiples cambios en una transacción atómica.

    Capa de API

        Expone controladores en ASP.NET Core como:
        ClientesController para CRUD de clientes y listado paginado.
        VehiculosController para CRUD de vehículos, filtrado por cliente o VIN.
        OrdenesServicioController para creación, actualización, cancelación y cierre de órdenes, con lógica de reserva de repuestos e inventario.
        RepuestosController para gestión de inventario: alta de repuestos, actualización de stock y baja de repuestos obsoletos.
        FacturasController para generar y consultar facturas, adjuntándolas a la orden correspondiente.
        UsuariosController para gestionar credenciales y roles.
        Cada endpoint maneja métodos HTTP GET, POST, PUT y DELETE, devolviendo respuestas JSON y códigos HTTP adecuados (200, 201, 204, 400, 404, 500).
        La paginación en listados de clientes, vehículos, órdenes y repuestos se controla mediante parámetros pageNumber y pageSize, devolviendo X-Total-Count en encabezados para informar del número total de registros.
        Se integra Rate Limiting con AspNetCoreRateLimit: reglas específicas como 60 solicitudes por minuto en /api/ordenesservicio y 30 por minuto en /api/repuestos. Al exceder el límite se devuelve HTTP 429 (Too Many Requests) con mensaje explicativo.
        La Autenticación utiliza JWT:
        Endpoint de login valida credenciales (correo/contraseña) y emite token firmado con clave secreta.
        El token incluye claims sub (UserId), email y role.
        La Autorización se implementa con políticas basadas en roles:
        Admin: acceso total a todos los recursos (gestión de usuarios, configuración general, alta/baja de repuestos).
        Mecánico: puede actualizar estado de órdenes, registrar trabajo realizado y generar facturas; no puede agregar ni eliminar repuestos del inventario.
        Recepcionista: crea órdenes de servicio, agenda citas de taller, consulta clientes y vehículos; sin acceso a estados internos de órdenes ni gestión de usuarios.
        Se configura Swagger / OpenAPI para documentar de forma interactiva todos los endpoints, modelos de request/response y esquemas de seguridad (Bearer JWT), facilitando la integración de aplicaciones web y móviles.

    Gracias a esta combinación de tecnologías —ASP.NET Core, Entity Framework Core con Fluent API, AutoMapper, JWT, Rate Limiting y paginación— AutoTallerManager se posiciona como una solución robusta, segura y preparada para gestionar de manera eficiente un taller automotriz, optimizando la atención al cliente y el control de inventarios, y asegurando la trazabilidad y control financiero en cada servicio realizado.

### Resultado esperado

    Gestión de Clientes y Vehículos

        Registro de clientes con datos completos (nombre, teléfono, correo).
        Vinculación de múltiples vehículos a un cliente con detalles (marca, modelo, año, VIN, kilometraje).
        Edición y eliminación controlada de clientes y vehículos, previniendo borrado si existen órdenes de servicio activas.

    Creación y Seguimiento de Órdenes de Servicio

        Generación de nuevas órdenes con validaciones de disponibilidad del mecánico y estado del inventario.
        Actualización del estado de la orden (pendiente, en proceso, completada, cancelada) y registro de trabajo realizado.
        Listado paginado y filtrable de órdenes por fecha, cliente, estado o mecánico asignado.

    Control de Inventario de Repuestos

        Alta, edición y baja de repuestos con stock actualizado en tiempo real.
        Validación de stock antes de asignar repuestos a una orden de servicio.
        Listado de repuestos con filtrado por categoría, descripción o nivel mínimo de stock.

    Generación de Facturas

        Cálculo automático de monto total (mano de obra + repuestos) al cerrar una orden.
        Creación de facturas vinculadas a la orden, con desglose de costos unitarios.
        Consulta histórica de facturas por fecha, cliente o número de orden.

    Autenticación y Roles

        Login mediante JWT con emisión de token que incluye claims de UserId, Email y Role.
        Roles “Admin”, “Mecánico” y “Recepcionista” con permisos diferenciados:
        Admin: gestión completa de usuarios, repuestos e informes generales.
        Mecánico: acceso a actualización de órdenes y generación de facturas.
        Recepcionista: creación de órdenes, consulta de clientes y vehículos.

    Paginación y Filtrado Eficaces

        Endpoints de listado aceptan parámetros pageNumber y pageSize, retornando encabezados X-Total-Count.
        Filtros dinámicos por campos relevantes (nombre de cliente, VIN, fecha de orden, estado).

    Rate Limiting

        Configuración de límites de solicitudes para rutas críticas (por ejemplo, máximo 60 solicitudes/min en /api/ordenesservicio).
        Respuesta HTTP 429 con mensaje al exceder el límite.

    Auditoría de Acciones

        Registro de todas las operaciones importantes en la tabla Auditorias: entidad afectada, tipo de acción, usuario responsable y marca de tiempo.
        Posibilidad de consultar logs para revisar quién creó, modificó o eliminó registros.

    Migraciones y Sincronización de Esquema

        Migraciones EF Core (“InitialCreate”, “AddRepuestosTable”, etc.) que reflejan el diseño del dominio.
        Aplicación de migraciones sin errores en MySQL o PostgreSQL en entornos de desarrollo y producción.

    Documentación Swagger / OpenAPI

        Generación automática de documentación interactiva con todos los endpoints, modelos y parámetros de seguridad.
        Instrucciones claras para autenticar (token Bearer) y probar rutas protegidas desde la interfaz Swagger.

## Rúbricas de evaluación

Dominio y conocimiento del código 32 Puntos 40.0%

Arquitectura Hexagonal 10 Puntos 12.5%

Modelado de Entidades y Fluent API 5 Puntos 6.3%

Migraciones y Sincronización de Esquema 3 Puntos 3.8%

Repository & Unit of Work 10 Puntos 12.5%

Autenticación & Autorización 10 Puntos 12.5%

Calidad del Código y Estilo 5 Puntos 6.3%

Documentación y README 5 Puntos 6.3%