# 🚀 Guía Completa de Pruebas - AutoTallerManager API

## 📋 **Análisis del Proyecto**

### **Arquitectura del Sistema**
- **Backend**: ASP.NET Core 9.0 con arquitectura Clean Architecture
- **Base de Datos**: PostgreSQL 16 con Entity Framework Core
- **Autenticación**: JWT con refresh tokens
- **Rate Limiting**: Configurado por endpoint específico
- **Containerización**: Docker Compose con servicios separados

### **Estructura de Capas**
```
AutoTallerManager.API/          # Capa de presentación (Controllers, DTOs, Middleware)
AutoTallerManager.Application/  # Lógica de negocio (Commands, Queries, Services)
AutoTallerManager.Domain/       # Entidades y reglas de negocio
AutoTallerManager.Infrastructure/ # Persistencia y repositorios
AutoTallerManager.Tests/        # Pruebas unitarias e integración
```

---

## 🔄 **CAMBIOS REALIZADOS - UNIFICACIÓN DE CONTROLADORES**

### **✅ Problema Resuelto: Múltiples Controladores de Autenticación**

#### **Antes (❌ Problemático):**
- `UsuarioController` en `/api/usuario/*` - Endpoints de autenticación dispersos
- `InitController` en `/api/init/create-admin` - Creación de admin separada
- **Problema**: Endpoints inconsistentes y duplicación de lógica

#### **Después (✅ Solucionado):**
- **`AuthController` unificado** en `/api/auth/*` - Todos los endpoints de autenticación
- **`InitController` simplificado** - Solo funciones básicas del sistema
- **Beneficio**: Arquitectura limpia y consistente

#### **Endpoints Migrados:**
```http
# ANTES (❌ No funcionan):
POST /api/usuario/login              → POST /api/auth/login
POST /api/usuario/register           → POST /api/auth/register
POST /api/usuario/refresh-token      → POST /api/auth/refresh-token
GET  /api/usuario/roles              → GET  /api/auth/roles
GET  /api/usuario                    → GET  /api/auth/users
POST /api/init/create-admin          → POST /api/auth/setup/admin
POST /api/init/seed-basic            → POST /api/auth/setup/seed-data

# NUEVOS ENDPOINTS AGREGADOS:
POST /api/auth/logout                # ✅ Logout
GET  /api/auth/health                # ✅ Health check
PUT  /api/auth/users/{id}            # ✅ Actualizar usuario
PUT  /api/auth/users/{id}/change-password # ✅ Cambiar contraseña
POST /api/auth/users/assign-role     # ✅ Asignar rol
```

#### **Credenciales Actualizadas:**
- **Email admin**: `admin@taller.com` → `admin@autotaller.com`
- **Password admin**: `Admin123!` → `admin123`

---

## ⚠️ **PROBLEMAS IDENTIFICADOS Y SOLUCIONES**

### **1. Problemas de Configuración de Base de Datos**

#### **🔴 Problema**: Inconsistencia en puertos PostgreSQL
- **Local**: Puerto 5433 en `appsettings.json`
- **Docker**: Puerto 5434 en `docker-compose.yml`
- **Contenedor API**: Puerto 5432 interno

#### **✅ Solución**:
```bash
# Para desarrollo local, usar puerto 5433
# Para Docker, usar puerto 5434
# Verificar que PostgreSQL esté ejecutándose en el puerto correcto
```

### **2. Problemas de Autenticación**

#### **🔴 Problema**: Endpoints de autenticación inconsistentes (RESUELTO)
- ~~`UsuarioController` en `/api/usuario/login`~~ ❌ **ELIMINADO**
- ~~`InitController` para crear admin en `/api/init/create-admin`~~ ❌ **ELIMINADO**
- **NUEVO**: `AuthController` unificado en `/api/auth/*` ✅ **IMPLEMENTADO**

#### **✅ Solución Implementada**:
```http
# NUEVOS ENDPOINTS UNIFICADOS:
POST /api/auth/login                    # ✅ Login unificado
POST /api/auth/register                 # ✅ Registro unificado
POST /api/auth/setup/admin              # ✅ Crear admin inicial
POST /api/auth/setup/seed-data          # ✅ Inicializar datos básicos
GET  /api/auth/health                   # ✅ Health check del sistema
```

### **3. Problemas de Rate Limiting**

