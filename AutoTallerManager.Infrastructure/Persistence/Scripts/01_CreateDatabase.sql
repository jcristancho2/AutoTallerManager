-- Complete script to create the AutoTallerManager database in English with snake_case
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Table: roles
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL,
    description VARCHAR(200),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: user_statuses
CREATE TABLE user_statuses (
    id SERIAL PRIMARY KEY,
    status_name VARCHAR(50) NOT NULL,
    description VARCHAR(200),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: users_members
CREATE TABLE users_members (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    status_id INTEGER NOT NULL REFERENCES user_statuses (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: user_member_roles
CREATE TABLE user_member_roles (
    id SERIAL PRIMARY KEY,
    user_member_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE CASCADE,
    role_id INTEGER NOT NULL REFERENCES roles (id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE (user_member_id, role_id)
);

-- Table: refresh_tokens
CREATE TABLE refresh_tokens (
    id SERIAL PRIMARY KEY,
    token VARCHAR(500) NOT NULL UNIQUE,
    user_member_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE CASCADE,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: countries
CREATE TABLE countries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: states
CREATE TABLE states (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150),
    country_id INTEGER NOT NULL REFERENCES countries (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: cities
CREATE TABLE cities (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    state_id INTEGER NOT NULL REFERENCES states (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: addresses
CREATE TABLE addresses (
    id SERIAL PRIMARY KEY,
    description VARCHAR(150) NOT NULL,
    country_id INTEGER NOT NULL REFERENCES countries (id) ON DELETE RESTRICT,
    state_id INTEGER NOT NULL REFERENCES states (id) ON DELETE RESTRICT,
    city_id INTEGER NOT NULL REFERENCES cities (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: customer_types
CREATE TABLE customer_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: customers
CREATE TABLE customers (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(150) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    customer_type_id INTEGER NOT NULL REFERENCES customer_types (id) ON DELETE RESTRICT,
    address_id INTEGER NOT NULL REFERENCES addresses (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: vehicle_types
CREATE TABLE vehicle_types (
    id SERIAL PRIMARY KEY,
    vehicle_type_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: vehicle_brands
CREATE TABLE vehicle_brands (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: vehicle_models
CREATE TABLE vehicle_models (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    brand_id INTEGER NOT NULL REFERENCES vehicle_brands (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: vehicles
CREATE TABLE vehicles (
    id SERIAL PRIMARY KEY,
    plate VARCHAR(20) NOT NULL UNIQUE,
    year INTEGER NOT NULL,
    vin VARCHAR(50) NOT NULL UNIQUE,
    mileage INTEGER NOT NULL DEFAULT 0,
    customer_id INTEGER NOT NULL REFERENCES customers (id) ON DELETE RESTRICT,
    vehicle_type_id INTEGER NOT NULL REFERENCES vehicle_types (id) ON DELETE RESTRICT,
    brand_id INTEGER NOT NULL REFERENCES vehicle_brands (id) ON DELETE RESTRICT,
    model_id INTEGER NOT NULL REFERENCES vehicle_models (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CHECK (mileage >= 0)
);

-- Table: service_types
CREATE TABLE service_types (
    id SERIAL PRIMARY KEY,
    service_type_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: service_statuses
CREATE TABLE service_statuses (
    id SERIAL PRIMARY KEY,
    service_status_name VARCHAR(80),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: service_orders
CREATE TABLE service_orders (
    id SERIAL PRIMARY KEY,
    entry_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    estimated_delivery_date TIMESTAMP WITH TIME ZONE NOT NULL,
    work_description TEXT,
    vehicle_id INTEGER NOT NULL REFERENCES vehicles (id) ON DELETE RESTRICT,
    mechanic_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE RESTRICT,
    service_type_id INTEGER NOT NULL REFERENCES service_types (id) ON DELETE RESTRICT,
    status_id INTEGER NOT NULL REFERENCES service_statuses (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: categories
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: manufacturers
CREATE TABLE manufacturers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(255),
    phone VARCHAR(20),
    email VARCHAR(80),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: spare_parts
CREATE TABLE spare_parts (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(255) NOT NULL,
    stock INTEGER NOT NULL DEFAULT 0,
    unit_price DECIMAL(10, 2) NOT NULL,
    stock_min INTEGER NOT NULL DEFAULT 0,
    category_id INTEGER NOT NULL REFERENCES categories (id) ON DELETE RESTRICT,
    vehicle_type_id INTEGER NOT NULL REFERENCES vehicle_types (id) ON DELETE RESTRICT,
    manufacturer_id INTEGER NOT NULL REFERENCES manufacturers (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CHECK (stock >= 0),
    CHECK (unit_price >= 0),
    CHECK (stock_min >= 0)
);

-- Table: order_details
CREATE TABLE order_details (
    id SERIAL PRIMARY KEY,
    service_order_id INTEGER NOT NULL REFERENCES service_orders (id) ON DELETE CASCADE,
    spare_part_id INTEGER NOT NULL REFERENCES spare_parts (id) ON DELETE RESTRICT,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CHECK (quantity > 0),
    CHECK (unit_price >= 0)
);

-- Table: payment_types
CREATE TABLE payment_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: invoices
CREATE TABLE invoices (
    id SERIAL PRIMARY KEY,
    invoice_number VARCHAR(50) NOT NULL UNIQUE,
    invoice_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    subtotal DECIMAL(10, 2) NOT NULL DEFAULT 0,
    taxes DECIMAL(10, 2) NOT NULL DEFAULT 0,
    total DECIMAL(10, 2) NOT NULL DEFAULT 0,
    customer_id INTEGER NOT NULL REFERENCES customers (id) ON DELETE RESTRICT,
    service_order_id INTEGER NOT NULL REFERENCES service_orders (id) ON DELETE RESTRICT,
    payment_type_id INTEGER NOT NULL REFERENCES payment_types (id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CHECK (subtotal >= 0),
    CHECK (taxes >= 0),
    CHECK (total >= 0)
);

-- Table: action_types
CREATE TABLE action_types (
    id SERIAL PRIMARY KEY,
    action_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Table: audits
CREATE TABLE audits (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users_members (id) ON DELETE RESTRICT,
    affected_entity VARCHAR(50) NOT NULL,
    action_id INTEGER NOT NULL REFERENCES action_types (id) ON DELETE RESTRICT,
    action_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    action_description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_customers_email ON customers (email);
CREATE INDEX idx_customers_phone ON customers (phone);
CREATE INDEX idx_vehicles_plate ON vehicles (plate);
CREATE INDEX idx_vehicles_vin ON vehicles (vin);
CREATE INDEX idx_spare_parts_code ON spare_parts (code);
CREATE INDEX idx_invoices_number ON invoices (invoice_number);
CREATE INDEX idx_service_orders_date ON service_orders (entry_date);
CREATE INDEX idx_audits_date ON audits (action_date);

-- Confirmation message
SELECT 'AutoTallerManager database created successfully with English snake_case' AS message;
