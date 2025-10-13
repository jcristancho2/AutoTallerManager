-- Script de inicialización de datos básicos para AutoTallerManager (COMPLETO)
-- Este script inserta todos los datos necesarios en el orden correcto

-- Insertar países
INSERT INTO
    paises (
        nombre,
        created_at,
        updated_at
    )
VALUES ('Colombia', NOW(), NOW()),
    ('México', NOW(), NOW()),
    ('Argentina', NOW(), NOW());

-- Insertar departamentos/estados
INSERT INTO
    departamentos (
        nombre,
        pais_id,
        created_at,
        updated_at
    )
VALUES (
        'Cundinamarca',
        1,
        NOW(),
        NOW()
    ),
    ('Antioquia', 1, NOW(), NOW()),
    (
        'Valle del Cauca',
        1,
        NOW(),
        NOW()
    ),
    (
        'Ciudad de México',
        2,
        NOW(),
        NOW()
    ),
    (
        'Buenos Aires',
        3,
        NOW(),
        NOW()
    );

-- Insertar ciudades
INSERT INTO
    ciudades (
        nombre,
        departamento_id,
        created_at,
        updated_at
    )
VALUES ('Bogotá', 1, NOW(), NOW()),
    ('Medellín', 2, NOW(), NOW()),
    ('Cali', 3, NOW(), NOW()),
    (
        'Ciudad de México',
        4,
        NOW(),
        NOW()
    ),
    (
        'Buenos Aires',
        5,
        NOW(),
        NOW()
    );

-- Insertar direcciones
INSERT INTO
    direcciones (
        descripcion,
        pais_id,
        departamento_id,
        ciudad_id,
        created_at,
        updated_at
    )
VALUES (
        'Calle 123 #45-67',
        1,
        1,
        1,
        NOW(),
        NOW()
    ),
    (
        'Carrera 80 #12-34',
        1,
        2,
        2,
        NOW(),
        NOW()
    ),
    (
        'Avenida 5 #23-45',
        1,
        3,
        3,
        NOW(),
        NOW()
    );

-- Insertar roles del sistema
INSERT INTO
    roles (
        nombre_rol,
        descripcion,
        activo,
        created_at,
        updated_at
    )
VALUES (
        'Admin',
        'Administrador del sistema con acceso completo',
        true,
        NOW(),
        NOW()
    ),
    (
        'Mecanico',
        'Mecánico con permisos para gestionar órdenes y facturas',
        true,
        NOW(),
        NOW()
    ),
    (
        'Recepcionista',
        'Recepcionista con permisos para crear órdenes y gestionar clientes',
        true,
        NOW(),
        NOW()
    );

-- Insertar estados de usuario
INSERT INTO
    estados_usuario (
        nombre_estado,
        descripcion,
        created_at,
        updated_at
    )
VALUES (
        'Activo',
        'Usuario activo en el sistema',
        NOW(),
        NOW()
    ),
    (
        'Inactivo',
        'Usuario inactivo temporalmente',
        NOW(),
        NOW()
    ),
    (
        'Suspendido',
        'Usuario suspendido por violación de políticas',
        NOW(),
        NOW()
    );

-- Insertar tipos de cliente
INSERT INTO
    tipos_cliente (
        nombre,
        created_at,
        updated_at
    )
VALUES ('Particular', NOW(), NOW()),
    ('Empresa', NOW(), NOW()),
    ('Fleet', NOW(), NOW());

-- Insertar tipos de vehículo
INSERT INTO
    tipos_vehiculo (
        nombre_tipo_vehiculo,
        created_at,
        updated_at
    )
VALUES ('Automóvil', NOW(), NOW()),
    ('Camioneta', NOW(), NOW()),
    ('Motocicleta', NOW(), NOW()),
    ('Camión', NOW(), NOW()),
    ('Bus', NOW(), NOW());

-- Insertar marcas de vehículo
INSERT INTO
    marcas_vehiculo (
        nombre,
        created_at,
        updated_at
    )
VALUES ('Toyota', NOW(), NOW()),
    ('Honda', NOW(), NOW()),
    ('Ford', NOW(), NOW()),
    ('Chevrolet', NOW(), NOW()),
    ('Nissan', NOW(), NOW()),
    ('Hyundai', NOW(), NOW()),
    ('Kia', NOW(), NOW()),
    ('Volkswagen', NOW(), NOW()),
    ('BMW', NOW(), NOW()),
    ('Mercedes-Benz', NOW(), NOW());