#### **🔴 Problema**: Configuración compleja con múltiples políticas
- Políticas específicas por endpoint pueden causar confusión
- Límites diferentes pueden afectar las pruebas

#### **✅ Solución**:
```bash
# Rate Limits configurados:
# - Auth: 10 req/min
# - OrdenesServicio: 60 req/min  
# - Repuestos: 30 req/min
# - Facturas: 20 req/min
# - Global: 100 req/min
```

### **4. Problemas de CORS**

#### **🔴 Problema**: Configuración CORS muy permisiva
```csharp
.AllowAnyOrigin()   // ⚠️ Permite cualquier origen
.AllowAnyMethod()   // ⚠️ Permite cualquier método
.AllowAnyHeader()   // ⚠️ Permite cualquier header
```

#### **✅ Solución**: Configuración más restrictiva para producción

---

## 🛠️ **CONFIGURACIÓN INICIAL**

### **Opción 1: Desarrollo Local**

#### **1.1 Configurar PostgreSQL Local**
```bash
# Instalar PostgreSQL en puerto 5433
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Crear base de datos
sudo -u postgres psql
CREATE DATABASE autotallerdb;
CREATE USER postgres WITH PASSWORD 'postgres';
GRANT ALL PRIVILEGES ON DATABASE autotallerdb TO postgres;
\q
```

#### **1.2 Aplicar Migraciones**
```bash
cd /home/raucrow/jc2dev/AutoTallerManager
dotnet ef database update --project AutoTallerManager.Infrastructure --startup-project AutoTallerManager.API
```

#### **1.3 Ejecutar Scripts de Inicialización**
```bash
# Ejecutar script de datos iniciales
psql -h localhost -p 5433 -U postgres -d autotallerdb -f AutoTallerManager.Infrastructure/Persistence/Scripts/02_SeedData.sql
```

#### **1.4 Iniciar la API**
```bash
cd /home/raucrow/jc2dev/AutoTallerManager
dotnet run --project AutoTallerManager.API
```

**URLs de acceso**:
- API: `http://localhost:5013` o `https://localhost:7061`
- Swagger: `http://localhost:5013` (configurado en raíz)

### **Opción 2: Docker Compose**

#### **2.1 Levantar Servicios**
```bash
cd /home/raucrow/jc2dev/AutoTallerManager
docker-compose -f docker/docker-compose.yml up -d
```

#### **2.2 Verificar Estado de Servicios**
```bash
# Verificar que los contenedores estén ejecutándose
docker-compose -f docker/docker-compose.yml ps

# Ver logs de la API
docker-compose -f docker/docker-compose.yml logs -f api

# Ver logs de la base de datos
docker-compose -f docker/docker-compose.yml logs -f db
```

#### **2.3 Aplicar Migraciones en Docker**
```bash
# Ejecutar migraciones dentro del contenedor
docker exec autotaller_api dotnet ef database update --project AutoTallerManager.Infrastructure --startup-project AutoTallerManager.API
```

#### **2.4 Ejecutar Scripts de Inicialización**
```bash
# Ejecutar script de datos iniciales
docker exec -i autotaller_db psql -U postgres -d autotallerdb < AutoTallerManager.Infrastructure/Persistence/Scripts/02_SeedData.sql
```

**URLs de acceso**:
- API: `http://localhost:8081`
- Base de datos: `localhost:5434`

---

## 🔐 **AUTENTICACIÓN Y AUTORIZACIÓN**

### **Paso 1: Crear Usuario Administrador**

#### **Opción A: Usar AuthController (Recomendado)**
```http
POST http://localhost:5013/api/auth/setup/admin
Content-Type: application/json

# Respuesta esperada:
{
  "message": "Usuario administrador creado exitosamente",
  "email": "admin@autotaller.com",
  "password": "admin123",
  "userId": 1,
  "roleId": 1
}
```

#### **Opción B: Usar Script SQL**
```sql
-- El script 02_SeedData.sql ya incluye un usuario admin
-- Email: admin@autotaller.com
-- Password: admin123
```

### **Paso 2: Obtener Token JWT**
```http
POST http://localhost:5013/api/auth/login
Content-Type: application/json

{
  "email": "admin@autotaller.com",
  "password": "admin123"
}
```

