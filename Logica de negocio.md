# Lógicas de Negocio

## 1. Gestión de Vehículos y Clientes

    Un cliente puede poseer múltiples vehículos (relación uno a muchos)
    Cada vehículo debe tener un VIN único (validación de unicidad)
    Registro conjunto: Es posible crear un cliente y simultáneamente asociarle uno o varios vehículos en una sola operación

## 2. Gestión de Órdenes de Servicio
Reglas de Disponibilidad:

Un vehículo NO puede estar en dos órdenes simultáneas - validación crítica para evitar conflictos de agenda

Cálculo de Fechas:

La fecha estimada de entrega se calcula automáticamente según el tipo de servicio y su complejidad:

Mantenimiento preventivo
Reparación
Diagnóstico



Asignación de Recursos:

Cada orden debe tener un mecánico asignado
Los repuestos necesarios se reservan al crear la orden (solo si hay stock disponible)

## 3. Gestión de Inventario de Repuestos
Reglas de Stock:

No se pueden utilizar repuestos fuera de stock - validación antes de crear/actualizar órdenes
Cada repuesto tiene código único (índice único)
Al registrar trabajo realizado, los repuestos se descontan automáticamente del inventario

## 4. Proceso de Facturación
Cálculo de Costos:

Al cerrar una orden, se genera automáticamente una factura que incluye:

Mano de obra (por el servicio realizado)
Repuestos utilizados (según DetalleOrden)
Monto total calculado



Trazabilidad:

Cada factura está vinculada a una orden de servicio específica

## 5. Control de Acceso por Roles
Admin:

Acceso total al sistema
Gestión de usuarios
Alta/baja de repuestos
Configuración general

Mecánico:

Actualizar estado de órdenes
Registrar trabajo realizado
Generar facturas
NO puede gestionar inventario de repuestos

Recepcionista:

Crear órdenes de servicio
Agendar citas
Consultar clientes y vehículos
NO puede acceder a estados internos de órdenes ni gestión de usuarios

## 6. Integridad Transaccional

Unit of Work: Múltiples operaciones se confirman en una transacción atómica única
Comportamiento de borrado: Al eliminar una orden, NO se elimina el vehículo asociado (preservación de datos maestros)

## 7. Control de Flujo de Trabajo
El ciclo de vida de una orden sigue estos pasos lógicos:

Creación → Reserva de repuestos + asignación de mecánico + cálculo de fecha estimada
Actualización → Registro de avance + descuento de inventario
Cierre → Generación automática de factura