-- Insertar modelos de vehículo (ejemplos para Toyota)
INSERT INTO
    modelos_vehiculo (
        nombre,
        marca_id,
        created_at,
        updated_at
    )
VALUES ('Corolla', 1, NOW(), NOW()),
    ('Camry', 1, NOW(), NOW()),
    ('RAV4', 1, NOW(), NOW()),
    ('Highlander', 1, NOW(), NOW()),
    ('Tacoma', 1, NOW(), NOW()),
    ('Tundra', 1, NOW(), NOW());

-- Insertar modelos de vehículo (ejemplos para Honda)
INSERT INTO
    modelos_vehiculo (
        nombre,
        marca_id,
        created_at,
        updated_at
    )
VALUES ('Civic', 2, NOW(), NOW()),
    ('Accord', 2, NOW(), NOW()),
    ('CR-V', 2, NOW(), NOW()),
    ('Pilot', 2, NOW(), NOW()),
    ('Ridgeline', 2, NOW(), NOW());

-- Insertar tipos de servicio
INSERT INTO
    tipos_servicio (
        nombre_tipo_serv,
        created_at,
        updated_at
    )
VALUES (
        'Mantenimiento Preventivo',
        NOW(),
        NOW()
    ),
    (
        'Cambio de Aceite',
        NOW(),
        NOW()
    ),
    ('Diagnóstico', NOW(), NOW()),
    ('Reparación', NOW(), NOW()),
    (
        'Revisión Técnico-Mecánica',
        NOW(),
        NOW()
    ),
    ('Limpieza', NOW(), NOW());

-- Insertar estados de servicio
INSERT INTO
    estados_servicio (
        nombre_est_serv,
        created_at,
        updated_at
    )
VALUES ('Pendiente', NOW(), NOW()),
    ('En Proceso', NOW(), NOW()),
    ('Completada', NOW(), NOW()),
    ('Cancelada', NOW(), NOW()),
    (
        'Esperando Repuestos',
        NOW(),
        NOW()
    ),
    (
        'En Espera Cliente',
        NOW(),
        NOW()
    );

-- Insertar tipos de pago
INSERT INTO
    tipos_pago (
        nombre,
        created_at,
        updated_at
    )
VALUES ('Efectivo', NOW(), NOW()),
    (
        'Tarjeta de Crédito',
        NOW(),
        NOW()
    ),
    (
        'Tarjeta de Débito',
        NOW(),
        NOW()
    ),
    (
        'Transferencia Bancaria',
        NOW(),
        NOW()
    ),
    ('Cheque', NOW(), NOW());

-- Insertar categorías de repuestos
INSERT INTO
    categorias (
        nombre,
        created_at,
        updated_at
    )
VALUES ('Motor', NOW(), NOW()),
    ('Frenos', NOW(), NOW()),
    ('Suspensión', NOW(), NOW()),
    ('Transmisión', NOW(), NOW()),
    ('Eléctrico', NOW(), NOW()),
    ('Filtros', NOW(), NOW()),
    ('Lubricantes', NOW(), NOW()),
    ('Neumáticos', NOW(), NOW()),
    ('Carrocería', NOW(), NOW()),
    ('Interior', NOW(), NOW());

-- Insertar fabricantes de repuestos
INSERT INTO
    fabricantes (
        nombre,
        descripcion,
        telefono,
        email,
        created_at,
        updated_at
    )
VALUES (
        'Bosch',
        'Fabricante alemán de componentes automotrices',
        '+49-711-811-0',
        'info@bosch.com',
        NOW(),
        NOW()
    ),
    (
        'Delphi',
        'Fabricante de componentes automotrices',
        '+1-248-324-2000',
        'info@delphi.com',
        NOW(),
        NOW()
    ),
    (
        'Denso',
        'Fabricante japonés de componentes automotrices',
        '+81-566-25-1111',
        'info@denso.com',
        NOW(),
        NOW()
    ),
    (
        'Mann-Filter',
        'Fabricante de filtros',
        '+49-621-890-0',
        'info@mann-filter.com',
        NOW(),
        NOW()
    ),
    (
        'NGK',
        'Fabricante de bujías',
        '+81-3-3263-9000',
        'info@ngk.com',
        NOW(),
        NOW()
    ),
    (
        'Continental',
        'Fabricante de neumáticos y componentes',
        '+49-511-938-0',
        'info@continental.com',
        NOW(),
        NOW()
    ),
    (
        'Michelin',
        'Fabricante de neumáticos',
        '+33-1-78-76-70-00',
        'info@michelin.com',
        NOW(),
        NOW()
    ),
    (
        'Bridgestone',
        'Fabricante de neumáticos',
        '+81-3-3245-0111',
        'info@bridgestone.com',
        NOW(),
        NOW()
    ),
    (
        'Mobil',
        'Fabricante de lubricantes',
        '+1-713-546-3000',
        'info@mobil.com',
        NOW(),
        NOW()
    ),
    (
        'Castrol',
        'Fabricante de lubricantes',
        '+44-20-7934-0000',
        'info@castrol.com',
        NOW(),
        NOW()
    );