**Respuesta esperada**:
```json
{
  "id": 1,
  "email": "admin@autotaller.com",
  "rolNombre": "Admin",
  "estadoNombre": "Activo",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123..."
}
```

### **Paso 3: Usar Token en Requests**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📊 **PRUEBAS DE ENDPOINTS POR MÓDULO**

### **1. MÓDULO DE INICIALIZACIÓN**

#### **1.1 Test de Conectividad**
```http
GET http://localhost:5013/api/init/test
```

#### **1.2 Estado de la Base de Datos**
```http
GET http://localhost:5013/api/init/database-status
```

#### **1.3 Información del Sistema**
```http
GET http://localhost:5013/api/init/system-info
```

### **2. MÓDULO DE AUTENTICACIÓN UNIFICADO**

#### **2.1 Crear Usuario Administrador Inicial**
```http
POST http://localhost:5013/api/auth/setup/admin
Content-Type: application/json
```

#### **2.2 Inicializar Datos Básicos del Sistema**
```http
POST http://localhost:5013/api/auth/setup/seed-data
Authorization: Bearer {token}
Content-Type: application/json
```

#### **2.3 Health Check del Sistema**
```http
GET http://localhost:5013/api/auth/health
```

#### **2.4 Login de Usuario**
```http
POST http://localhost:5013/api/auth/login
Content-Type: application/json

{
  "email": "admin@autotaller.com",
  "password": "admin123"
}
```

#### **2.5 Registrar Nuevo Usuario**
```http
POST http://localhost:5013/api/auth/register
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "mecanico@taller.com",
  "password": "Mecanico123!",
  "username": "mecanico"
}
```

#### **2.6 Renovar Token**
```http
POST http://localhost:5013/api/auth/refresh-token
```

#### **2.7 Logout**
```http
POST http://localhost:5013/api/auth/logout
Authorization: Bearer {token}
```

#### **2.8 Obtener Roles Disponibles**
```http
GET http://localhost:5013/api/auth/roles
```

#### **2.9 Obtener Todos los Usuarios**
```http
GET http://localhost:5013/api/auth/users
Authorization: Bearer {token}
```

#### **2.10 Obtener Usuario por ID**
```http
GET http://localhost:5013/api/auth/users/1
Authorization: Bearer {token}
```

#### **2.11 Actualizar Usuario**
```http
PUT http://localhost:5013/api/auth/users/2
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "mecanico.actualizado@taller.com"
}
```

#### **2.12 Cambiar Contraseña**
```http
PUT http://localhost:5013/api/auth/users/2/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "newPassword": "NuevaPassword123!"
}
```

#### **2.13 Asignar Rol a Usuario**
```http
POST http://localhost:5013/api/auth/users/assign-role
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 2,
  "roleId": 2
}
```

### **3. MÓDULO DE CLIENTES**

#### **3.1 Crear Cliente**
```http
POST http://localhost:5013/api/clientes
Authorization: Bearer {token}
Content-Type: application/json

{
  "nombreCompleto": "Juan Carlos Pérez",
  "telefono": "3001234567",
  "email": "juan.perez@email.com",
  "tipoCliente_Id": 1,
  "direccion_Id": 1
}
```

#### **3.2 Obtener Clientes con Paginación**
```http
GET http://localhost:5013/api/clientes?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### **3.3 Obtener Cliente por ID**
```http
GET http://localhost:5013/api/clientes/1
Authorization: Bearer {token}
```

#### **3.4 Actualizar Cliente**
```http
PUT http://localhost:5013/api/clientes/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "nombreCompleto": "Juan Carlos Pérez García",
  "telefono": "3001234567",
  "email": "juan.perez@email.com",
  "tipoCliente_Id": 1,
  "direccion_Id": 1
}
```

#### **3.5 Registrar Cliente con Vehículo**
```http
POST http://localhost:5013/api/clientes/registrar-con-vehiculo
Authorization: Bearer {token}
Content-Type: application/json

{
  "nombreCompleto": "María González",
  "telefono": "3009876543",
  "email": "maria.gonzalez@email.com",
  "tipoCliente_Id": 1,
  "vehiculos": [
    {
      "placa": "ABC123",
      "anio": 2020,
      "vin": "1HGBH41JXMN109186",
      "kilometraje": 50000,
      "tipoVehiculoId": 1,
      "marcaVehiculoId": 1,
      "modeloVehiculoId": 1
    }
  ]
}
```

### **4. MÓDULO DE VEHÍCULOS**

#### **4.1 Crear Vehículo**
```http
POST http://localhost:5013/api/vehiculos
Authorization: Bearer {token}
Content-Type: application/json

