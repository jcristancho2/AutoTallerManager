using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar países
            migrationBuilder.Sql(@"
                INSERT INTO countries (nombre, created_at, updated_at)
                VALUES 
                    ('Colombia', NOW(), NOW()),
                    ('México', NOW(), NOW()),
                    ('Argentina', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar departamentos
            migrationBuilder.Sql(@"
                INSERT INTO states (nombre, pais_id, created_at, updated_at)
                VALUES 
                    ('Cundinamarca', 1, NOW(), NOW()),
                    ('Antioquia', 1, NOW(), NOW()),
                    ('Valle del Cauca', 1, NOW(), NOW()),
                    ('Ciudad de México', 2, NOW(), NOW()),
                    ('Buenos Aires', 3, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar ciudades
            migrationBuilder.Sql(@"
                INSERT INTO cities (nombre, departamento_id, created_at, updated_at)
                VALUES 
                    ('Bogotá', 1, NOW(), NOW()),
                    ('Medellín', 2, NOW(), NOW()),
                    ('Cali', 3, NOW(), NOW()),
                    ('Ciudad de México', 4, NOW(), NOW()),
                    ('Buenos Aires', 5, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar direcciones
            migrationBuilder.Sql(@"
                INSERT INTO addresses (descripcion, ciudad_id, created_at, updated_at)
                VALUES 
                    ('Calle 123 #45-67', 1, NOW(), NOW()),
                    ('Carrera 80 #12-34', 2, NOW(), NOW()),
                    ('Avenida 5 #23-45', 3, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar roles
            migrationBuilder.Sql(@"
                INSERT INTO rols (nombre_rol, descripcion, activo, created_at, updated_at)
                VALUES 
                    ('Admin', 'Administrador del sistema con acceso completo', true, NOW(), NOW()),
                    ('Mecanico', 'Mecánico con permisos para gestionar órdenes y facturas', true, NOW(), NOW()),
                    ('Recepcionista', 'Recepcionista con permisos para crear órdenes y gestionar clientes', true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar estados de usuario
            migrationBuilder.Sql(@"
                INSERT INTO estados_usuario (nombre_estado, descripcion, created_at, updated_at)
                VALUES 
                    ('Activo', 'Usuario activo en el sistema', NOW(), NOW()),
                    ('Inactivo', 'Usuario inactivo temporalmente', NOW(), NOW()),
                    ('Suspendido', 'Usuario suspendido por violación de políticas', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de cliente
            migrationBuilder.Sql(@"
                INSERT INTO customers_types (nombre, created_at, updated_at)
                VALUES 
                    ('Particular', NOW(), NOW()),
                    ('Empresa', NOW(), NOW()),
                    ('Fleet', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de vehículo
            migrationBuilder.Sql(@"
                INSERT INTO vehicle_types (nombre_tipo_vehiculo, created_at, updated_at)
                VALUES 
                    ('Automóvil', NOW(), NOW()),
                    ('Camioneta', NOW(), NOW()),
                    ('Motocicleta', NOW(), NOW()),
                    ('Camión', NOW(), NOW()),
                    ('Bus', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar marcas de vehículo
            migrationBuilder.Sql(@"
                INSERT INTO vehicle_brands (nombre, created_at, updated_at)
                VALUES 
                    ('Toyota', NOW(), NOW()),
                    ('Honda', NOW(), NOW()),
                    ('Ford', NOW(), NOW()),
                    ('Chevrolet', NOW(), NOW()),
                    ('Nissan', NOW(), NOW()),
                    ('Hyundai', NOW(), NOW()),
                    ('Kia', NOW(), NOW()),
                    ('Volkswagen', NOW(), NOW()),
                    ('BMW', NOW(), NOW()),
                    ('Mercedes-Benz', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar modelos de vehículo
            migrationBuilder.Sql(@"
                INSERT INTO vehicle_models (nombre, marca_id, created_at, updated_at)
                VALUES 
                    ('Corolla', 1, NOW(), NOW()),
                    ('Camry', 1, NOW(), NOW()),
                    ('RAV4', 1, NOW(), NOW()),
                    ('Civic', 2, NOW(), NOW()),
                    ('Accord', 2, NOW(), NOW()),
                    ('CR-V', 2, NOW(), NOW()),
                    ('Focus', 3, NOW(), NOW()),
                    ('F-150', 3, NOW(), NOW()),
                    ('Escape', 3, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de servicio
            migrationBuilder.Sql(@"
                INSERT INTO service_types (nombre_servicio, descripcion, created_at, updated_at)
                VALUES 
                    ('Mantenimiento Preventivo', 'Servicio de mantenimiento programado', NOW(), NOW()),
                    ('Reparación', 'Reparación de fallas específicas', NOW(), NOW()),
                    ('Diagnóstico', 'Diagnóstico de problemas del vehículo', NOW(), NOW()),
                    ('Revisión Técnica', 'Revisión técnica obligatoria', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar estados de servicio
            migrationBuilder.Sql(@"
                INSERT INTO services_status (nombre_estado, descripcion, created_at, updated_at)
                VALUES 
                    ('Pendiente', 'Orden pendiente de asignación', NOW(), NOW()),
                    ('En Proceso', 'Orden en proceso de ejecución', NOW(), NOW()),
                    ('Completada', 'Orden completada exitosamente', NOW(), NOW()),
                    ('Cancelada', 'Orden cancelada', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de pago
            migrationBuilder.Sql(@"
                INSERT INTO payment_types (nombre_tipo_pago, descripcion, created_at, updated_at)
                VALUES 
                    ('Efectivo', 'Pago en efectivo', NOW(), NOW()),
                    ('Tarjeta de Crédito', 'Pago con tarjeta de crédito', NOW(), NOW()),
                    ('Tarjeta de Débito', 'Pago con tarjeta de débito', NOW(), NOW()),
                    ('Transferencia', 'Pago por transferencia bancaria', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de acción
            migrationBuilder.Sql(@"
                INSERT INTO types_actions (nombre_accion, descripcion, created_at, updated_at)
                VALUES 
                    ('Crear', 'Creación de registro', NOW(), NOW()),
                    ('Modificar', 'Modificación de registro', NOW(), NOW()),
                    ('Eliminar', 'Eliminación de registro', NOW(), NOW()),
                    ('Consultar', 'Consulta de registro', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar categorías
            migrationBuilder.Sql(@"
                INSERT INTO categories (nombre_cat, created_at, updated_at)
                VALUES 
                    ('Motor', NOW(), NOW()),
                    ('Frenos', NOW(), NOW()),
                    ('Suspensión', NOW(), NOW()),
                    ('Transmisión', NOW(), NOW()),
                    ('Eléctrico', NOW(), NOW()),
                    ('Filtros', NOW(), NOW()),
                    ('Lubricantes', NOW(), NOW()),
                    ('Neumáticos', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar fabricantes
            migrationBuilder.Sql(@"
                INSERT INTO manufacturer (nombre, descripcion, telefono, email, created_at, updated_at)
                VALUES 
                    ('NGK', 'Fabricante de bujías', '+81-3-3212-2111', 'info@ngk.com', NOW(), NOW()),
                    ('Bosch', 'Fabricante de componentes automotrices', '+49-711-811-0', 'info@bosch.com', NOW(), NOW()),
                    ('Brembo', 'Fabricante de sistemas de frenos', '+39-035-605-111', 'info@brembo.com', NOW(), NOW()),
                    ('Mann-Filter', 'Fabricante de filtros', '+49-621-8901-0', 'info@mann-filter.com', NOW(), NOW()),
                    ('Michelin', 'Fabricante de neumáticos', '+33-1-78-76-70-00', 'info@michelin.com', NOW(), NOW()),
                    ('Bridgestone', 'Fabricante de neumáticos', '+81-3-3244-5000', 'info@bridgestone.com', NOW(), NOW()),
                    ('Mobil', 'Fabricante de lubricantes', '+1-713-546-3000', 'info@mobil.com', NOW(), NOW()),
                    ('Castrol', 'Fabricante de lubricantes', '+44-20-7934-0000', 'info@castrol.com', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar usuario administrador por defecto
            migrationBuilder.Sql(@"
                INSERT INTO users_members (username, email, password_hash, estado_id, created_at, updated_at)
                VALUES ('admin', 'admin@autotaller.com', '$2a$11$K8Y1OjqK8Y1OjqK8Y1OjqO', 1, NOW(), NOW())
                ON CONFLICT (email) DO NOTHING;
            ");

            // Asignar rol de administrador
            migrationBuilder.Sql(@"
                INSERT INTO user_member_roles (user_member_id, rol_id, created_at, updated_at)
                SELECT u.id, r.id, NOW(), NOW()
                FROM users_members u, rols r
                WHERE u.email = 'admin@autotaller.com' AND r.nombre_rol = 'Admin'
                ON CONFLICT DO NOTHING;
            ");

            // Insertar repuestos de ejemplo
            migrationBuilder.Sql(@"
                INSERT INTO replacement (codigo, nombre_repu, descripcion, stock, precio_unitario, categoria_id, tipo_vehiculo_id, fabricante_id, created_at, updated_at)
                VALUES 
                    ('FIL-001', 'Filtro de Aceite', 'Filtro de aceite estándar', 50, 15.99, 6, 1, 4, NOW(), NOW()),
                    ('ACE-001', 'Aceite Motor 5W-30', 'Aceite sintético para motor', 30, 25.99, 7, 1, 7, NOW(), NOW()),
                    ('BUI-001', 'Bujía de Encendido', 'Bujía estándar NGK', 100, 8.99, 1, 1, 1, NOW(), NOW()),
                    ('PAS-001', 'Pastillas de Freno', 'Pastillas de freno delanteras', 25, 45.99, 2, 1, 3, NOW(), NOW()),
                    ('DIS-001', 'Disco de Freno', 'Disco de freno delantero', 15, 89.99, 2, 1, 3, NOW(), NOW())
                ON CONFLICT (codigo) DO NOTHING;
            ");

            // Insertar clientes de ejemplo
            migrationBuilder.Sql(@"
                INSERT INTO customers (nombre_completo, telefono, email, tipo_cliente_id, direccion_id, created_at, updated_at)
                VALUES 
                    ('Juan Pérez', '3001234567', 'juan.perez@email.com', 1, 1, NOW(), NOW()),
                    ('María García', '3002345678', 'maria.garcia@email.com', 1, 2, NOW(), NOW()),
                    ('Empresa ABC S.A.S.', '6012345678', 'contacto@empresaabc.com', 2, 3, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar vehículos de ejemplo
            migrationBuilder.Sql(@"
                INSERT INTO vehiculos (placa, ano, vin, kilometraje, cliente_id, tipo_vehiculo_id, marca_id, modelo_id, created_at, updated_at)
                VALUES 
                    ('ABC123', 2020, '1HGBH41JXMN109186', 45000, 1, 1, 1, 1, NOW(), NOW()),
                    ('DEF456', 2019, '2HGBH41JXMN109187', 62000, 2, 1, 2, 4, NOW(), NOW()),
                    ('GHI789', 2021, '3HGBH41JXMN109188', 28000, 3, 1, 1, 2, NOW(), NOW())
                ON CONFLICT (vin) DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar datos de ejemplo (opcional)
            migrationBuilder.Sql(@"
                DELETE FROM vehiculos WHERE placa IN ('ABC123', 'DEF456', 'GHI789');
                DELETE FROM clientes WHERE email IN ('juan.perez@email.com', 'maria.garcia@email.com', 'contacto@empresaabc.com');
                DELETE FROM repuestos WHERE codigo IN ('FIL-001', 'ACE-001', 'BUI-001', 'PAS-001', 'DIS-001');
                DELETE FROM user_member_roles WHERE user_member_id IN (SELECT id FROM users_members WHERE email = 'admin@autotaller.com');
                DELETE FROM users_members WHERE email = 'admin@autotaller.com';
            ");
        }
    }
}
