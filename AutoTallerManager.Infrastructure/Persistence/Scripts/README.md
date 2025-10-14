# Scripts de Base de Datos - AutoTallerManager

Este directorio contiene los scripts SQL necesarios para configurar la base de datos del sistema AutoTallerManager.

## Archivos Disponibles

### 01_CreateDatabase.sql
**Propósito**: Crear la estructura completa de la base de datos desde cero.
- Crea todas las tablas con nombres en snake_case
- Incluye todas las relaciones y restricciones
- Crea índices para optimizar el rendimiento
- Compatible con PostgreSQL 16+

**Cuándo usar**: 
- Primera instalación del sistema
- Recrear la base de datos completamente
- Desarrollo local

**Comando**:
```bash
docker exec -i autotaller_db psql -U postgres -d autotallerdb < 01_CreateDatabase.sql
```

### 02_SeedData.sql
**Propósito**: Insertar datos de inicialización y prueba.
- Datos de catálogos (roles, tipos de vehículo, marcas, etc.)
- Usuario administrador por defecto
- Datos de ejemplo (clientes, vehículos, repuestos)
- Compatible con la estructura snake_case

**Cuándo usar**:
- Después de crear la estructura de la base de datos
- Para tener datos de prueba en desarrollo
- Configuración inicial del sistema

**Comando**:
```bash
docker exec -i autotaller_db psql -U postgres -d autotallerdb < 02_SeedData.sql
```


## Orden de Ejecución Recomendado

### Para nueva instalación:
1. `01_CreateDatabase.sql` - Crear estructura
2. `02_SeedData.sql` - Insertar datos iniciales


## Convenciones de Nomenclatura

El sistema utiliza **snake_case** para todos los nombres de tablas y columnas:
- ✅ `clientes`, `vehiculos`, `ordenes_servicio`
- ✅ `nombre_completo`, `fecha_ingreso`, `tipo_cliente_id`
- ❌ `Clientes`, `Vehiculos`, `OrdenesServicio`
- ❌ `NombreCompleto`, `FechaIngreso`, `TipoClienteId`

## Notas Importantes

- Todos los scripts están diseñados para PostgreSQL 16+
- Los scripts incluyen timestamps automáticos (`created_at`, `updated_at`)
- Se crean índices automáticamente para optimizar consultas frecuentes
- Las restricciones de integridad referencial están configuradas correctamente

## Solución de Problemas

### Error: "relation does not exist"
- Asegúrate de ejecutar `01_CreateDatabase.sql` primero
- Verifica que estés conectado a la base de datos correcta

### Error: "duplicate key value"
- Los datos ya existen en la base de datos
- Puedes ignorar estos errores o limpiar la tabla antes de ejecutar

### Error: "permission denied"
- Verifica que el usuario `postgres` tenga permisos suficientes
- Asegúrate de estar ejecutando desde el contenedor correcto