{
  "placa": "XYZ789",
  "anio": 2021,
  "vin": "1HGBH41JXMN109187",
  "kilometraje": 30000,
  "clienteId": 1,
  "tipoVehiculoId": 1,
  "marcaVehiculoId": 1,
  "modeloVehiculoId": 1
}
```

#### **4.2 Obtener Vehículos**
```http
GET http://localhost:5013/api/vehiculos?pageNumber=1&pageSize=10&clienteId=1
Authorization: Bearer {token}
```

#### **4.3 Buscar Vehículo por VIN**
```http
GET http://localhost:5013/api/vehiculos/vin/1HGBH41JXMN109186
Authorization: Bearer {token}
```

#### **4.4 Obtener Vehículos por Cliente**
```http
GET http://localhost:5013/api/vehiculos/cliente/1
Authorization: Bearer {token}
```

#### **4.5 Actualizar Vehículo**
```http
PUT http://localhost:5013/api/vehiculos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "placa": "ABC123",
  "anio": 2020,
  "vin": "1HGBH41JXMN109186",
  "kilometraje": 55000,
  "clienteId": 1,
  "tipoVehiculoId": 1,
  "marcaVehiculoId": 1,
  "modeloVehiculoId": 1
}
```

### **5. MÓDULO DE REPUESTOS**

#### **5.1 Crear Repuesto**
```http
POST http://localhost:5013/api/repuestos
Authorization: Bearer {token}
Content-Type: application/json

{
  "codigo": "FIL001",
  "nombreRepu": "Filtro de Aceite",
  "descripcion": "Filtro de aceite para motor",
  "stock": 50,
  "precioUnitario": 25000,
  "categoriaId": 1,
  "tipoVehiculoId": 1,
  "fabricanteId": 1
}
```

#### **5.2 Obtener Repuestos**
```http
GET http://localhost:5013/api/repuestos?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

#### **5.3 Buscar Repuesto por Código**
```http
GET http://localhost:5013/api/repuestos/codigo/FIL001
Authorization: Bearer {token}
```

#### **5.4 Obtener Repuestos con Stock Bajo**
```http
GET http://localhost:5013/api/repuestos/stock/bajo?stockMinimo=5
Authorization: Bearer {token}
```

#### **5.5 Actualizar Repuesto**
```http
PUT http://localhost:5013/api/repuestos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "codigo": "FIL001",
  "nombreRepu": "Filtro de Aceite Premium",
  "descripcion": "Filtro de aceite de alta calidad",
  "stock": 60,
  "precioUnitario": 30000,
  "categoriaId": 1,
  "tipoVehiculoId": 1,
  "fabricanteId": 1
}
```

### **6. MÓDULO DE ÓRDENES DE SERVICIO**

#### **6.1 Crear Orden de Servicio**
```http
POST http://localhost:5013/api/ordenesservicio
Authorization: Bearer {token}
Content-Type: application/json

{
  "fechaIngreso": "2024-01-15T08:00:00Z",
  "fechaEstimadaEntrega": "2024-01-16T17:00:00Z",
  "vehiculoId": 1,
  "mecanicoId": 1,
  "tipoServId": 1,
  "estadoId": 1
}
```

#### **6.2 Obtener Órdenes de Servicio**
```http
GET http://localhost:5013/api/ordenesservicio?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### **6.3 Obtener Orden por ID**
```http
GET http://localhost:5013/api/ordenesservicio/1
Authorization: Bearer {token}
```

#### **6.4 Agregar Detalle a Orden**
```http
POST http://localhost:5013/api/ordenesservicio/1/detalles
Authorization: Bearer {token}
Content-Type: application/json

{
  "repuestoId": 1,
  "cantidad": 2,
  "descripcion": "Cambio de filtro de aceite",
  "precioUnitario": 25000,
  "precioManoDeObra": 15000
}
```

#### **6.5 Actualizar Detalle de Orden**
```http
PUT http://localhost:5013/api/ordenesservicio/1/detalles/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "repuestoId": 1,
  "cantidad": 3,
  "descripcion": "Cambio de filtro de aceite - cantidad actualizada",
  "precioUnitario": 25000,
  "precioManoDeObra": 15000
}
```

#### **6.6 Actualizar Estado de Orden**
```http
PUT http://localhost:5013/api/ordenesservicio/1/estado
Authorization: Bearer {token}
Content-Type: application/json

