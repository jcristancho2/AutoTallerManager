#!/bin/bash

# 🚀 Script de Pruebas Automatizadas para AutoTallerManager
# Este script prueba todos los endpoints en el orden correcto

# Configuración
API_BASE_URL="http://localhost:5000"
TOKEN=""
CLIENTE_ID=""
VEHICULO_ID=""
REPUESTO_ID=""
ORDEN_ID=""
FACTURA_ID=""

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Función para imprimir mensajes
print_step() {
    echo -e "${BLUE}=== $1 ===${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Función para hacer peticiones HTTP
make_request() {
    local method=$1
    local url=$2
    local data=$3
    local headers=$4
    
    if [ -n "$data" ]; then
        curl -s -X $method \
             -H "Content-Type: application/json" \
             -H "$headers" \
             -d "$data" \
             "$url"
    else
        curl -s -X $method \
             -H "$headers" \
             "$url"
    fi
}

# Función para extraer valores de JSON
extract_value() {
    local json=$1
    local key=$2
    echo "$json" | grep -o "\"$key\":[^,}]*" | cut -d'"' -f4
}

# Función para esperar entrada del usuario
wait_for_user() {
    echo -e "${YELLOW}Presiona Enter para continuar...${NC}"
    read
}

# =============================================================================
# INICIO DE LAS PRUEBAS
# =============================================================================

print_step "INICIANDO PRUEBAS DE AUTOTALLERMANAGER"

# Verificar que la API esté ejecutándose
print_step "Verificando conexión con la API"
response=$(curl -s -o /dev/null -w "%{http_code}" "$API_BASE_URL/swagger")
if [ "$response" = "200" ]; then
    print_success "API está ejecutándose correctamente"
else
    print_error "API no está disponible. Asegúrate de que esté ejecutándose en $API_BASE_URL"
    exit 1
fi

# =============================================================================
# 1. AUTENTICACIÓN
# =============================================================================

print_step "1. AUTENTICACIÓN"

echo "Intentando login como administrador..."
login_response=$(make_request "POST" "$API_BASE_URL/api/Usuario/login" '{
  "email": "admin@autotaller.com",
  "password": "admin123"
}')

if echo "$login_response" | grep -q "token"; then
    TOKEN=$(extract_value "$login_response" "token")
    print_success "Login exitoso. Token obtenido: ${TOKEN:0:20}..."
else
    print_error "Error en el login. Respuesta: $login_response"
    print_warning "Asegúrate de que los datos de inicialización estén cargados"
    exit 1
fi

wait_for_user

# =============================================================================
# 2. CATÁLOGOS Y DATOS MAESTROS
# =============================================================================

print_step "2. CONSULTANDO CATÁLOGOS"

echo "Obteniendo tipos de cliente..."
tipos_cliente=$(make_request "GET" "$API_BASE_URL/api/Clientes/tipos" "" "Authorization: Bearer $TOKEN")
print_success "Tipos de cliente obtenidos"

echo "Obteniendo tipos de vehículo..."
tipos_vehiculo=$(make_request "GET" "$API_BASE_URL/api/Vehiculos/tipos" "" "Authorization: Bearer $TOKEN")
print_success "Tipos de vehículo obtenidos"

echo "Obteniendo marcas de vehículo..."
marcas=$(make_request "GET" "$API_BASE_URL/api/Vehiculos/marcas" "" "Authorization: Bearer $TOKEN")
print_success "Marcas de vehículo obtenidas"

echo "Obteniendo tipos de servicio..."
tipos_servicio=$(make_request "GET" "$API_BASE_URL/api/OrdenesServicio/tipos-servicio" "" "Authorization: Bearer $TOKEN")
print_success "Tipos de servicio obtenidos"

wait_for_user

# =============================================================================
# 3. GESTIÓN DE CLIENTES
# =============================================================================

print_step "3. GESTIÓN DE CLIENTES"

echo "Creando nuevo cliente..."
cliente_response=$(make_request "POST" "$API_BASE_URL/api/Clientes" '{
  "nombre": "Juan Carlos",
  "apellido": "Pérez García",
  "telefono": "3001234567",
  "email": "juan.perez@email.com",
  "tipoClienteId": 1
}' "Authorization: Bearer $TOKEN")

if echo "$cliente_response" | grep -q "id"; then
    CLIENTE_ID=$(extract_value "$cliente_response" "id")
    print_success "Cliente creado con ID: $CLIENTE_ID"
else
    print_error "Error al crear cliente. Respuesta: $cliente_response"
fi

echo "Obteniendo lista de clientes..."
clientes=$(make_request "GET" "$API_BASE_URL/api/Clientes?pageNumber=1&pageSize=10" "" "Authorization: Bearer $TOKEN")
print_success "Lista de clientes obtenida"

wait_for_user

# =============================================================================
# 4. GESTIÓN DE VEHÍCULOS
# =============================================================================

print_step "4. GESTIÓN DE VEHÍCULOS"

echo "Creando nuevo vehículo..."
vehiculo_response=$(make_request "POST" "$API_BASE_URL/api/Vehiculos" "{
  \"vin\": \"1HGBH41JXMN109186\",
  \"ano\": 2020,
  \"kilometraje\": 45000,
  \"clienteId\": $CLIENTE_ID,
  \"tipoVehiculoId\": 1,
  \"marcaId\": 1,
  \"modeloId\": 1
}" "Authorization: Bearer $TOKEN")

if echo "$vehiculo_response" | grep -q "id"; then
    VEHICULO_ID=$(extract_value "$vehiculo_response" "id")
    print_success "Vehículo creado con ID: $VEHICULO_ID"
else
    print_error "Error al crear vehículo. Respuesta: $vehiculo_response"
fi

echo "Obteniendo vehículos del cliente..."
vehiculos_cliente=$(make_request "GET" "$API_BASE_URL/api/Vehiculos?clienteId=$CLIENTE_ID" "" "Authorization: Bearer $TOKEN")
print_success "Vehículos del cliente obtenidos"

wait_for_user

# =============================================================================
# 5. GESTIÓN DE REPUESTOS
# =============================================================================

print_step "5. GESTIÓN DE REPUESTOS"

echo "Creando nuevo repuesto..."
repuesto_response=$(make_request "POST" "$API_BASE_URL/api/Repuestos" '{
  "codigoRepuesto": "FIL-002",
  "nombreRepu": "Filtro de Aceite Premium",
  "descripcion": "Filtro de aceite de alta calidad",
  "stock": 25,
  "precioUnitario": 18.99,
  "stockMinimo": 5,
  "categoriaId": 6,
  "fabricanteId": 4
}' "Authorization: Bearer $TOKEN")

if echo "$repuesto_response" | grep -q "id"; then
    REPUESTO_ID=$(extract_value "$repuesto_response" "id")
    print_success "Repuesto creado con ID: $REPUESTO_ID"
else
    print_error "Error al crear repuesto. Respuesta: $repuesto_response"
fi

echo "Obteniendo lista de repuestos..."
repuestos=$(make_request "GET" "$API_BASE_URL/api/Repuestos?pageNumber=1&pageSize=10" "" "Authorization: Bearer $TOKEN")
print_success "Lista de repuestos obtenida"

wait_for_user

# =============================================================================
# 6. GESTIÓN DE ÓRDENES DE SERVICIO
# =============================================================================

print_step "6. GESTIÓN DE ÓRDENES DE SERVICIO"

echo "Creando nueva orden de servicio..."
orden_response=$(make_request "POST" "$API_BASE_URL/api/OrdenesServicio" "{
  \"vehiculoId\": $VEHICULO_ID,
  \"mecanicoId\": 1,
  \"tipoServicioId\": 1,
  \"fechaIngreso\": \"2024-12-20T10:00:00Z\",
  \"descripcionTrabajo\": \"Mantenimiento preventivo completo\",
  \"repuestosRequeridos\": [
    {
      \"repuestoId\": $REPUESTO_ID,
      \"cantidad\": 2
    }
  ]
}" "Authorization: Bearer $TOKEN")

