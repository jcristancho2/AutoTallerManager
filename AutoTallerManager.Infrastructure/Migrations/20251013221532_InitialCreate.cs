using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoTallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoriaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_cat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_categorias", x => x.categoriaid);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipo_cliente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_est_usu = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_estados_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "manufacturer",
                columns: table => new
                {
                    fabricanteid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombrefab = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_fabricantes", x => x.fabricanteid);
                });

            migrationBuilder.CreateTable(
                name: "payment_types",
                columns: table => new
                {
                    tipo_pago_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_tipo_pag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_tipos_pago", x => x.tipo_pago_id);
                });

            migrationBuilder.CreateTable(
                name: "rols",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_types",
                columns: table => new
                {
                    tipo_servid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_tipo_serv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_tipos_servicio", x => x.tipo_servid);
                });

            migrationBuilder.CreateTable(
                name: "services_status",
                columns: table => new
                {
                    estado_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_est_serv = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_estados_servicio", x => x.estado_id);
                });

            migrationBuilder.CreateTable(
                name: "types_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipo_accion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marca_vehiculo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_models",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modelo_vehiculo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_types",
                columns: table => new
                {
                    tipo_vehiculo_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_tipo_vehiculo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_tipos_vehiculo", x => x.tipo_vehiculo_id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    pais_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departamento", x => x.id);
                    table.ForeignKey(
                        name: "fk_departamento_pais",
                        column: x => x.pais_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    estado_usuario_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_estados_usuario_estado_usuario_id",
                        column: x => x.estado_usuario_id,
                        principalTable: "estados_usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_rols_rol_id",
                        column: x => x.rol_id,
                        principalTable: "rols",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: true),
                    expiries = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_user_member",
                        column: x => x.user_id,
                        principalTable: "users_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_member_roles",
                columns: table => new
                {
                    user_member_id = table.Column<int>(type: "integer", nullable: false),
                    rol_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_member_rol", x => new { x.user_member_id, x.rol_id });
                    table.ForeignKey(
                        name: "fk_user_member_rol_rol",
                        column: x => x.rol_id,
                        principalTable: "rols",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_member_rol_user_member",
                        column: x => x.user_member_id,
                        principalTable: "users_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "replacement",
                columns: table => new
                {
                    repuesto_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre_rep = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    precio_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    categoria_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_vehiculo_id = table.Column<int>(type: "integer", nullable: false),
                    fabricante_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_repuestos", x => x.repuesto_id);
                    table.ForeignKey(
                        name: "FK_replacement_categories_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categories",
                        principalColumn: "categoriaid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_replacement_manufacturer_fabricante_id",
                        column: x => x.fabricante_id,
                        principalTable: "manufacturer",
                        principalColumn: "fabricanteid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_replacement_vehicle_types_tipo_vehiculo_id",
                        column: x => x.tipo_vehiculo_id,
                        principalTable: "vehicle_types",
                        principalColumn: "tipo_vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    departamento_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ciudad", x => x.id);
                    table.ForeignKey(
                        name: "fk_ciudad_departamento",
                        column: x => x.departamento_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auditorias",
                columns: table => new
                {
                    auditoria_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    entidad_afectada = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    accion_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    descripcion_accion = table.Column<string>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditoria", x => x.auditoria_id);
                    table.ForeignKey(
                        name: "fk_auditoria_tipo_accion",
                        column: x => x.accion_id,
                        principalTable: "types_actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_auditoria_usuario",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ciudad_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_direccion", x => x.id);
                    table.ForeignKey(
                        name: "fk_direccion_ciudad",
                        column: x => x.ciudad_id,
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_completo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tipo_cliente_id = table.Column<int>(type: "integer", nullable: false),
                    direccion_id = table.Column<int>(type: "integer", nullable: false),
                    DireccionId = table.Column<int>(type: "integer", nullable: false),
                    TipoClienteId = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cliente", x => x.id);
                    table.ForeignKey(
                        name: "FK_customers_addresses_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customers_customers_types_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "customers_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cliente_direccion",
                        column: x => x.direccion_id,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cliente_tipo_cliente",
                        column: x => x.tipo_cliente_id,
                        principalTable: "customers_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    placa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    anio = table.Column<int>(type: "integer", nullable: false),
                    vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    kilometraje = table.Column<int>(type: "integer", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_vehiculo_id = table.Column<int>(type: "integer", nullable: false),
                    marca_vehiculo_id = table.Column<int>(type: "integer", nullable: false),
                    modelo_vehiculo_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehiculos", x => x.id);
                    table.CheckConstraint("CK_Vehiculo_Anio", "anio >= 1900 AND anio <= 2030");
                    table.CheckConstraint("CK_Vehiculo_Kilometraje", "kilometraje >= 0");
                    table.ForeignKey(
                        name: "FK_Vehiculos_Clientes_ClienteId",
                        column: x => x.cliente_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_MarcasVehiculo_MarcaVehiculoId",
                        column: x => x.marca_vehiculo_id,
                        principalTable: "vehicle_brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_ModelosVehiculo_ModeloVehiculoId",
                        column: x => x.modelo_vehiculo_id,
                        principalTable: "vehicle_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_TiposVehiculo_TipoVehiculoId",
                        column: x => x.tipo_vehiculo_id,
                        principalTable: "vehicle_types",
                        principalColumn: "tipo_vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordenes_servicio",
                columns: table => new
                {
                    orden_servicio_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha_ingreso = table.Column<DateTime>(type: "DATE", nullable: false),
                    fecha_estimada_entrega = table.Column<DateTime>(type: "DATE", nullable: false),
                    descripcion_trabajo = table.Column<string>(type: "text", nullable: true),
                    vehiculo_id = table.Column<int>(type: "integer", nullable: false),
                    mecanico_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_serv_id = table.Column<int>(type: "integer", nullable: false),
                    estado_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orden_servicio", x => x.orden_servicio_id);
                    table.ForeignKey(
                        name: "fk_orden_servicio_estado",
                        column: x => x.estado_id,
                        principalTable: "services_status",
                        principalColumn: "estado_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orden_servicio_mecanico",
                        column: x => x.mecanico_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orden_servicio_tipo_servicio",
                        column: x => x.tipo_serv_id,
                        principalTable: "service_types",
                        principalColumn: "tipo_servid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orden_servicio_vehiculo",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bills",
                columns: table => new
                {
                    factura_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha = table.Column<DateTime>(type: "date", nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    numero_factura = table.Column<string>(type: "text", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    orden_servicio_id = table.Column<int>(type: "integer", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    pago_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_facturas", x => x.factura_id);
                    table.CheckConstraint("CK_Factura_Total_Positive", "total >= 0");
                    table.ForeignKey(
                        name: "FK_bills_customers_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bills_payment_types_pago_id",
                        column: x => x.pago_id,
                        principalTable: "payment_types",
                        principalColumn: "tipo_pago_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orden_servicio_factura",
                        column: x => x.orden_servicio_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "orden_servicio_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders_details",
                columns: table => new
                {
                    detalle_orden_id = table.Column<int>(type: "integer", nullable: false),
                    orden_servicio_id = table.Column<int>(type: "integer", nullable: false),
                    repuesto_id = table.Column<int>(type: "integer", nullable: true),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    cantidad = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    precio_unitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    precio_mano_de_obra = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_detail", x => new { x.detalle_orden_id, x.orden_servicio_id });
                    table.CheckConstraint("ck_order_detail_cantidad", "cantidad > 0");
                    table.CheckConstraint("ck_order_detail_mano", "precio_mano_de_obra >= 0");
                    table.CheckConstraint("ck_order_detail_pu", "precio_unitario >= 0");
                    table.ForeignKey(
                        name: "fk_orden_servicio_detalle",
                        column: x => x.orden_servicio_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "orden_servicio_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_detail_repuesto",
                        column: x => x.repuesto_id,
                        principalTable: "replacement",
                        principalColumn: "repuesto_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_direcciones_ciudad_id",
                table: "addresses",
                column: "ciudad_id");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_accion_id",
                table: "auditorias",
                column: "accion_id");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_fecha_hora",
                table: "auditorias",
                column: "fecha_hora");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_usuario_id",
                table: "auditorias",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "i_x_facturas_cliente_id",
                table: "bills",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "i_x_facturas_orden_servicio_id",
                table: "bills",
                column: "orden_servicio_id");

            migrationBuilder.CreateIndex(
                name: "i_x_facturas_tipo_pago_id",
                table: "bills",
                column: "pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_cities_departamento_id",
                table: "cities",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "ix_pais_nombre",
                table: "countries",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_customers_direccion_id",
                table: "customers",
                column: "direccion_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_DireccionId",
                table: "customers",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_tipo_cliente_id",
                table: "customers",
                column: "tipo_cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_TipoClienteId",
                table: "customers",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "ix_tipo_cliente_nombre",
                table: "customers_types",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "i_x_ordenes_servicio_mecanico_id",
                table: "ordenes_servicio",
                column: "mecanico_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ordenes_servicio_vehiculo_id",
                table: "ordenes_servicio",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "ix_orden_servicio_estado_id",
                table: "ordenes_servicio",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "ix_orden_servicio_fecha_ingreso",
                table: "ordenes_servicio",
                column: "fecha_ingreso");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_servicio_tipo_serv_id",
                table: "ordenes_servicio",
                column: "tipo_serv_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_detail_orden_servicio_id",
                table: "orders_details",
                column: "orden_servicio_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_detail_repuesto_id",
                table: "orders_details",
                column: "repuesto_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id",
                table: "RefreshTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_repuestos_categoria_id",
                table: "replacement",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "i_x_repuestos_fabricante_id",
                table: "replacement",
                column: "fabricante_id");

            migrationBuilder.CreateIndex(
                name: "i_x_repuestos_tipo_vehiculo_id",
                table: "replacement",
                column: "tipo_vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "ix_departamento_nombre",
                table: "states",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "ix_departamento_pais_id",
                table: "states",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "ix_tipo_accion_nombre_accion",
                table: "types_actions",
                column: "nombre_accion");

            migrationBuilder.CreateIndex(
                name: "ix_user_member_rol_rol_id",
                table: "user_member_roles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_member_rol_user_member_id",
                table: "user_member_roles",
                column: "user_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_members_email",
                table: "users_members",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_members_username",
                table: "users_members",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_usuarios_estado_usuario_id",
                table: "usuarios",
                column: "estado_usuario_id");

            migrationBuilder.CreateIndex(
                name: "i_x_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "ix_marca_vehiculo_nombre",
                table: "vehicle_brands",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "i_x_vehiculos_cliente_id",
                table: "vehiculos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehiculos_marca_vehiculo_id",
                table: "vehiculos",
                column: "marca_vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehiculos_modelo_vehiculo_id",
                table: "vehiculos",
                column: "modelo_vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehiculos_tipo_vehiculo_id",
                table: "vehiculos",
                column: "tipo_vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_placa",
                table: "vehiculos",
                column: "placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_vin",
                table: "vehiculos",
                column: "vin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditorias");

            migrationBuilder.DropTable(
                name: "bills");

            migrationBuilder.DropTable(
                name: "orders_details");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "user_member_roles");

            migrationBuilder.DropTable(
                name: "types_actions");

            migrationBuilder.DropTable(
                name: "payment_types");

            migrationBuilder.DropTable(
                name: "ordenes_servicio");

            migrationBuilder.DropTable(
                name: "replacement");

            migrationBuilder.DropTable(
                name: "users_members");

            migrationBuilder.DropTable(
                name: "services_status");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "service_types");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "manufacturer");

            migrationBuilder.DropTable(
                name: "estados_usuario");

            migrationBuilder.DropTable(
                name: "rols");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "vehicle_brands");

            migrationBuilder.DropTable(
                name: "vehicle_models");

            migrationBuilder.DropTable(
                name: "vehicle_types");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "customers_types");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
