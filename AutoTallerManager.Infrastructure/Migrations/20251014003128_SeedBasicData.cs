using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedBasicData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar roles básicos
            migrationBuilder.Sql(@"
                INSERT INTO rols (nombre_rol, descripcion, activo, created_at, updated_at)
                VALUES 
                    ('Admin', 'Administrador del sistema', true, NOW(), NOW()),
                    ('Mecanico', 'Mecánico del taller', true, NOW(), NOW()),
                    ('Recepcionista', 'Recepcionista del taller', true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar estados de usuario
            migrationBuilder.Sql(@"
                INSERT INTO estados_usuario (nombre_est_usu, created_at, updated_at)
                VALUES 
                    ('Activo', NOW(), NOW()),
                    ('Inactivo', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de cliente
            migrationBuilder.Sql(@"
                INSERT INTO customers_types (nombre, created_at, updated_at)
                VALUES 
                    ('Particular', NOW(), NOW()),
                    ('Empresa', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de vehículo
            migrationBuilder.Sql(@"
                INSERT INTO vehicle_types (nombre_tipo_vehiculo, created_at, updated_at)
                VALUES 
                    ('Automóvil', NOW(), NOW()),
                    ('Camioneta', NOW(), NOW()),
                    ('Motocicleta', NOW(), NOW())
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
                    ('Nissan', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar modelos de vehículo
            migrationBuilder.Sql(@"
                INSERT INTO vehicle_models (nombre, created_at, updated_at)
                VALUES 
                    ('Corolla', NOW(), NOW()),
                    ('Camry', NOW(), NOW()),
                    ('Civic', NOW(), NOW()),
                    ('Accord', NOW(), NOW()),
                    ('Focus', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de servicio
            migrationBuilder.Sql(@"
                INSERT INTO service_types (nombre_tipo_serv, created_at, updated_at)
                VALUES 
                    ('Mantenimiento', NOW(), NOW()),
                    ('Reparación', NOW(), NOW()),
                    ('Diagnóstico', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar estados de servicio
            migrationBuilder.Sql(@"
                INSERT INTO services_status (nombre_est_serv, created_at, updated_at)
                VALUES 
                    ('Pendiente', NOW(), NOW()),
                    ('En Proceso', NOW(), NOW()),
                    ('Completada', NOW(), NOW()),
                    ('Cancelada', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar tipos de pago
            migrationBuilder.Sql(@"
                INSERT INTO payment_types (nombre_tipo_pag, created_at, updated_at)
                VALUES 
                    ('Efectivo', NOW(), NOW()),
                    ('Tarjeta', NOW(), NOW()),
                    ('Transferencia', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar categorías de repuestos
            migrationBuilder.Sql(@"
                INSERT INTO categories (nombre_cat, created_at, updated_at)
                VALUES 
                    ('Motor', NOW(), NOW()),
                    ('Frenos', NOW(), NOW()),
                    ('Suspensión', NOW(), NOW()),
                    ('Filtros', NOW(), NOW()),
                    ('Lubricantes', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar fabricantes
            migrationBuilder.Sql(@"
                INSERT INTO manufacturer (nombrefab, descripcion, telefono, email, created_at, updated_at)
                VALUES 
                    ('NGK', 'Fabricante de bujías', '+81-3-3212-2111', 'info@ngk.com', NOW(), NOW()),
                    ('Bosch', 'Componentes automotrices', '+49-711-811-0', 'info@bosch.com', NOW(), NOW()),
                    ('Mobil', 'Lubricantes', '+1-713-546-3000', 'info@mobil.com', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Insertar usuario administrador
            migrationBuilder.Sql(@"
                INSERT INTO users_members (username, email, password, created_at, updated_at)
                VALUES ('admin', 'admin@autotaller.com', '$2a$11$K8Y1OjqK8Y1OjqK8Y1OjqO', NOW(), NOW())
                ON CONFLICT (email) DO NOTHING;
            ");

            // Asignar rol de administrador
            migrationBuilder.Sql(@"
                INSERT INTO user_member_roles (user_member_id, rol_id)
                SELECT u.id, r.id
                FROM users_members u, rols r
                WHERE u.email = 'admin@autotaller.com' AND r.nombre_rol = 'Admin'
                ON CONFLICT DO NOTHING;
            ");

            // Insertar algunos repuestos de ejemplo (comentado por problemas de foreign keys)
            // migrationBuilder.Sql(@"
            //     INSERT INTO replacement (codigo, nombre_rep, descripcion, stock, precio_unitario, categoria_id, tipo_vehiculo_id, fabricante_id, created_at, updated_at)
            //     VALUES 
            //         ('FIL-001', 'Filtro de Aceite', 'Filtro de aceite estándar', 50, 15.99, 4, 1, 1, NOW(), NOW()),
            //         ('ACE-001', 'Aceite Motor 5W-30', 'Aceite sintético', 30, 25.99, 5, 1, 3, NOW(), NOW()),
            //         ('BUI-001', 'Bujía de Encendido', 'Bujía NGK', 100, 8.99, 1, 1, 1, NOW(), NOW());
            // ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
