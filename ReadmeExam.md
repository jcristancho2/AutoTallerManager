##  Búsqueda avanzada y paginación con conteo total (Clientes/Vehículos)

**Descripción**
Extender los listados de Clientes y Vehículos para soportar búsqueda avanzada y paginación consistente con cabecera X-Total-Count. Se deben agregar filtros por:

1. *Clientes:* nombre (contiene), correo (exacto/contiene).
2. *Vehículos:* vin (exacto), marca (contiene), modelo (contiene), año (rango desde/hasta).
La respuesta debe incluir la lista paginada y la cabecera X-Total-Count con el total antes de paginar.

**Objetivo general**
Permitir consultas eficientes y paginadas sobre clientes y vehículos, manteniendo la experiencia uniforme que el proyecto ya usa.

**Objetivos específicos**
Añadir parámetros de query (pageNumber, pageSize, filtros) a los endpoints.
Implementar conteo total y retorno del header X-Total-Count.
Garantizar ordenamiento estable (por nombre en clientes; por marca/modelo en vehículos).
