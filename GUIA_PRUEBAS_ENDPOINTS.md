# 🚀 Guía Completa para Probar Endpoints de AutoTallerManager

## 📋 **Orden Correcto de Pruebas**

### **1. Preparación del Entorno**

#### **1.1 Iniciar la Base de Datos**
```bash
# Opción 1: Docker Compose
cd /home/raucrow/jc2dev/AutoTallerManager
docker-compose -f docker/docker-compose.yml up -d

# Opción 2: PostgreSQL local
# Asegúrate de que PostgreSQL esté ejecutándose en el puerto 5433
```

#### **1.2 Aplicar Migraciones**
```bash
cd /home/raucrow/jc2dev/AutoTallerManager
dotnet ef database update --project AutoTallerManager.Infrastructure --startup-project AutoTallerManager.API
```

#### **1.3 Poblar Datos Iniciales**
```bash
# Conectar a PostgreSQL y ejecutar:
psql -h localhost -p 5433 -U postgres -d autotallerdb -f AutoTallerManager.Infrastructure/Persistence/Scripts/SeedData.sql
```

#### **1.4 Iniciar la API**
```bash
cd /home/raucrow/jc2dev/AutoTallerManager
dotnet run --project AutoTallerManager.API
```

La API estará disponible en: `http://localhost:5000` o `https://localhost:5001`

---

## 🔐 **2. Autenticación (PASO OBLIGATORIO)**

### **2.1 Login del Usuario Administrador**
```http
POST http://localhost:5000/api/Usuario/login
Content-Type: application/json

{
  "email": "admin@autotaller.com",
  "password": "admin123"
}
```

**Respuesta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "expiration": "2024-12-21T10:30:00Z",
  "usuario": {
    "id": 1,
    "username": "admin",
    "email": "admin@autotaller.com",
    "estadoId": 1,
    "roles": ["Admin"]
  }
}
```

**⚠️ IMPORTANTE:** Guarda el token para usar en todas las siguientes peticiones:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📊 **3. Catálogos y Datos Maestros**

### **3.1 Obtener Tipos de Cliente**
```http
GET http://localhost:5000/api/Clientes/tipos
Authorization: Bearer {token}
```

### **3.2 Obtener Tipos de Vehículo**
```http
GET http://localhost:5000/api/Vehiculos/tipos
Authorization: Bearer {token}
```

### **3.3 Obtener Marcas de Vehículo**
```http
GET http://localhost:5000/api/Vehiculos/marcas
Authorization: Bearer {token}
```

### **3.4 Obtener Modelos de Vehículo**
```http
GET http://localhost:5000/api/Vehiculos/modelos
Authorization: Bearer {token}
```

### **3.5 Obtener Tipos de Servicio**
```http
GET http://localhost:5000/api/OrdenesServicio/tipos-servicio
Authorization: Bearer {token}
```

### **3.6 Obtener Estados de Servicio**
```http
GET http://localhost:5000/api/OrdenesServicio/estados
Authorization: Bearer {token}
```

### **3.7 Obtener Categorías de Repuestos**
```http
GET http://localhost:5000/api/Repuestos/categorias
Authorization: Bearer {token}
```

### **3.8 Obtener Fabricantes de Repuestos**
```http
GET http://localhost:5000/api/Repuestos/fabricantes
Authorization: Bearer {token}
```

---

## 👥 **4. Gestión de Clientes**

### **4.1 Crear Cliente**
```http
POST http://localhost:5000/api/Clientes
Authorization: Bearer {token}
Content-Type: application/json

{
  "nombre": "Juan Carlos",
  "apellido": "Pérez García",
  "telefono": "3001234567",
  "email": "juan.perez@email.com",
  "tipoClienteId": 1
}
```

### **4.2 Obtener Todos los Clientes**
```http
GET http://localhost:5000/api/Clientes?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

### **4.3 Obtener Cliente por ID**
```http
GET http://localhost:5000/api/Clientes/{clienteId}
Authorization: Bearer {token}
```

### **4.4 Actualizar Cliente**
```http
PUT http://localhost:5000/api/Clientes/{clienteId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": {clienteId},
  "nombre": "Juan Carlos",
  "apellido": "Pérez García",
  "telefono": "3001234567",
  "email": "juan.perez.actualizado@email.com",
  "tipoClienteId": 1
}
```

---

## 🚗 **5. Gestión de Vehículos**

### **5.1 Crear Vehículo**
```http
POST http://localhost:5000/api/Vehiculos
Authorization: Bearer {token}
Content-Type: application/json

{
  "vin": "1HGBH41JXMN109186",
  "ano": 2020,
  "kilometraje": 45000,
  "clienteId": {clienteId},
  "tipoVehiculoId": 1,
  "marcaId": 1,
  "modeloId": 1
}
```

