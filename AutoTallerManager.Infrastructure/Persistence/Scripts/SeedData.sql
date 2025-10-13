-- Script de inicialización de datos para AutoTallerManager
-- Este script debe ejecutarse después de aplicar las migraciones

-- Insertar roles del sistema
INSERT INTO
    "Roles" (
        "NombreRol",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Admin',
        'Administrador del sistema con acceso completo',
        NOW(),
        NOW()
    ),
    (
        'Mecanico',
        'Mecánico con permisos para gestionar órdenes y facturas',
        NOW(),
        NOW()
    ),
    (
        'Recepcionista',
        'Recepcionista con permisos para crear órdenes y gestionar clientes',
        NOW(),
        NOW()
    );

-- Insertar estados de usuario
INSERT INTO
    "EstadosUsuario" (
        "NombreEstado",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
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
    "TiposCliente" (
        "NombreTipoCliente",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Particular',
        'Cliente particular',
        NOW(),
        NOW()
    ),
    (
        'Empresa',
        'Cliente empresarial',
        NOW(),
        NOW()
    ),
    (
        'Fleet',
        'Cliente con flota de vehículos',
        NOW(),
        NOW()
    );

-- Insertar tipos de vehículo
INSERT INTO
    "TiposVehiculo" (
        "NombreTipoVehiculo",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Automóvil',
        'Vehículo de pasajeros',
        NOW(),
        NOW()
    ),
    (
        'Camioneta',
        'Vehículo de carga ligera',
        NOW(),
        NOW()
    ),
    (
        'Motocicleta',
        'Vehículo de dos ruedas',
        NOW(),
        NOW()
    ),
    (
        'Camión',
        'Vehículo de carga pesada',
        NOW(),
        NOW()
    ),
    (
        'Bus',
        'Vehículo de transporte público',
        NOW(),
        NOW()
    );

-- Insertar marcas de vehículo
INSERT INTO
    "MarcasVehiculo" (
        "NombreMarca",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Toyota',
        'Marca japonesa de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Honda',
        'Marca japonesa de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Ford',
        'Marca estadounidense de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Chevrolet',
        'Marca estadounidense de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Nissan',
        'Marca japonesa de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Hyundai',
        'Marca coreana de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Kia',
        'Marca coreana de vehículos',
        NOW(),
        NOW()
    ),
    (
        'Volkswagen',
        'Marca alemana de vehículos',
        NOW(),
        NOW()
    ),
    (
        'BMW',
        'Marca alemana de vehículos de lujo',
        NOW(),
        NOW()
    ),
    (
        'Mercedes-Benz',
        'Marca alemana de vehículos de lujo',
        NOW(),
        NOW()
    );

-- Insertar modelos de vehículo (ejemplos para Toyota)
INSERT INTO
    "ModelosVehiculo" (
        "NombreModelo",
        "Descripcion",
        "MarcaId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Corolla',
        'Sedán compacto',
        1,
        NOW(),
        NOW()
    ),
    (
        'Camry',
        'Sedán mediano',
        1,
        NOW(),
        NOW()
    ),
    (
        'RAV4',
        'SUV compacto',
        1,
        NOW(),
        NOW()
    ),
    (
        'Highlander',
        'SUV mediano',
        1,
        NOW(),
        NOW()
    ),
    (
        'Tacoma',
        'Pickup mediano',
        1,
        NOW(),
        NOW()
    ),
    (
        'Tundra',
        'Pickup grande',
        1,
        NOW(),
        NOW()
    );

-- Insertar modelos de vehículo (ejemplos para Honda)
INSERT INTO
    "ModelosVehiculo" (
        "NombreModelo",
        "Descripcion",
        "MarcaId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Civic',
        'Sedán compacto',
        2,
        NOW(),
        NOW()
    ),
    (
        'Accord',
        'Sedán mediano',
        2,
        NOW(),
        NOW()
    ),
    (
        'CR-V',
        'SUV compacto',
        2,
        NOW(),
        NOW()
    ),
    (
        'Pilot',
        'SUV mediano',
        2,
        NOW(),
        NOW()
    ),
    (
        'Ridgeline',
        'Pickup mediano',
        2,
        NOW(),
        NOW()
    );

-- Insertar tipos de servicio
INSERT INTO
    "TiposServicio" (
        "NombreTipoServ",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Mantenimiento Preventivo',
        'Servicio de mantenimiento programado',
        NOW(),
        NOW()
    ),
    (
        'Cambio de Aceite',
        'Cambio de aceite y filtro',
        NOW(),
        NOW()
    ),
    (
        'Diagnóstico',
        'Diagnóstico de problemas del vehículo',
        NOW(),
        NOW()
    ),
    (
        'Reparación',
        'Reparación de componentes defectuosos',
        NOW(),
        NOW()
    ),
    (
        'Revisión Técnico-Mecánica',
        'Revisión técnica obligatoria',
        NOW(),
        NOW()
    ),
    (
        'Limpieza',
        'Limpieza y detallado del vehículo',
        NOW(),
        NOW()
    );

-- Insertar estados de servicio
INSERT INTO
    "EstadosServicio" (
        "NombreEstServ",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Pendiente',
        'Orden creada, esperando asignación',
        NOW(),
        NOW()
    ),
    (
        'En Proceso',
        'Trabajo en progreso',
        NOW(),
        NOW()
    ),
    (
        'Completada',
        'Servicio completado exitosamente',
        NOW(),
        NOW()
    ),
    (
        'Cancelada',
        'Orden cancelada',
        NOW(),
        NOW()
    ),
    (
        'Esperando Repuestos',
        'Esperando llegada de repuestos',
        NOW(),
        NOW()
    ),
    (
        'En Espera Cliente',
        'Esperando respuesta del cliente',
        NOW(),
        NOW()
    );

-- Insertar tipos de pago
INSERT INTO
    "TiposPago" (
        "NombreTipoPag",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Efectivo',
        'Pago en efectivo',
        NOW(),
        NOW()
    ),
    (
        'Tarjeta de Crédito',
        'Pago con tarjeta de crédito',
        NOW(),
        NOW()
    ),
    (
        'Tarjeta de Débito',
        'Pago con tarjeta de débito',
        NOW(),
        NOW()
    ),
    (
        'Transferencia Bancaria',
        'Transferencia bancaria',
        NOW(),
        NOW()
    ),
    (
        'Cheque',
        'Pago con cheque',
        NOW(),
        NOW()
    );

-- Insertar categorías de repuestos
INSERT INTO
    "Categorias" (
        "NombreCategoria",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Motor',
        'Repuestos del motor',
        NOW(),
        NOW()
    ),
    (
        'Frenos',
        'Sistema de frenos',
        NOW(),
        NOW()
    ),
    (
        'Suspensión',
        'Sistema de suspensión',
        NOW(),
        NOW()
    ),
    (
        'Transmisión',
        'Sistema de transmisión',
        NOW(),
        NOW()
    ),
    (
        'Eléctrico',
        'Sistema eléctrico',
        NOW(),
        NOW()
    ),
    (
        'Filtros',
        'Filtros de aire, aceite, combustible',
        NOW(),
        NOW()
    ),
    (
        'Lubricantes',
        'Aceites y lubricantes',
        NOW(),
        NOW()
    ),
    (
        'Neumáticos',
        'Neumáticos y llantas',
        NOW(),
        NOW()
    ),
    (
        'Carrocería',
        'Piezas de carrocería',
        NOW(),
        NOW()
    ),
    (
        'Interior',
        'Piezas del interior del vehículo',
        NOW(),
        NOW()
    );

-- Insertar fabricantes de repuestos
INSERT INTO
    "Fabricantes" (
        "NombreFabricante",
        "Descripcion",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Bosch',
        'Fabricante alemán de componentes automotrices',
        NOW(),
        NOW()
    ),
    (
        'Delphi',
        'Fabricante de componentes automotrices',
        NOW(),
        NOW()
    ),
    (
        'Denso',
        'Fabricante japonés de componentes automotrices',
        NOW(),
        NOW()
    ),
    (
        'Mann-Filter',
        'Fabricante de filtros',
        NOW(),
        NOW()
    ),
    (
        'NGK',
        'Fabricante de bujías',
        NOW(),
        NOW()
    ),
    (
        'Continental',
        'Fabricante de neumáticos y componentes',
        NOW(),
        NOW()
    ),
    (
        'Michelin',
        'Fabricante de neumáticos',
        NOW(),
        NOW()
    ),
    (
        'Bridgestone',
        'Fabricante de neumáticos',
        NOW(),
        NOW()
    ),
    (
        'Mobil',
        'Fabricante de lubricantes',
        NOW(),
        NOW()
    ),
    (
        'Castrol',
        'Fabricante de lubricantes',
        NOW(),
        NOW()
    );

-- Insertar usuario administrador por defecto
-- Nota: La contraseña debe ser hasheada en la aplicación
INSERT INTO
    "UsersMembers" (
        "Username",
        "Email",
        "PasswordHash",
        "EstadoId",
        "CreatedAt",
        "UpdatedAt"
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
    "UserMemberRols" (
        "UserMemberId",
        "RolId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (1, 1, NOW(), NOW());

-- Insertar algunos repuestos de ejemplo
INSERT INTO
    "Repuestos" (
        "CodigoRepuesto",
        "NombreRepu",
        "Descripcion",
        "Stock",
        "PrecioUnitario",
        "StockMinimo",
        "CategoriaId",
        "FabricanteId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'FIL-001',
        'Filtro de Aceite',
        'Filtro de aceite estándar',
        50,
        15.99,
        10,
        6,
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
        NOW(),
        NOW()
    );

-- Crear algunos clientes de ejemplo
INSERT INTO
    "Clientes" (
        "Nombre",
        "Apellido",
        "Telefono",
        "Email",
        "TipoClienteId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        'Juan',
        'Pérez',
        '3001234567',
        'juan.perez@email.com',
        1,
        NOW(),
        NOW()
    ),
    (
        'María',
        'García',
        '3002345678',
        'maria.garcia@email.com',
        1,
        NOW(),
        NOW()
    ),
    (
        'Empresa ABC',
        'S.A.S.',
        '6012345678',
        'contacto@empresaabc.com',
        2,
        NOW(),
        NOW()
    );

-- Crear algunos vehículos de ejemplo
INSERT INTO
    "Vehiculos" (
        "Vin",
        "Ano",
        "Kilometraje",
        "ClienteId",
        "TipoVehiculoId",
        "MarcaId",
        "ModeloId",
        "CreatedAt",
        "UpdatedAt"
    )
VALUES (
        '1HGBH41JXMN109186',
        2020,
        45000,
        1,
        1,
        1,
        1,
        NOW(),
        NOW()
    ),
    (
        '2HGBH41JXMN109187',
        2019,
        62000,
        2,
        1,
        2,
        7,
        NOW(),
        NOW()
    ),
    (
        '3HGBH41JXMN109188',
        2021,
        28000,
        3,
        1,
        1,
        2,
        NOW(),
        NOW()
    );

-- Mensaje de confirmación
SELECT 'Datos de inicialización insertados correctamente' AS Mensaje;