-- Insertar usuario administrador por defecto
INSERT INTO
    users_members (
        username,
        email,
        password_hash,
        estado_id,
        created_at,
        updated_at
    )
VALUES (
        'admin',
        'admin@autotaller.com',
        '$2a$11$K8Y1OjqK8Y1OjqK8Y1OjqO',
        1,
        NOW(),
        NOW()
    );

-- Asignar rol de administrador al usuario por defecto
INSERT INTO
    user_member_roles (
        user_member_id,
        rol_id,
        created_at,
        updated_at
    )
VALUES (1, 1, NOW(), NOW());

-- Insertar algunos repuestos de ejemplo
INSERT INTO
    repuestos (
        codigo,
        nombre,
        descripcion,
        stock,
        precio_unitario,
        stock_minimo,
        categoria_id,
        tipo_vehiculo_id,
        fabricante_id,
        created_at,
        updated_at
    )
VALUES (
        'FIL-001',
        'Filtro de Aceite',
        'Filtro de aceite estándar',
        50,
        15.99,
        10,
        6,
        1,
        4,
        NOW(),
        NOW()
    ),
    (
        'ACE-001',
        'Aceite Motor 5W-30',
        'Aceite sintético para motor',
        30,
        25.99,
        5,
        7,
        1,
        9,
        NOW(),
        NOW()
    ),
    (
        'BUI-001',
        'Bujía de Encendido',
        'Bujía estándar NGK',
        100,
        8.99,
        20,
        1,
        1,
        5,
        NOW(),
        NOW()
    ),
    (
        'PAS-001',
        'Pastillas de Freno',
        'Pastillas de freno delanteras',
        25,
        45.99,
        5,
        2,
        1,
        1,
        NOW(),
        NOW()
    ),
    (
        'DIS-001',
        'Disco de Freno',
        'Disco de freno delantero',
        15,
        89.99,
        3,
        2,
        1,
        1,
        NOW(),
        NOW()
    );

-- Crear algunos clientes de ejemplo
INSERT INTO
    clientes (
        nombre_completo,
        telefono,
        email,
        tipo_cliente_id,
        direccion_id,
        created_at,
        updated_at
    )
VALUES (
        'Juan Pérez',
        '3001234567',
        'juan.perez@email.com',
        1,
        1,
        NOW(),
        NOW()
    ),
    (
        'María García',
        '3002345678',
        'maria.garcia@email.com',
        1,
        2,
        NOW(),
        NOW()
    ),
    (
        'Empresa ABC S.A.S.',
        '6012345678',
        'contacto@empresaabc.com',
        2,
        3,
        NOW(),
        NOW()
    );

-- Crear algunos vehículos de ejemplo
INSERT INTO
    vehiculos (
        placa,
        ano,
        vin,
        kilometraje,
        cliente_id,
        tipo_vehiculo_id,
        marca_id,
        modelo_id,
        created_at,
        updated_at
    )
VALUES (
        'ABC123',
        2020,
        '1HGBH41JXMN109186',
        45000,
        1,
        1,
        1,
        1,
        NOW(),
        NOW()
    ),
    (
        'DEF456',
        2019,
        '2HGBH41JXMN109187',
        62000,
        2,
        1,
        2,
        7,
        NOW(),
        NOW()
    ),
    (
        'GHI789',
        2021,
        '3HGBH41JXMN109188',
        28000,
        3,
        1,
        1,
        2,
        NOW(),
        NOW()
    );

-- Mensaje de confirmación
SELECT 'Datos de inicialización completos insertados correctamente' AS mensaje;