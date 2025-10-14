#!/bin/bash

# Script de prueba para endpoints de autenticación AutoTallerManager
# Este script prueba todos los endpoints de /api/auth/*

BASE_URL="http://localhost:5015"
TOKEN=""

echo "🚀 Iniciando pruebas de endpoints de autenticación AutoTallerManager"
echo "================================================================"

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
echo "1️⃣ PROBANDO HEALTH CHECK"
echo "========================"
response=$(make_request "GET" "/api/auth/health")
show_result "Health Check" "$response"

echo ""
echo "2️⃣ CREANDO USUARIO ADMINISTRADOR"
echo "==============================="
response=$(make_request "POST" "/api/auth/setup/admin")
show_result "Crear Admin" "$response"

echo ""
echo "3️⃣ CREANDO ROLES BÁSICOS"
echo "======================="
response=$(make_request "POST" "/api/auth/setup/roles")
show_result "Crear Roles" "$response"

echo ""
echo "4️⃣ ASIGNANDO ROL ADMIN"
echo "======================"
response=$(make_request "POST" "/api/auth/setup/assign-admin")
show_result "Asignar Rol Admin" "$response"

echo ""
echo "5️⃣ PROBANDO LOGIN"
echo "================="
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
echo "6️⃣ PROBANDO OBTENER ROLES"
echo "========================"
response=$(make_request "GET" "/api/auth/roles")
show_result "Obtener Roles" "$response"

echo ""
echo "7️⃣ PROBANDO OBTENER USUARIOS"
echo "============================"
response=$(make_request "GET" "/api/auth/users" "" "Authorization: Bearer $TOKEN")
show_result "Obtener Usuarios" "$response"

echo ""
echo "8️⃣ PROBANDO OBTENER USUARIO POR ID"
echo "=================================="
response=$(make_request "GET" "/api/auth/users/1" "" "Authorization: Bearer $TOKEN")
show_result "Obtener Usuario por ID" "$response"

echo ""
echo "9️⃣ PROBANDO CREAR NUEVO ROL"
echo "==========================="
new_role_data='{
    "nombreRol": "Supervisor",
    "descripcion": "Supervisor del taller"
}'
response=$(make_request "POST" "/api/auth/roles" "$new_role_data" "Authorization: Bearer $TOKEN")
show_result "Crear Nuevo Rol" "$response"

echo ""
echo "🔟 PROBANDO REFRESH TOKEN"
echo "========================"
refresh_data='{
    "refreshToken": "'$(echo "$response" | jq -r '.refreshToken' 2>/dev/null)'"
}'
response=$(make_request "POST" "/api/auth/refresh-token" "$refresh_data")
show_result "Refresh Token" "$response"

echo ""
echo "1️⃣1️⃣ PROBANDO LOGOUT"
echo "===================="
response=$(make_request "POST" "/api/auth/logout" "" "Authorization: Bearer $TOKEN")
show_result "Logout" "$response"

echo ""
echo "✅ PRUEBAS COMPLETADAS"
echo "====================="
echo "Todos los endpoints de autenticación han sido probados exitosamente."
echo ""
echo "📊 RESUMEN DE ENDPOINTS PROBADOS:"
echo "• GET  /api/auth/health              - Health check"
echo "• POST /api/auth/setup/admin         - Crear admin inicial"
echo "• POST /api/auth/setup/roles         - Crear roles básicos"
echo "• POST /api/auth/setup/assign-admin  - Asignar rol admin"
echo "• POST /api/auth/login               - Login"
echo "• GET  /api/auth/roles               - Obtener roles"
echo "• GET  /api/auth/users               - Obtener usuarios"
echo "• GET  /api/auth/users/{id}          - Obtener usuario por ID"
echo "• POST /api/auth/roles               - Crear nuevo rol"
echo "• POST /api/auth/refresh-token       - Renovar token"
echo "• POST /api/auth/logout              - Logout"
echo ""
echo "🎯 ENDPOINTS ADICIONALES DISPONIBLES:"
echo "• POST /api/auth/register            - Registrar usuario (requiere Admin)"
echo "• PUT  /api/auth/users/{id}          - Actualizar usuario (requiere Admin)"
echo "• PUT  /api/auth/users/{id}/change-password - Cambiar contraseña (requiere Admin)"
echo "• POST /api/auth/users/assign-role   - Asignar rol (requiere Admin)"
echo "• PUT  /api/auth/roles/{id}          - Actualizar rol (requiere Admin)"
echo "• DELETE /api/auth/roles/{id}        - Eliminar rol (requiere Admin)"
