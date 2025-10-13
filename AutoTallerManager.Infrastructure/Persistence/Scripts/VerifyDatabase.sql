-- Script de verificación de la base de datos AutoTallerManager
-- Este script verifica que todas las tablas y relaciones estén correctamente configuradas

-- Verificar que todas las tablas existen
SELECT
    table_name,
    CASE
        WHEN table_name IN (
            'Roles',
            'EstadosUsuario',
            'UsersMembers',
            'RefreshTokens',
            'UserMemberRols',
            'TiposCliente',
            'Clientes',
            'TiposVehiculo',
            'MarcasVehiculo',
            'ModelosVehiculo',
            'Vehiculos',
            'TiposServicio',
            'EstadosServicio',
            'OrdenesServicio',
            'DetalleOrdenes',
            'Categorias',
            'Fabricantes',
            'Repuestos',
            'TiposPago',
            'Facturas',
            'Auditorias'
        ) THEN '✓ Tabla encontrada'
        ELSE '✗ Tabla faltante'
    END as estado
FROM information_schema.tables
WHERE
    table_schema = 'public'
    AND table_name IN (
        'Roles',
        'EstadosUsuario',
        'UsersMembers',
        'RefreshTokens',
        'UserMemberRols',
        'TiposCliente',
        'Clientes',
        'TiposVehiculo',
        'MarcasVehiculo',
        'ModelosVehiculo',
        'Vehiculos',
        'TiposServicio',
        'EstadosServicio',
        'OrdenesServicio',
        'DetalleOrdenes',
        'Categorias',
        'Fabricantes',
        'Repuestos',
        'TiposPago',
        'Facturas',
        'Auditorias'
    )
ORDER BY table_name;

-- Verificar índices únicos críticos
SELECT
    indexname,
    tablename,
    CASE
        WHEN indexname IN (
            'IX_UsersMembers_Email',
            'IX_UsersMembers_Username',
            'IX_Vehiculos_Vin',
            'IX_Repuestos_CodigoRepuesto',
            'IX_Facturas_NumeroFactura'
        ) THEN '✓ Índice único encontrado'
        ELSE '✗ Índice único faltante'
    END as estado
FROM pg_indexes
WHERE
    schemaname = 'public'
    AND indexname IN (
        'IX_UsersMembers_Email',
        'IX_UsersMembers_Username',
        'IX_Vehiculos_Vin',
        'IX_Repuestos_CodigoRepuesto',
        'IX_Facturas_NumeroFactura'
    )
ORDER BY indexname;

-- Verificar foreign keys críticas
SELECT
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name,
    '✓ FK encontrada' as estado
FROM
    information_schema.table_constraints AS tc
    JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
    JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name
    AND ccu.table_schema = tc.table_schema
WHERE
    tc.constraint_type = 'FOREIGN KEY'
    AND tc.table_schema = 'public'
    AND tc.table_name IN (
        'Vehiculos',
        'OrdenesServicio',
        'DetalleOrdenes',
        'Facturas',
        'Repuestos'
    )
ORDER BY tc.table_name, kcu.column_name;

-- Verificar check constraints
SELECT
    conname as constraint_name,
    pg_get_constraintdef (oid) as constraint_definition,
    '✓ Check constraint encontrado' as estado
FROM pg_constraint
WHERE
    contype = 'c'
    AND conname LIKE 'CK_%'
ORDER BY conname;

-- Verificar datos de inicialización
SELECT
    'Roles' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 3 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "Roles"
UNION ALL
SELECT
    'EstadosUsuario' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 3 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "EstadosUsuario"
UNION ALL
SELECT
    'TiposCliente' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 3 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "TiposCliente"
UNION ALL
SELECT
    'TiposVehiculo' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "TiposVehiculo"
UNION ALL
SELECT
    'MarcasVehiculo' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "MarcasVehiculo"
UNION ALL
SELECT
    'TiposServicio' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "TiposServicio"
UNION ALL
SELECT
    'EstadosServicio' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 6 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "EstadosServicio"
UNION ALL
SELECT
    'TiposPago' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "TiposPago"
UNION ALL
SELECT
    'Categorias' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "Categorias"
UNION ALL
SELECT
    'Fabricantes' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 5 THEN '✓ Datos OK'
        ELSE '✗ Datos faltantes'
    END as estado
FROM "Fabricantes"
UNION ALL
SELECT
    'UsersMembers' as tabla,
    COUNT(*) as registros,
    CASE
        WHEN COUNT(*) >= 1 THEN '✓ Usuario admin creado'
        ELSE '✗ Usuario admin faltante'
    END as estado
FROM "UsersMembers";

-- Verificar usuario administrador
SELECT 
    um."Username",
    um."Email",
    r."NombreRol",
    eu."NombreEstado",
    '✓ Usuario admin configurado' as estado
FROM "UsersMembers" um
JOIN "UserMemberRols" umr ON um."Id" = umr."UserMemberId"
JOIN "Roles" r ON umr."RolId" = r."Id"
JOIN "EstadosUsuario" eu ON um."EstadoId" = eu."Id"
WHERE um."Username" = 'admin';

-- Resumen de verificación
SELECT 'VERIFICACIÓN COMPLETA' as mensaje, 'Revisa los resultados anteriores para confirmar que todo esté correcto' as instruccion;