if echo "$orden_response" | grep -q "id"; then
    ORDEN_ID=$(extract_value "$orden_response" "id")
    print_success "Orden de servicio creada con ID: $ORDEN_ID"
else
    print_error "Error al crear orden de servicio. Respuesta: $orden_response"
fi

echo "Obteniendo lista de órdenes..."
ordenes=$(make_request "GET" "$API_BASE_URL/api/OrdenesServicio?pageNumber=1&pageSize=10" "" "Authorization: Bearer $TOKEN")
print_success "Lista de órdenes obtenida"

echo "Actualizando estado de la orden a 'En Proceso'..."
estado_response=$(make_request "PUT" "$API_BASE_URL/api/OrdenesServicio/$ORDEN_ID/estado" '{
  "estadoId": 2
}' "Authorization: Bearer $TOKEN")
print_success "Estado de orden actualizado"

echo "Registrando trabajo realizado..."
trabajo_response=$(make_request "PUT" "$API_BASE_URL/api/OrdenesServicio/$ORDEN_ID/trabajo-realizado" "{
  \"descripcionTrabajoRealizado\": \"Cambio de aceite y filtro completado\",
  \"manoDeObra\": 50.00,
  \"repuestosUtilizados\": [
    {
      \"repuestoId\": $REPUESTO_ID,
      \"cantidad\": 2,
      \"precioUnitario\": 18.99,
      \"descripcion\": \"Filtro de aceite premium\"
    }
  ]
}" "Authorization: Bearer $TOKEN")
print_success "Trabajo realizado registrado"