### **5.2 Obtener Vehículos por Cliente**
```http
GET http://localhost:5000/api/Vehiculos?clienteId={clienteId}
Authorization: Bearer {token}
```

### **5.3 Obtener Vehículo por VIN**
```http
GET http://localhost:5000/api/Vehiculos/vin/1HGBH41JXMN109186
Authorization: Bearer {token}
```

### **5.4 Actualizar Kilometraje**
```http
PUT http://localhost:5000/api/Vehiculos/{vehiculoId}/kilometraje
Authorization: Bearer {token}
Content-Type: application/json

{
  "kilometraje": 50000
}
```

---

## 📦 **6. Gestión de Repuestos**

### **6.1 Crear Repuesto**
```http
POST http://localhost:5000/api/Repuestos
Authorization: Bearer {token}
Content-Type: application/json

{
  "codigoRepuesto": "FIL-002",
  "nombreRepu": "Filtro de Aceite Premium",
  "descripcion": "Filtro de aceite de alta calidad",
  "stock": 25,
  "precioUnitario": 18.99,
  "stockMinimo": 5,
  "categoriaId": 6,
  "fabricanteId": 4
}
```

### **6.2 Obtener Repuestos**
```http
GET http://localhost:5000/api/Repuestos?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

### **6.3 Buscar Repuestos por Código**
```http
GET http://localhost:5000/api/Repuestos/codigo/FIL-002
Authorization: Bearer {token}
```

### **6.4 Actualizar Stock**
```http
PUT http://localhost:5000/api/Repuestos/{repuestoId}/stock
Authorization: Bearer {token}
Content-Type: application/json

{
  "stock": 30
}
```

### **6.5 Obtener Repuestos con Stock Bajo**
```http
GET http://localhost:5000/api/Repuestos/stock-bajo
Authorization: Bearer {token}
```

---

## 🔧 **7. Gestión de Órdenes de Servicio**

### **7.1 Crear Orden de Servicio**
```http
POST http://localhost:5000/api/OrdenesServicio
Authorization: Bearer {token}
Content-Type: application/json

{
  "vehiculoId": {vehiculoId},
  "mecanicoId": 1,
  "tipoServicioId": 1,
  "fechaIngreso": "2024-12-20T10:00:00Z",
  "descripcionTrabajo": "Mantenimiento preventivo completo",
  "repuestosRequeridos": [
    {
      "repuestoId": 1,
      "cantidad": 2
    },
    {
      "repuestoId": 2,
      "cantidad": 1
    }
  ]
}
```

### **7.2 Obtener Órdenes de Servicio**
```http
GET http://localhost:5000/api/OrdenesServicio?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

### **7.3 Obtener Orden por ID**
```http
GET http://localhost:5000/api/OrdenesServicio/{ordenId}
Authorization: Bearer {token}
```

### **7.4 Actualizar Estado de Orden**
```http
PUT http://localhost:5000/api/OrdenesServicio/{ordenId}/estado
Authorization: Bearer {token}
Content-Type: application/json

{
  "estadoId": 2
}
```

### **7.5 Registrar Trabajo Realizado**
```http
PUT http://localhost:5000/api/OrdenesServicio/{ordenId}/trabajo-realizado
Authorization: Bearer {token}
Content-Type: application/json

{
  "descripcionTrabajoRealizado": "Cambio de aceite y filtro completado",
  "manoDeObra": 50.00,
  "repuestosUtilizados": [
    {
      "repuestoId": 1,
      "cantidad": 2,
      "precioUnitario": 15.99,
      "descripcion": "Filtro de aceite estándar"
    }
  ]
}
```

### **7.6 Asignar Repuestos Adicionales**
```http
POST http://localhost:5000/api/OrdenesServicio/{ordenId}/asignar-repuestos
Authorization: Bearer {token}
Content-Type: application/json

{
  "repuestos": [
    {
      "repuestoId": 3,
      "cantidad": 4
    }
  ]
}
```

### **7.7 Cerrar Orden de Servicio**
```http
POST http://localhost:5000/api/OrdenesServicio/{ordenId}/cerrar
Authorization: Bearer {token}
Content-Type: application/json

{
  "tipoPagoId": 1,
  "observacionesFactura": "Servicio completado satisfactoriamente"
}
```

---

## 💰 **8. Gestión de Facturas**

### **8.1 Generar Factura Manual**
```http
POST http://localhost:5000/api/Facturas/generar
Authorization: Bearer {token}
Content-Type: application/json

{
  "ordenServicioId": {ordenId},
  "tipoPagoId": 1,
  "observaciones": "Factura generada manualmente"
}
```

