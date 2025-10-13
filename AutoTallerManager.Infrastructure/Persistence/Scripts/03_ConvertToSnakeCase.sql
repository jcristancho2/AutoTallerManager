-- Script para convertir todas las tablas y columnas a snake_case
-- Este script debe ejecutarse después de hacer backup de la base de datos

-- Renombrar tablas a snake_case
ALTER TABLE "EstadosUsuario" RENAME TO estados_usuario;

ALTER TABLE "Usuarios" RENAME TO usuarios;

ALTER TABLE "RefreshTokens" RENAME TO refresh_tokens;

ALTER TABLE "UserMemberRols" RENAME TO user_member_roles;

ALTER TABLE "UsersMembers" RENAME TO users_members;

ALTER TABLE "Roles" RENAME TO roles;

ALTER TABLE "TiposCliente" RENAME TO tipos_cliente;

ALTER TABLE "TiposVehiculo" RENAME TO tipos_vehiculo;

ALTER TABLE "MarcasVehiculo" RENAME TO marcas_vehiculo;

ALTER TABLE "ModelosVehiculo" RENAME TO modelos_vehiculo;

ALTER TABLE "TiposServicio" RENAME TO tipos_servicio;

ALTER TABLE "EstadosServicio" RENAME TO estados_servicio;

ALTER TABLE "TiposPago" RENAME TO tipos_pago;

ALTER TABLE "TiposAccion" RENAME TO tipos_accion;

ALTER TABLE "Categorias" RENAME TO categorias;

ALTER TABLE "Fabricantes" RENAME TO fabricantes;

ALTER TABLE "Paises" RENAME TO paises;

ALTER TABLE "Departamentos" RENAME TO departamentos;

ALTER TABLE "Ciudades" RENAME TO ciudades;

ALTER TABLE "Direcciones" RENAME TO direcciones;

ALTER TABLE "Clientes" RENAME TO clientes;

ALTER TABLE "Vehiculos" RENAME TO vehiculos;

ALTER TABLE "OrdenesServicio" RENAME TO ordenes_servicio;

ALTER TABLE "DetalleOrdenes" RENAME TO detalle_ordenes;

ALTER TABLE "Repuestos" RENAME TO repuestos;

ALTER TABLE "Facturas" RENAME TO facturas;

ALTER TABLE "Auditorias" RENAME TO auditorias;

-- Renombrar columnas en tabla roles
ALTER TABLE roles RENAME COLUMN "NombreRol" TO nombre_rol;

ALTER TABLE roles RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE roles RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE roles RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla estados_usuario
ALTER TABLE estados_usuario
RENAME COLUMN "NombreEstado" TO nombre_estado;

ALTER TABLE estados_usuario
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE estados_usuario RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE estados_usuario RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla tipos_cliente
ALTER TABLE tipos_cliente
RENAME COLUMN "NombreTipoCliente" TO nombre_tipo_cliente;

ALTER TABLE tipos_cliente RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE tipos_cliente RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE tipos_cliente RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla tipos_vehiculo
ALTER TABLE tipos_vehiculo
RENAME COLUMN "NombreTipoVehiculo" TO nombre_tipo_vehiculo;

ALTER TABLE tipos_vehiculo
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE tipos_vehiculo RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE tipos_vehiculo RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla marcas_vehiculo
ALTER TABLE marcas_vehiculo
RENAME COLUMN "NombreMarca" TO nombre_marca;

ALTER TABLE marcas_vehiculo
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE marcas_vehiculo RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE marcas_vehiculo RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla modelos_vehiculo
ALTER TABLE modelos_vehiculo
RENAME COLUMN "NombreModelo" TO nombre_modelo;

ALTER TABLE modelos_vehiculo
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE modelos_vehiculo RENAME COLUMN "MarcaId" TO marca_id;

ALTER TABLE modelos_vehiculo RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE modelos_vehiculo RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla tipos_servicio
ALTER TABLE tipos_servicio
RENAME COLUMN "NombreTipoServ" TO nombre_tipo_serv;