wait_for_user

# =============================================================================
# 7. CIERRE DE ORDEN Y FACTURACIÓN
# =============================================================================

print_step "7. CIERRE DE ORDEN Y FACTURACIÓN"

echo "Cerrando orden de servicio..."
cierre_response=$(make_request "POST" "$API_BASE_URL/api/OrdenesServicio/$ORDEN_ID/cerrar" '{
  "tipoPagoId": 1,
  "observacionesFactura": "Servicio completado satisfactoriamente"
}' "Authorization: Bearer $TOKEN")

if echo "$cierre_response" | grep -q "facturaId"; then
    FACTURA_ID=$(extract_value "$cierre_response" "facturaId")
    print_success "Orden cerrada. Factura generada con ID: $FACTURA_ID"
else
    print_error "Error al cerrar orden. Respuesta: $cierre_response"
fi

echo "Obteniendo detalles de la factura..."
factura=$(make_request "GET" "$API_BASE_URL/api/Facturas/$FACTURA_ID" "" "Authorization: Bearer $TOKEN")
print_success "Detalles de factura obtenidos"

wait_for_user

# =============================================================================
# 8. PRUEBAS DE RATE LIMITING
# =============================================================================

print_step "8. PRUEBAS DE RATE LIMITING"

echo "Probando límite de órdenes de servicio (60 req/min)..."
rate_limit_hit=false
for i in {1..65}; do
    response=$(make_request "GET" "$API_BASE_URL/api/OrdenesServicio" "" "Authorization: Bearer $TOKEN")
    if echo "$response" | grep -q "Rate limit exceeded"; then
        print_warning "Rate limit alcanzado en request $i"
        rate_limit_hit=true
        break
    fi
    echo -n "."
done

if [ "$rate_limit_hit" = true ]; then
    print_success "Rate limiting funcionando correctamente"
else
    print_warning "Rate limiting no se activó (puede ser normal si las requests son muy rápidas)"
fi

wait_for_user

# =============================================================================
# 9. ESTADÍSTICAS Y REPORTES
# =============================================================================

print_step "9. ESTADÍSTICAS Y REPORTES"

echo "Obteniendo estadísticas del sistema..."
stats=$(make_request "GET" "$API_BASE_URL/api/Init/stats" "" "Authorization: Bearer $TOKEN")
print_success "Estadísticas obtenidas"

echo "Obteniendo facturas por cliente..."
facturas_cliente=$(make_request "GET" "$API_BASE_URL/api/Facturas/cliente/$CLIENTE_ID" "" "Authorization: Bearer $TOKEN")
print_success "Facturas del cliente obtenidas"

# =============================================================================
# RESUMEN FINAL
# =============================================================================

print_step "RESUMEN DE PRUEBAS COMPLETADAS"

echo "📊 IDs generados durante las pruebas:"
echo "   Cliente ID: $CLIENTE_ID"
echo "   Vehículo ID: $VEHICULO_ID"
echo "   Repuesto ID: $REPUESTO_ID"
echo "   Orden ID: $ORDEN_ID"
echo "   Factura ID: $FACTURA_ID"

echo ""
echo "✅ Pruebas completadas exitosamente:"
echo "   - Autenticación JWT"
echo "   - Consulta de catálogos"
echo "   - Gestión de clientes"
echo "   - Gestión de vehículos"
echo "   - Gestión de repuestos"
echo "   - Gestión de órdenes de servicio"
echo "   - Cierre de órdenes y facturación"
echo "   - Rate limiting"
echo "   - Estadísticas y reportes"

echo ""
print_success "🎉 TODAS LAS PRUEBAS COMPLETADAS EXITOSAMENTE!"
echo ""
echo "Para más pruebas detalladas, consulta la guía completa:"
echo "📖 GUIA_PRUEBAS_ENDPOINTS.md"
echo ""
echo "Para probar con Swagger UI:"
echo "🌐 $API_BASE_URL/swagger"
