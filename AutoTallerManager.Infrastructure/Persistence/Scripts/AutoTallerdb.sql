-- Crear base de datos
CREATE DATABASE IF NOT EXISTS AutoTallerDBV2;
USE AutoTallerDBV2;

-- Tabla: TipoCliente (Natural, Juridica)
CREATE TABLE TipoCliente(
    TipoClienteId INT AUTO_INCREMENT PRIMARY KEY,
    NombreTipoCli VARCHAR(500)
);

-- Tabla: Pais 
CREATE TABLE Pais(
    PaisId INT AUTO_INCREMENT PRIMARY KEY,
    NombrePais VARCHAR(50)
);

-- Tabla: Ciudad
CREATE TABLE Ciudad(
    CiudadId INT AUTO_INCREMENT PRIMARY KEY,
    NombreCiudad VARCHAR(50)
);

-- Tabla: Departamento
CREATE TABLE Departamento(
    DepartamentoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreDepart VARCHAR(50)
);

-- Tabla: CodPostal
CREATE TABLE CodPostal(
    CodPostalId INT AUTO_INCREMENT PRIMARY KEY,
    NombreCodPost VARCHAR(50)
);

-- Tabla: Direccion
CREATE TABLE Direccion(
    DireccionId INT AUTO_INCREMENT PRIMARY KEY,
    Descripcion VARCHAR(150) NOT NULL,
    PaisId INT NOT NULL,
    DepartamentoId INT NOT NULL,
    CiudadID INT NOT NULL,
    CodPostalId INT NOT NULL,
    FOREIGN KEY (PaisID) REFERENCES Pais(PaisId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (DepartamentoID) REFERENCES Departamento(DepartamentoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (CiudadId) REFERENCES Ciudad(CiudadId)
        ON DELETE RESTRICT ON UPDATE CASCADE,  
    FOREIGN KEY (CodPostalId) REFERENCES CodPostal(CodPostalId)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Tabla: Cliente
CREATE TABLE Cliente (
    ClienteId INT AUTO_INCREMENT PRIMARY KEY,
    NombreCliente VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    Correo VARCHAR(100) UNIQUE,
    DireccionId INT NOT NULL,
    TipoClienteId INT NOT NULL,
    FOREIGN KEY (DireccionId) REFERENCES Direccion(DireccionId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (TipoClienteId) REFERENCES TipoCliente(TipoClienteId)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Tabla: TipoVehiculo(moto, Carro, Vehiculo Pesado)
CREATE TABLE TipoVehiculo(
    TipoVehiculoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreTipoVehi VARCHAR(50)
);

-- Tabla: MarcaVehiculo
CREATE TABLE MarcaVehiculo(
    MarcaVehiculoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreMarcaVehi VARCHAR(50)
);

-- Tabla: ModeloVehiculo
CREATE TABLE ModeloVehiculo(
    ModeloVehiculoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreModVehi VARCHAR(50)
);

-- Tabla: Vehiculo - CORREGIDO: Agregada Placa
CREATE TABLE Vehiculo (
    VehiculoId INT AUTO_INCREMENT PRIMARY KEY,
    ClienteId INT NOT NULL,
    Placa VARCHAR(20) NOT NULL UNIQUE,
    Anio YEAR NOT NULL,
    VIN VARCHAR(50) NOT NULL UNIQUE,
    Kilometraje INT NOT NULL,
    TipoVehiculoId INT NOT NULL,
    MarcaVehiculoId INT NOT NULL,
    ModeloVehiculoId INT NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (TipoVehiculoId) REFERENCES TipoVehiculo(TipoVehiculoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (MarcaVehiculoId) REFERENCES MarcaVehiculo(MarcaVehiculoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ModeloVehiculoId) REFERENCES ModeloVehiculo(ModeloVehiculoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Kilometraje >= 0)
);

-- Tabla: Rol (Admin, Mecanico, Recepcionista)
CREATE TABLE Rol (
    RolId INT AUTO_INCREMENT PRIMARY KEY,
    NombreRol VARCHAR(50)
);

-- Tabla: EstadoUsuario(Activo, NoActivo)
CREATE TABLE EstadoUsuario(
    EstadoUsuarioId INT AUTO_INCREMENT PRIMARY KEY,
    NombreEstUsu VARCHAR(50)
);

-- Tabla: Usuario
CREATE TABLE Usuario (
    UsuarioId INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    RolId INT NOT NULL,
    EstadoUsuarioId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Rol(RolId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (EstadoUsuarioId) REFERENCES EstadoUsuario(EstadoUsuarioId)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Tabla: Categoria
CREATE TABLE Categoria(
    CategoriaId INT AUTO_INCREMENT PRIMARY KEY,
    NombreCat VARCHAR(50)
);

-- Tabla: Fabricante - CORREGIDO: Telefono como VARCHAR
CREATE TABLE Fabricante(
    FabricanteId INT AUTO_INCREMENT PRIMARY KEY,
    NombreFab VARCHAR(50),
    Descripcion VARCHAR(255),
    Telefono VARCHAR(20),
    Correo VARCHAR(80)
);

-- Tabla: Repuesto
CREATE TABLE Repuesto (
    RepuestoId INT AUTO_INCREMENT PRIMARY KEY,
    Codigo VARCHAR(50) NOT NULL UNIQUE,
    NombreRep VARCHAR(150) NOT NULL,
    Descripcion VARCHAR(100) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    CategoriaId INT NOT NULL,
    TipoVehiculoId INT NOT NULL,
    FabricanteId INT NOT NULL,
    FOREIGN KEY (CategoriaId) REFERENCES Categoria(categoriaId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (TipoVehiculoId) REFERENCES TipoVehiculo(TipoVehiculoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (FabricanteId) REFERENCES Fabricante(FabricanteId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Stock >= 0),
    CHECK (PrecioUnitario >= 0)
);

-- Tabla: TipoServicio('Mantenimiento', 'Reparacion', 'Diagnostico')
CREATE TABLE TipoServicio ( 
    TipoServId INT AUTO_INCREMENT PRIMARY KEY,
    NombreTipoServ VARCHAR(80) NOT NULL
);

-- Tabla: EstadoServicio(Pendiente, EnProceso, Completada, Cancelada) DEFAULT Pendiente,
CREATE TABLE EstadoServ(
    EstadoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreEstServ VARCHAR(80)
);

-- Tabla: OrdenServicio
CREATE TABLE OrdenServicio (
    OrdenServicioId INT AUTO_INCREMENT PRIMARY KEY,
    VehiculoId INT NOT NULL,
    MecanicoId INT NOT NULL,
    FechaIngreso DATE NOT NULL,
    FechaEstimadaEntrega DATE NOT NULL,
    TipoServId INT NOT NULL,
    EstadoId INT NOT NULL,
    FOREIGN KEY (VehiculoId) REFERENCES Vehiculo(VehiculoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (MecanicoId) REFERENCES Usuario(UsuarioId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (TipoServId) REFERENCES TipoServicio(TipoServId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (EstadoId) REFERENCES EstadoServ(EstadoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (FechaEstimadaEntrega >= FechaIngreso)
);

-- Tabla: DetalleOrden 
CREATE TABLE DetalleOrden (
    DetalleOrdenId INT AUTO_INCREMENT,
    OrdenServicioId INT NOT NULL,
    RepuestoId INT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    Cantidad INT NOT NULL DEFAULT 1,
    PrecioUnitario DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrecioManoDeObra DECIMAL(10,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (DetalleOrdenID, OrdenServicioId),
    FOREIGN KEY (OrdenServicioId) REFERENCES OrdenServicio(OrdenServicioId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (RepuestoId) REFERENCES Repuesto(RepuestoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Cantidad > 0),
    CHECK (PrecioUnitario >= 0),
    CHECK (PrecioManoDeObra >= 0)
);

-- Tabla: Tipo de pago (Efectivo, Tarjeta_Credito, Tarjeta_Debito, Transferencia)
CREATE TABLE TipoPago (
    PagoId INT AUTO_INCREMENT PRIMARY KEY,
    NombreTipoPag VARCHAR(80)
);

-- Tabla: Factura
CREATE TABLE Factura (
    FacturaId INT AUTO_INCREMENT PRIMARY KEY,
    OrdenServicioId INT NOT NULL UNIQUE,
    ClienteId INT NOT NULL,
    Fecha DATE NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    PagoId INT NOT NULL,
    FOREIGN KEY (OrdenServicioId) REFERENCES OrdenServicio(OrdenServicioId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (PagoId) REFERENCES TipoPago(PagoId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (Total >= 0)
);

-- Tabla: TipoAccion (crear, modificar, eliminar)
CREATE TABLE TipoAccion (
    AccionId INT AUTO_INCREMENT PRIMARY KEY,
    NombreAcci VARCHAR(80)
);

-- Tabla: Auditoria
CREATE TABLE Auditoria (
    AuditoriaId INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    EntidadAfectada VARCHAR(50) NOT NULL,
    AccionId INT NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DescripcionAccion TEXT,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (AccionId) REFERENCES TipoAccion(AccionId)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Índices para rendimiento en búsquedas frecuentes
CREATE INDEX idx_vehiculo_vin ON Vehiculo(VIN);
CREATE INDEX idx_vehiculo_placa ON Vehiculo(Placa);
CREATE INDEX idx_usuario_email ON Usuario(Email);
CREATE INDEX idx_repuesto_codigo ON Repuesto(Codigo);
CREATE INDEX idx_orden_estado ON OrdenServicio(EstadoId);
CREATE INDEX idx_orden_fecha ON OrdenServicio(FechaIngreso);
CREATE INDEX idx_factura_cliente ON Factura(ClienteId);
CREATE INDEX idx_factura_fecha ON Factura(Fecha);