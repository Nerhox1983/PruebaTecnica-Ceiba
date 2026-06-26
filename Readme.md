# CourierMax - Sistema de Gestión de Envíos

CourierMax es una solución robusta para la gestión logística de envíos, construida bajo estándares de desarrollo modernos para garantizar escalabilidad y mantenibilidad.

## 🏗️ Arquitectura y Diseño

El proyecto implementa una **Arquitectura Limpia (Clean Architecture)** combinada con el patrón **CQRS (Command Query Responsibility Segregation)**.

### Justificación:
- **Separación de Responsabilidades:** El dominio es independiente de frameworks externos.
- **Escalabilidad:** CQRS permite optimizar flujos de lectura y escritura.
- **Mantenibilidad:** Facilidad para realizar pruebas unitarias y cambios en la infraestructura.

---

## 🛠️ Tecnologías Utilizadas

- **.NET 8.0 SDK**
- **ASP.NET Core Minimal APIs / Web API**
- **C# 12**
- **SQL Server** (Base de datos)
- **OpenAPI / Swagger** para documentación de endpoints.

---

## 🗄️ Base de Datos

El sistema cuenta con un esquema relacional diseñado para gestionar ciudades, distancias, vehículos y el ciclo de vida de los envíos.

### Estructura de Tablas:
- `Cities`: Catálogo de ciudades.
- `CityDistances`: Configuración de distancias y tarifas entre ciudades.
- `Vehicles`: Registro de la flota y conductores.
- `Shipments`: Gestión central de los envíos, incluyendo cálculos de costos y estados.
- `ShipmentStatusLogs`: Auditoría del historial de cambios de estado por envío.

*Se incluyen scripts SQL para la creación de la estructura y la carga de datos maestros iniciales (Ciudades, Distancias y Vehículos).*

## 🚀 Instrucciones para Ejecución Local

### Requisitos Previos:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (o instancia compatible)
- IDE compatible (VS 2022, VS Code, JetBrains Rider)

### Pasos:
1. **Clonar el repositorio:**
   ```bash
   git clone <url-del-repositorio>
   cd CourierMax
   ```
2. **Restaurar dependencias:**
   ```bash
   dotnet restore
   ```
3. **Ejecutar la API:**
   ```bash
   dotnet run --project CourierMax.Api
   ```
4. **Acceder a Swagger:**
   Abra en su navegador: `https://localhost:7117/swagger` (o el puerto configurado).

---

## 📞 Ejemplos de Llamadas a la API (Endpoints Principales)

### 1. Crear un nuevo envío
**POST** `/api/shipments`

```bash
curl -X POST https://localhost:7117/api/shipments \
-H "Content-Type: application/json" \
-d '{
  "senderName": "Juan Perez",
  "senderPhone": "555-1234",
  "senderAddress": "Calle 1",
  "receiverName": "Ana Maria",
  "receiverPhone": "555-5678",
  "receiverAddress": "Carrera 2",
  "originCityId": 1,
  "destinationCityId": 2,
  "weightKg": 5.0,
  "lengthCm": 20,
  "widthCm": 20,
  "heightCm": 10,
  "packageType": "Caja",
  "serviceType": "Express",
  "price": 15000
}'
```

### 2. Asignar vehículo a un envío
**POST** `/api/shipments/assign-vehicle`

```bash
curl -X POST https://localhost:7117/api/shipments/assign-vehicle \
-H "Content-Type: application/json" \
-d '{
  "shipmentId": 1,
  "vehicleId": 101,
  "userId": "user-guid-123"
}'
```

### 3. Consultar historial de un envío
**GET** `/api/shipments/{id}/history`

```bash
curl -X GET https://localhost:7117/api/shipments/1/history
```

### 4. Consultar envíos retrasados
**GET** `/api/shipments/delayed?startDate=2023-01-01&endDate=2023-12-31`

### 5. Eficiencia del conductor
**GET** `/api/shipments/drivers/{driverId}/efficiency?startDate=...&endDate=...`

---

## 🧪 Pruebas
Ejecute el siguiente comando para correr la suite de pruebas:
```bash
dotnet test
```