ALTER TABLE tipos_servicio
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE tipos_servicio RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE tipos_servicio RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla estados_servicio
ALTER TABLE estados_servicio
RENAME COLUMN "NombreEstServ" TO nombre_est_serv;

ALTER TABLE estados_servicio
RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE estados_servicio RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE estados_servicio RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla tipos_pago
ALTER TABLE tipos_pago
RENAME COLUMN "NombreTipoPag" TO nombre_tipo_pag;

ALTER TABLE tipos_pago RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE tipos_pago RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE tipos_pago RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla categorias
ALTER TABLE categorias
RENAME COLUMN "NombreCategoria" TO nombre_categoria;

ALTER TABLE categorias RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE categorias RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE categorias RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla fabricantes
ALTER TABLE fabricantes
RENAME COLUMN "NombreFabricante" TO nombre_fabricante;

ALTER TABLE fabricantes RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE fabricantes RENAME COLUMN "Telefono" TO telefono;

ALTER TABLE fabricantes RENAME COLUMN "Email" TO email;

ALTER TABLE fabricantes RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE fabricantes RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla users_members
ALTER TABLE users_members RENAME COLUMN "Username" TO username;

ALTER TABLE users_members RENAME COLUMN "Email" TO email;

ALTER TABLE users_members
RENAME COLUMN "PasswordHash" TO password_hash;

ALTER TABLE users_members RENAME COLUMN "EstadoId" TO estado_id;

ALTER TABLE users_members RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE users_members RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla user_member_roles
ALTER TABLE user_member_roles
RENAME COLUMN "UserMemberId" TO user_member_id;

ALTER TABLE user_member_roles RENAME COLUMN "RolId" TO rol_id;

ALTER TABLE user_member_roles
RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE user_member_roles
RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla repuestos
ALTER TABLE repuestos
RENAME COLUMN "CodigoRepuesto" TO codigo_repuesto;

ALTER TABLE repuestos RENAME COLUMN "NombreRepu" TO nombre_repu;

ALTER TABLE repuestos RENAME COLUMN "Descripcion" TO descripcion;

ALTER TABLE repuestos RENAME COLUMN "Stock" TO stock;

ALTER TABLE repuestos
RENAME COLUMN "PrecioUnitario" TO precio_unitario;

ALTER TABLE repuestos RENAME COLUMN "StockMinimo" TO stock_minimo;

ALTER TABLE repuestos RENAME COLUMN "CategoriaId" TO categoria_id;

ALTER TABLE repuestos RENAME COLUMN "FabricanteId" TO fabricante_id;

ALTER TABLE repuestos RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE repuestos RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla clientes
ALTER TABLE clientes RENAME COLUMN "Nombre" TO nombre;

ALTER TABLE clientes RENAME COLUMN "Apellido" TO apellido;

ALTER TABLE clientes RENAME COLUMN "Telefono" TO telefono;

ALTER TABLE clientes RENAME COLUMN "Email" TO email;

ALTER TABLE clientes
RENAME COLUMN "TipoClienteId" TO tipo_cliente_id;

ALTER TABLE clientes RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE clientes RENAME COLUMN "UpdatedAt" TO updated_at;

-- Renombrar columnas en tabla vehiculos
ALTER TABLE vehiculos RENAME COLUMN "Vin" TO vin;

ALTER TABLE vehiculos RENAME COLUMN "Ano" TO ano;

ALTER TABLE vehiculos RENAME COLUMN "Kilometraje" TO kilometraje;

ALTER TABLE vehiculos RENAME COLUMN "ClienteId" TO cliente_id;

ALTER TABLE vehiculos
RENAME COLUMN "TipoVehiculoId" TO tipo_vehiculo_id;

ALTER TABLE vehiculos RENAME COLUMN "MarcaId" TO marca_id;

ALTER TABLE vehiculos RENAME COLUMN "ModeloId" TO modelo_id;

ALTER TABLE vehiculos RENAME COLUMN "CreatedAt" TO created_at;

ALTER TABLE vehiculos RENAME COLUMN "UpdatedAt" TO updated_at;

-- Mensaje de confirmación
SELECT 'Conversión a snake_case completada exitosamente' AS mensaje;