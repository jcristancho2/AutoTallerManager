using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar direcciones básicas (sin jerarquía completa por ahora)
            migrationBuilder.Sql(@"
                INSERT INTO addresses (descripcion, ciudad_id, created_at, updated_at)
                VALUES 
                    ('Calle 123 #45-67', 1, NOW(), NOW()),
                    ('Carrera 80 #12-34', 1, NOW(), NOW()),
                    ('Avenida 5 #23-45', 1, NOW(), NOW()),
                    ('Calle Principal #100', 1, NOW(), NOW()),
                    ('Av. Libertador #500', 1, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
