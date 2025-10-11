# Autenticación
    POST /api/usuario/login - Login con JWT
    POST /api/usuario/register - Registro (Admin)
    GET /api/usuario/roles - Listar roles
    GET /api/usuario/estados - Listar estados de usuario
# Clientes 
    GET /api/clientes - Listar (paginado, filtros)
    GET /api/clientes/{id} - Obtener por ID
    POST /api/clientes - Crear (Admin/Recepcionista)
    PUT /api/clientes/{id} - Actualizar (Admin/Recepcionista)
    DELETE /api/clientes/{id} - Eliminar (Admin)
# Vehículos 
    GET /api/vehiculos - Listar (paginado, filtros)
    GET /api/vehiculos/{id} - Obtener por ID
    GET /api/vehiculos/cliente/{clienteId} - Por cliente
    GET /api/vehiculos/vin/{vin} - Buscar por VIN
    POST /api/vehiculos - Crear (Admin/Recepcionista)
    PUT /api/vehiculos/{id} - Actualizar (Admin/Recepcionista)
    DELETE /api/vehiculos/{id} - Eliminar (Admin)
# Órdenes de Servicio modo basico
    GET /api/ordenesservicio - Listar (paginado, filtros)
    GET /api/ordenesservicio/{id} - Obtener por ID
    POST /api/ordenesservicio - Crear (Admin/Recepcionista)
    PUT /api/ordenesservicio/{id} - Actualizar (Admin/Mecánico/Recepcionista)
    DELETE /api/ordenesservicio/{id} - Eliminar (Admin)
# Repuestos
    GET /api/repuestos - Listar (paginado, filtros)
    GET /api/repuestos/{id} - Obtener por ID
    GET /api/repuestos/codigo/{codigo} - Buscar por código
    GET /api/repuestos/stock/bajo - Stock bajo
    POST /api/repuestos - Crear (Admin)
    PUT /api/repuestos/{id} - Actualizar (Admin)
DELETE /api/repuestos/{id} - Eliminar (Admin)
# Facturas
    GET /api/facturas - Listar (paginado, filtros)
    GET /api/facturas/{id} - Obtener por ID
    GET /api/facturas/cliente/{clienteId} - Por cliente
    GET /api/facturas/ingresos - Calcular ingresos
    POST /api/facturas - Crear (Admin/Recepcionista)
    PUT /api/facturas/{id} - Actualizar (Admin/Recepcionista)
    DELETE /api/facturas/{id} - Eliminar (Admin)