### **8.2 Obtener Facturas**
```http
GET http://localhost:5000/api/Facturas?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

### **8.3 Obtener Factura por ID**
```http
GET http://localhost:5000/api/Facturas/{facturaId}
Authorization: Bearer {token}
```

### **8.4 Obtener Facturas por Cliente**
```http
GET http://localhost:5000/api/Facturas/cliente/{clienteId}
Authorization: Bearer {token}
```

---

## 👤 **9. Gestión de Usuarios (Solo Admin)**

### **9.1 Crear Usuario**
```http
POST http://localhost:5000/api/Usuario/register
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "mecanico1",
  "email": "mecanico1@autotaller.com",
  "password": "mecanico123",
  "estadoId": 1,
  "rolId": 2
}
```

### **9.2 Obtener Usuarios**
```http
GET http://localhost:5000/api/Usuario
Authorization: Bearer {token}
```

### **9.3 Actualizar Usuario**
```http
PUT http://localhost:5000/api/Usuario/{usuarioId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": {usuarioId},
  "username": "mecanico1",
  "email": "mecanico1.actualizado@autotaller.com",
  "estadoId": 1,
  "rolId": 2
}
```

---

## 📈 **10. Reportes y Estadísticas**

### **10.1 Estadísticas del Sistema**
```http
GET http://localhost:5000/api/Init/stats
Authorization: Bearer {token}
```

### **10.2 Órdenes por Estado**
```http
GET http://localhost:5000/api/OrdenesServicio/por-estado
Authorization: Bearer {token}
```

### **10.3 Ingresos por Período**
```http
GET http://localhost:5000/api/Facturas/ingresos?fechaInicio=2024-12-01&fechaFin=2024-12-31
Authorization: Bearer {token}
```

---

## 🧪 **11. Pruebas de Rate Limiting**

### **11.1 Probar Límite de Órdenes de Servicio**
```bash
# Ejecutar múltiples veces para probar el límite de 60 req/min
for i in {1..65}; do
  curl -H "Authorization: Bearer {token}" \
       http://localhost:5000/api/OrdenesServicio
  echo "Request $i"
done
```

### **11.2 Probar Límite de Repuestos**
```bash
# Ejecutar múltiples veces para probar el límite de 30 req/min
for i in {1..35}; do
  curl -H "Authorization: Bearer {token}" \
       http://localhost:5000/api/Repuestos
  echo "Request $i"
done
```

---

## 🔍 **12. Verificación de Swagger**

### **12.1 Acceder a Swagger UI**
```
http://localhost:5000/swagger
```

### **12.2 Autenticar en Swagger**
1. Haz clic en "Authorize"
2. Introduce: `Bearer {tu_token}`
3. Haz clic en "Authorize"

---

## ⚠️ **Errores Comunes y Soluciones**

### **Error 401 Unauthorized**
- **Causa:** Token expirado o inválido
- **Solución:** Hacer login nuevamente y obtener un nuevo token

### **Error 403 Forbidden**
- **Causa:** Usuario sin permisos para la operación
- **Solución:** Verificar que el usuario tenga el rol correcto

### **Error 429 Too Many Requests**
- **Causa:** Rate limit excedido
- **Solución:** Esperar el tiempo especificado en el header `Retry-After`

### **Error 400 Bad Request**
- **Causa:** Datos de entrada inválidos
- **Solución:** Verificar el formato JSON y los campos requeridos

### **Error 404 Not Found**
- **Causa:** Recurso no encontrado
- **Solución:** Verificar que el ID existe en la base de datos

---

## 📝 **Notas Importantes**

1. **Orden de Pruebas:** Siempre sigue el orden indicado para evitar errores de dependencias
2. **Tokens:** Los tokens JWT expiran en 60 minutos por defecto
3. **Rate Limiting:** Cada endpoint tiene límites específicos de requests por minuto
4. **Validaciones:** Todos los endpoints tienen validaciones de entrada
5. **Auditoría:** Todas las operaciones se registran en la tabla de auditoría

---

## 🎯 **Flujo de Prueba Completo Recomendado**

1. **Autenticación** → Login como admin
2. **Catálogos** → Verificar datos maestros
3. **Cliente** → Crear cliente
4. **Vehículo** → Crear vehículo para el cliente
5. **Repuestos** → Crear repuestos necesarios
6. **Orden** → Crear orden de servicio
7. **Trabajo** → Registrar trabajo realizado
8. **Factura** → Cerrar orden (genera factura automáticamente)
9. **Verificación** → Consultar factura generada

¡Con esta guía puedes probar completamente todos los endpoints del sistema AutoTallerManager! 🚀