{
  "estadoId": 2
}
```

#### **6.7 Cerrar Orden (Generar Factura)**
```http
POST http://localhost:5013/api/ordenesservicio/1/cerrar
Authorization: Bearer {token}
Content-Type: application/json

{
  "tipoPagoId": 1
}
```

### **7. MÓDULO DE FACTURAS**

#### **7.1 Obtener Facturas**
```http
GET http://localhost:5013/api/facturas?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### **7.2 Obtener Factura por ID**
```http
GET http://localhost:5013/api/facturas/1
Authorization: Bearer {token}
```

#### **7.3 Obtener Facturas por Cliente**
```http
GET http://localhost:5013/api/facturas/cliente/1
Authorization: Bearer {token}
```

#### **7.4 Obtener Ingresos por Período**
```http
GET http://localhost:5013/api/facturas/ingresos?fechaDesde=2024-01-01&fechaHasta=2024-12-31
Authorization: Bearer {token}
```

#### **7.5 Crear Factura Manual**
```http
POST http://localhost:5013/api/facturas
Authorization: Bearer {token}
Content-Type: application/json

{
  "fecha": "2024-01-15T10:00:00Z",
  "total": 65000,
  "ordenServicioId": 1,
  "clienteId": 1,
  "tipoPagoId": 1
}
```

---

## 🧪 **PRUEBAS DE CASOS ESPECIALES**

### **1. Pruebas de Rate Limiting**

#### **1.1 Probar Límite de Autenticación**
```bash
# Ejecutar múltiples requests rápidamente
for i in {1..12}; do
  curl -X POST http://localhost:5013/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@autotaller.com","password":"admin123"}'
  echo "Request $i"
done
# Debería devolver 429 después del request 10
```

#### **1.2 Probar Límite de Repuestos**
```bash
# Ejecutar múltiples requests rápidamente
for i in {1..35}; do
  curl -H "Authorization: Bearer {token}" \
    http://localhost:5013/api/repuestos
  echo "Request $i"
done
# Debería devolver 429 después del request 30
```

### **2. Pruebas de Validación**

#### **2.1 Error: VIN Duplicado**
```http
POST http://localhost:5013/api/vehiculos
Authorization: Bearer {token}
Content-Type: application/json

{
  "placa": "XYZ789",
  "anio": 2021,
  "vin": "1HGBH41JXMN109186",  # VIN ya existente
  "kilometraje": 30000,
  "clienteId": 1,
  "tipoVehiculoId": 1,
  "marcaVehiculoId": 1,
  "modeloVehiculoId": 1
}
```

#### **2.2 Error: Stock Insuficiente**
```http
POST http://localhost:5013/api/ordenesservicio/1/detalles
Authorization: Bearer {token}
Content-Type: application/json

{
  "repuestoId": 1,
  "cantidad": 100,  # Cantidad mayor al stock disponible
  "descripcion": "Repuesto con stock insuficiente",
  "precioUnitario": 25000,
  "precioManoDeObra": 15000
}
```

#### **2.3 Error: Acceso sin Token**
```http
GET http://localhost:5013/api/clientes
# Debería devolver 401 Unauthorized
```

#### **2.4 Error: Token Expirado**
```http
GET http://localhost:5013/api/clientes
Authorization: Bearer token_expirado_invalido
# Debería devolver 401 Unauthorized
```

### **3. Pruebas de Integridad de Datos**

#### **3.1 Eliminar Cliente con Órdenes Activas**
```http
DELETE http://localhost:5013/api/clientes/1
Authorization: Bearer {token}
# Debería devolver error si tiene órdenes activas
```

#### **3.2 Eliminar Vehículo con Órdenes Activas**
```http
DELETE http://localhost:5013/api/vehiculos/1
Authorization: Bearer {token}
# Debería devolver error si tiene órdenes activas
```

---

## 🔍 **VERIFICACIÓN DE SWAGGER**

### **1. Acceder a Swagger UI**
```
http://localhost:5013  # Configurado en raíz
```

