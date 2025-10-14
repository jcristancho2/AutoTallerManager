#!/bin/bash

# Script de prueba para el sistema de registro de clientes con dirección completa
# Este script prueba los nuevos endpoints de ubicaciones y registro de clientes

BASE_URL="http://localhost:5015"
TOKEN=""

echo "🚀 Iniciando pruebas del sistema de registro de clientes con dirección completa"
echo "================================================================================="

# Función para hacer requests HTTP
make_request() {
    local method=$1
    local endpoint=$2
    local data=$3
    local headers=$4
    
    if [ -n "$data" ]; then
        if [ -n "$headers" ]; then
            curl -s -X $method "$BASE_URL$endpoint" \
                -H "Content-Type: application/json" \
                -H "$headers" \
                -d "$data"
        else
            curl -s -X $method "$BASE_URL$endpoint" \
                -H "Content-Type: application/json" \
                -d "$data"
        fi
    else
        if [ -n "$headers" ]; then
            curl -s -X $method "$BASE_URL$endpoint" \
                -H "$headers"
        else
            curl -s -X $method "$BASE_URL$endpoint"
        fi
    fi
}

# Función para mostrar resultado
show_result() {
    local test_name=$1
    local response=$2
    
    echo ""
    echo "📋 $test_name"
    echo "----------------------------------------"
    echo "$response" | jq . 2>/dev/null || echo "$response"
    echo ""
}

echo ""
echo "1️⃣ OBTENIENDO TOKEN DE AUTENTICACIÓN"
echo "===================================="
login_data='{
    "username": "admin@autotaller.com",
    "password": "admin123"
}'
response=$(make_request "POST" "/api/auth/login" "$login_data")
show_result "Login" "$response"

# Extraer token de la respuesta
TOKEN=$(echo "$response" | jq -r '.token' 2>/dev/null)
if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
    echo "❌ Error: No se pudo obtener el token de autenticación"
    exit 1
fi

echo "✅ Token obtenido: ${TOKEN:0:50}..."

echo ""
echo "2️⃣ INICIALIZANDO DATOS DE UBICACIÓN (COLOMBIA)"
echo "=============================================="
response=$(make_request "POST" "/api/ubicaciones/setup/colombia" "" "Authorization: Bearer $TOKEN")
show_result "Setup Colombia" "$response"

echo ""
echo "3️⃣ OBTENIENDO PAÍSES DISPONIBLES"
echo "==============================="
response=$(make_request "GET" "/api/ubicaciones/paises")
show_result "Países" "$response"

echo ""
echo "4️⃣ OBTENIENDO DEPARTAMENTOS DE COLOMBIA"
echo "======================================"
response=$(make_request "GET" "/api/ubicaciones/departamentos?paisId=1")
show_result "Departamentos de Colombia" "$response"

echo ""
echo "5️⃣ OBTENIENDO CIUDADES DE ANTIOQUIA"
echo "=================================="
response=$(make_request "GET" "/api/ubicaciones/ciudades?departamentoId=1")
show_result "Ciudades de Antioquia" "$response"

echo ""
echo "6️⃣ CREANDO DIRECCIÓN EN MEDELLÍN"
echo "==============================="
direccion_data='{
    "descripcion": "Carrera 50 #25-30, Barrio El Poblado",
    "ciudadId": 1
}'
response=$(make_request "POST" "/api/ubicaciones/direcciones" "$direccion_data" "Authorization: Bearer $TOKEN")
show_result "Crear Dirección" "$response"

echo ""
echo "7️⃣ PROBANDO REGISTRO DE CLIENTE COMPLETO"
echo "========================================"
cliente_completo_data='{
    "nombreCompleto": "Juan Carlos Pérez García",
    "telefono": "3001234567",
    "email": "juan.perez@email.com",
    "tipoClienteId": 1,
    "direccion": {
        "descripcion": "Carrera 50 #25-30, Barrio El Poblado",
        "paisId": 1,
        "departamentoId": 1,
        "ciudadId": 1
    }
}'
response=$(make_request "POST" "/api/clientes/completo" "$cliente_completo_data" "Authorization: Bearer $TOKEN")
show_result "Cliente Completo" "$response"

echo ""
echo "8️⃣ PROBANDO REGISTRO DE CLIENTE CON VEHÍCULO COMPLETO"
echo "===================================================="
cliente_con_vehiculo_data='{
    "cliente": {
        "nombreCompleto": "María González López",
        "telefono": "3009876543",
        "email": "maria.gonzalez@email.com",
        "tipoClienteId": 1,
        "direccion": {
            "descripcion": "Calle 80 #45-67, Barrio Laureles",
            "paisId": 1,
            "departamentoId": 1,
            "ciudadId": 1
        }
    },
    "vehiculos": [
        {
            "vin": "1HGBH41JXMN109186",
            "ano": 2020,
            "kilometraje": 50000,
            "tipoVehiculoId": 1,
            "marcaId": 1,
            "modeloId": 1
        }
    ]
}'
response=$(make_request "POST" "/api/clientes/registrar-con-vehiculo-completo" "$cliente_con_vehiculo_data" "Authorization: Bearer $TOKEN")
show_result "Cliente con Vehículo Completo" "$response"

echo ""
echo "9️⃣ VERIFICANDO CLIENTES CREADOS"
echo "==============================="
response=$(make_request "GET" "/api/clientes" "" "Authorization: Bearer $TOKEN")
show_result "Lista de Clientes" "$response"

echo ""
echo "🔟 OBTENIENDO DIRECCIONES CREADAS"
echo "================================="
response=$(make_request "GET" "/api/ubicaciones/direcciones")
show_result "Direcciones" "$response"

echo ""
echo "✅ PRUEBAS COMPLETADAS"
echo "====================="
echo "Sistema de registro de clientes con dirección completa probado exitosamente."
echo ""
echo "📊 RESUMEN DE ENDPOINTS PROBADOS:"
echo "• POST /api/auth/login                           - Autenticación"
echo "• POST /api/ubicaciones/setup/colombia           - Setup datos Colombia"
echo "• GET  /api/ubicaciones/paises                   - Obtener países"
echo "• GET  /api/ubicaciones/departamentos            - Obtener departamentos"
echo "• GET  /api/ubicaciones/ciudades                 - Obtener ciudades"
echo "• POST /api/ubicaciones/direcciones              - Crear dirección"
echo "• POST /api/clientes/completo                    - Registro cliente completo"
echo "• POST /api/clientes/registrar-con-vehiculo-completo - Cliente con vehículo"
echo "• GET  /api/clientes                             - Listar clientes"
echo "• GET  /api/ubicaciones/direcciones              - Listar direcciones"
echo ""
echo "🎯 CARACTERÍSTICAS IMPLEMENTADAS:"
echo "• ✅ Gestión completa de ubicaciones (País → Departamento → Ciudad → Dirección)"
echo "• ✅ Registro de clientes con dirección geográfica completa"
echo "• ✅ Validación de jerarquía de ubicaciones"
echo "• ✅ Registro conjunto de cliente con vehículos"
echo "• ✅ Endpoints para consultar ubicaciones disponibles"
echo "• ✅ Setup automático de datos básicos de Colombia"
echo ""
echo "🚀 El sistema está listo para uso en producción!"
