-- Script completo para crear la base de datos AutoTallerManager con snake_case
-- Este script crea todas las tablas con nombres en snake_case desde el inicio

-- Crear extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Tabla: roles
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    nombre_rol VARCHAR(50) NOT NULL,
    descripcion VARCHAR(200),
    activo BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: estados_usuario
CREATE TABLE estados_usuario (
    id SERIAL PRIMARY KEY,
    nombre_estado VARCHAR(50) NOT NULL,
    descripcion VARCHAR(200),
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: users_members
CREATE TABLE users_members (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    estado_id INTEGER NOT NULL REFERENCES estados_usuario (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: user_member_roles
CREATE TABLE user_member_roles (
    id SERIAL PRIMARY KEY,
    user_member_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE CASCADE,
    rol_id INTEGER NOT NULL REFERENCES roles (id) ON DELETE CASCADE,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        UNIQUE (user_member_id, rol_id)
);

-- Tabla: refresh_tokens
CREATE TABLE refresh_tokens (
    id SERIAL PRIMARY KEY,
    token VARCHAR(500) NOT NULL UNIQUE,
    user_member_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE CASCADE,
    expires_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL,
        created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: paises
CREATE TABLE paises (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: departamentos
CREATE TABLE departamentos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(150),
    pais_id INTEGER NOT NULL REFERENCES paises (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: ciudades
CREATE TABLE ciudades (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    departamento_id INTEGER NOT NULL REFERENCES departamentos (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: direcciones
CREATE TABLE direcciones (
    id SERIAL PRIMARY KEY,
    descripcion VARCHAR(150) NOT NULL,
    pais_id INTEGER NOT NULL REFERENCES paises (id) ON DELETE RESTRICT,
    departamento_id INTEGER NOT NULL REFERENCES departamentos (id) ON DELETE RESTRICT,
    ciudad_id INTEGER NOT NULL REFERENCES ciudades (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: tipos_cliente
CREATE TABLE tipos_cliente (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: clientes
CREATE TABLE clientes (
    id SERIAL PRIMARY KEY,
    nombre_completo VARCHAR(150) NOT NULL,
    telefono VARCHAR(20),
    email VARCHAR(100),
    tipo_cliente_id INTEGER NOT NULL REFERENCES tipos_cliente (id) ON DELETE RESTRICT,
    direccion_id INTEGER NOT NULL REFERENCES direcciones (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: tipos_vehiculo
CREATE TABLE tipos_vehiculo (
    id SERIAL PRIMARY KEY,
    nombre_tipo_vehiculo VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: marcas_vehiculo
CREATE TABLE marcas_vehiculo (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: modelos_vehiculo
CREATE TABLE modelos_vehiculo (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    marca_id INTEGER NOT NULL REFERENCES marcas_vehiculo (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: vehiculos
CREATE TABLE vehiculos (
    id SERIAL PRIMARY KEY,
    placa VARCHAR(20) NOT NULL UNIQUE,
    ano INTEGER NOT NULL,
    vin VARCHAR(50) NOT NULL UNIQUE,
    kilometraje INTEGER NOT NULL DEFAULT 0,
    cliente_id INTEGER NOT NULL REFERENCES clientes (id) ON DELETE RESTRICT,
    tipo_vehiculo_id INTEGER NOT NULL REFERENCES tipos_vehiculo (id) ON DELETE RESTRICT,
    marca_id INTEGER NOT NULL REFERENCES marcas_vehiculo (id) ON DELETE RESTRICT,
    modelo_id INTEGER NOT NULL REFERENCES modelos_vehiculo (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        CHECK (kilometraje >= 0)
);

-- Tabla: tipos_servicio
CREATE TABLE tipos_servicio (
    id SERIAL PRIMARY KEY,
    nombre_tipo_serv VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: estados_servicio
CREATE TABLE estados_servicio (
    id SERIAL PRIMARY KEY,
    nombre_est_serv VARCHAR(80),
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: ordenes_servicio
CREATE TABLE ordenes_servicio (
    id SERIAL PRIMARY KEY,
    fecha_ingreso TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        fecha_estimada_entrega TIMESTAMP
    WITH
        TIME ZONE NOT NULL,
        descripcion_trabajo TEXT,
        vehiculo_id INTEGER NOT NULL REFERENCES vehiculos (id) ON DELETE RESTRICT,
        mecanico_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE RESTRICT,
        tipo_servicio_id INTEGER NOT NULL REFERENCES tipos_servicio (id) ON DELETE RESTRICT,
        estado_id INTEGER NOT NULL REFERENCES estados_servicio (id) ON DELETE RESTRICT,
        created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: categorias
CREATE TABLE categorias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: fabricantes
CREATE TABLE fabricantes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    descripcion VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(80),
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: repuestos
CREATE TABLE repuestos (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL UNIQUE,
    nombre VARCHAR(150) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    stock INTEGER NOT NULL DEFAULT 0,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    stock_minimo INTEGER NOT NULL DEFAULT 0,
    categoria_id INTEGER NOT NULL REFERENCES categorias (id) ON DELETE RESTRICT,
    tipo_vehiculo_id INTEGER NOT NULL REFERENCES tipos_vehiculo (id) ON DELETE RESTRICT,
    fabricante_id INTEGER NOT NULL REFERENCES fabricantes (id) ON DELETE RESTRICT,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        CHECK (stock >= 0),
        CHECK (precio_unitario >= 0),
        CHECK (stock_minimo >= 0)
);

-- Tabla: detalle_ordenes
CREATE TABLE detalle_ordenes (
    id SERIAL PRIMARY KEY,
    orden_servicio_id INTEGER NOT NULL REFERENCES ordenes_servicio (id) ON DELETE CASCADE,
    repuesto_id INTEGER NOT NULL REFERENCES repuestos (id) ON DELETE RESTRICT,
    cantidad INTEGER NOT NULL DEFAULT 1,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        CHECK (cantidad > 0),
        CHECK (precio_unitario >= 0)
);

-- Tabla: tipos_pago
CREATE TABLE tipos_pago (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: facturas
CREATE TABLE facturas (
    id SERIAL PRIMARY KEY,
    numero_factura VARCHAR(50) NOT NULL UNIQUE,
    fecha_factura TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        subtotal DECIMAL(10, 2) NOT NULL DEFAULT 0,
        impuestos DECIMAL(10, 2) NOT NULL DEFAULT 0,
        total DECIMAL(10, 2) NOT NULL DEFAULT 0,
        cliente_id INTEGER NOT NULL REFERENCES clientes (id) ON DELETE RESTRICT,
        orden_servicio_id INTEGER NOT NULL REFERENCES ordenes_servicio (id) ON DELETE RESTRICT,
        tipo_pago_id INTEGER NOT NULL REFERENCES tipos_pago (id) ON DELETE RESTRICT,
        created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        CHECK (subtotal >= 0),
        CHECK (impuestos >= 0),
        CHECK (total >= 0)
);

-- Tabla: tipos_accion
CREATE TABLE tipos_accion (
    id SERIAL PRIMARY KEY,
    nombre_accion VARCHAR(100) NOT NULL,
    created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla: auditorias
CREATE TABLE auditorias (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE RESTRICT,
    entidad_afectada VARCHAR(50) NOT NULL,
    accion_id INTEGER NOT NULL REFERENCES tipos_accion (id) ON DELETE RESTRICT,
    fecha_hora TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        descripcion_accion TEXT,
        created_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW(),
        updated_at TIMESTAMP
    WITH
        TIME ZONE NOT NULL DEFAULT NOW()
);

-- Crear índices para mejorar el rendimiento
CREATE INDEX idx_clientes_email ON clientes (email);

CREATE INDEX idx_clientes_telefono ON clientes (telefono);

CREATE INDEX idx_vehiculos_placa ON vehiculos (placa);

CREATE INDEX idx_vehiculos_vin ON vehiculos (vin);

CREATE INDEX idx_repuestos_codigo ON repuestos (codigo);

CREATE INDEX idx_facturas_numero ON facturas (numero_factura);

CREATE INDEX idx_ordenes_servicio_fecha ON ordenes_servicio (fecha_ingreso);

CREATE INDEX idx_auditorias_fecha ON auditorias (fecha_hora);

-- Mensaje de confirmación
SELECT 'Base de datos AutoTallerManager creada exitosamente con snake_case' AS mensaje;