### **2. Autenticar en Swagger**
1. Haz clic en "Authorize" (🔒)
2. Introduce: `Bearer {tu_token}`
3. Haz clic en "Authorize"
4. Ahora puedes probar todos los endpoints desde Swagger

### **3. Probar Endpoints desde Swagger**
- Usar el botón "Try it out"
- Introducir los datos requeridos
- Ejecutar y ver la respuesta

---

## ⚠️ **ERRORES COMUNES Y SOLUCIONES**

### **Error 401 Unauthorized**
- **Causa**: Token expirado, inválido o ausente
- **Solución**: 
  ```bash
  # Hacer login nuevamente
  curl -X POST http://localhost:5013/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@autotaller.com","password":"admin123"}'
  ```

### **Error 403 Forbidden**
- **Causa**: Usuario sin permisos para la operación
- **Solución**: Verificar que el usuario tenga el rol correcto (Admin, Mecanico, Recepcionista)

### **Error 429 Too Many Requests**
- **Causa**: Rate limit excedido
- **Solución**: Esperar el tiempo especificado o usar un token diferente

### **Error 400 Bad Request**
- **Causa**: Datos de entrada inválidos
- **Solución**: Verificar el formato JSON y los campos requeridos

### **Error 404 Not Found**
- **Causa**: Recurso no encontrado
- **Solución**: Verificar que el ID existe en la base de datos

### **Error 500 Internal Server Error**
- **Causa**: Error interno del servidor
- **Solución**: 
  ```bash
  # Verificar logs de la aplicación
  docker-compose logs -f api
  
  # O en desarrollo local, verificar la consola
  ```

### **Error de Conexión a Base de Datos**
- **Causa**: PostgreSQL no está ejecutándose o configuración incorrecta
- **Solución**:
  ```bash
  # Verificar que PostgreSQL esté ejecutándose
  sudo systemctl status postgresql
  
  # O verificar contenedor Docker
  docker-compose ps
  ```

---

## 📝 **FLUJO DE PRUEBA COMPLETO RECOMENDADO**

### **Fase 1: Configuración Inicial**
1. ✅ Configurar base de datos (local o Docker)
2. ✅ Aplicar migraciones
3. ✅ Ejecutar scripts de inicialización
4. ✅ Iniciar la API
5. ✅ Verificar conectividad con `/api/init/test`

### **Fase 2: Autenticación**
1. ✅ Crear usuario administrador con `/api/auth/setup/admin`
2. ✅ Obtener token JWT con `/api/auth/login`
3. ✅ Verificar token con endpoint protegido

### **Fase 3: Datos Maestros**
1. ✅ Crear datos básicos con `/api/auth/setup/seed-data`
2. ✅ Verificar catálogos con `/api/auth/roles`
3. ✅ Verificar estado del sistema con `/api/auth/health`

### **Fase 4: Flujo de Negocio Completo**
1. ✅ Crear cliente
2. ✅ Crear vehículo para el cliente
3. ✅ Crear repuestos necesarios
4. ✅ Crear orden de servicio
5. ✅ Agregar detalles a la orden
6. ✅ Actualizar estado de la orden
7. ✅ Cerrar orden (genera factura automáticamente)
8. ✅ Verificar factura generada

### **Fase 5: Pruebas de Casos Especiales**
1. ✅ Probar rate limiting
2. ✅ Probar validaciones de negocio
3. ✅ Probar manejo de errores
4. ✅ Probar integridad de datos

### **Fase 6: Verificación Final**
1. ✅ Probar todos los endpoints desde Swagger
2. ✅ Verificar logs de auditoría
3. ✅ Verificar rendimiento básico

---

## 🎯 **COMANDOS ÚTILES PARA DEBUGGING**

### **Verificar Estado de Servicios**
```bash
# Docker
docker-compose ps
docker-compose logs -f api
docker-compose logs -f db

# Local
sudo systemctl status postgresql
dotnet run --project AutoTallerManager.API
```

### **Verificar Base de Datos**
```bash
# Conectar a PostgreSQL
psql -h localhost -p 5433 -U postgres -d autotallerdb

# Verificar tablas
\dt

# Verificar datos
SELECT * FROM users_members;
SELECT * FROM clientes;
SELECT * FROM vehiculos;
```

### **Verificar Logs**
```bash
# Docker
docker exec autotaller_api cat /app/logs/app.log

# Local
tail -f logs/app.log
```

---

## 📊 **MÉTRICAS DE PRUEBA**

### **Endpoints por Módulo**
- **Inicialización**: 3 endpoints (`/api/init/*`)
- **Autenticación Unificada**: 13 endpoints (`/api/auth/*`)
- **Clientes**: 6 endpoints (`/api/clientes/*`)
- **Vehículos**: 6 endpoints (`/api/vehiculos/*`)
- **Repuestos**: 6 endpoints (`/api/repuestos/*`)
- **Órdenes de Servicio**: 10 endpoints (`/api/ordenesservicio/*`)
- **Facturas**: 6 endpoints (`/api/facturas/*`)
- **Total**: 50 endpoints

### **Rate Limits Configurados**
- **Auth**: 10 req/min
- **OrdenesServicio**: 60 req/min
- **Repuestos**: 30 req/min
- **Facturas**: 20 req/min
- **Global**: 100 req/min

### **Roles de Usuario**
- **Admin**: Acceso completo
- **Mecanico**: Gestión de órdenes y facturas
- **Recepcionista**: Gestión de clientes y vehículos

---

## 🚀 **CONCLUSIONES**

### **Fortalezas del Sistema**
1. ✅ Arquitectura bien estructurada con Clean Architecture
2. ✅ **Autenticación JWT unificada** con refresh tokens
3. ✅ Rate limiting configurado por endpoint
4. ✅ Validaciones de negocio implementadas
5. ✅ Manejo de errores centralizado
6. ✅ Auditoría de operaciones
7. ✅ Documentación Swagger completa
8. ✅ **Controladores de autenticación unificados** (RESUELTO)

### **Áreas de Mejora**
1. ⚠️ Configuración CORS muy permisiva
2. ⚠️ Inconsistencia en puertos de base de datos
3. ⚠️ Falta de validación de relaciones en algunos endpoints
4. ⚠️ Configuración de rate limiting compleja

### **Recomendaciones**
1. 🔧 Configurar CORS más restrictivo para producción
2. 🔧 Estandarizar puertos de base de datos
3. 🔧 Implementar validaciones adicionales de relaciones
4. 🔧 Simplificar configuración de rate limiting
5. 🔧 Agregar más pruebas unitarias e integración
6. ✅ **Unificación de controladores completada**

---

---

## 🚀 **VERIFICACIÓN RÁPIDA - ENDPOINTS FUNCIONANDO**

### **✅ Comandos de Verificación Inmediata:**

#### **1. Verificar Conectividad:**
```bash
curl http://localhost:5013/api/init/test
# Respuesta esperada: {"message":"Sistema AutoTallerManager funcionando correctamente",...}
```

#### **2. Crear Admin Inicial:**
```bash
curl -X POST http://localhost:5013/api/auth/setup/admin
# Respuesta esperada: {"message":"Usuario administrador creado exitosamente",...}
```

#### **3. Login y Obtener Token:**
```bash
curl -X POST http://localhost:5013/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@autotaller.com","password":"admin123"}'
# Respuesta esperada: {"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",...}
```

#### **4. Verificar Health Check:**
```bash
curl http://localhost:5013/api/auth/health
# Respuesta esperada: {"status":"healthy","database":true,"adminExists":true,...}
```

#### **5. Inicializar Datos Básicos:**
```bash
curl -X POST http://localhost:5013/api/auth/setup/seed-data \
  -H "Authorization: Bearer {tu_token}"
# Respuesta esperada: {"message":"Datos básicos inicializados exitosamente",...}
```

### **✅ Estado de Endpoints:**
- **✅ Funcionando**: Todos los endpoints `/api/auth/*`
- **✅ Funcionando**: Todos los endpoints `/api/init/*`
- **✅ Funcionando**: Todos los endpoints de negocio (`/api/clientes/*`, `/api/vehiculos/*`, etc.)
- **❌ Deprecados**: Todos los endpoints `/api/usuario/*` (eliminados)

---

**¡Con esta guía puedes probar completamente todos los endpoints del sistema AutoTallerManager! 🚀**

**Fecha de actualización**: $(date)
**Versión**: 2.0 - Unificación Completa
**Autor**: Análisis completo del proyecto AutoTallerManager
**Estado**: ✅ FUNCIONANDO CORRECTAMENTE
