# PruebaYape

Proyecto de ejemplo para gestión de transacciones y validación anti-fraude usando .NET, PostgreSQL y Kafka.

---

## Requisitos

-   [.NET 8 SDK](https://dotnet.microsoft.com/download)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/)
-   [pgAdmin](https://www.pgadmin.org/download/) (opcional, para ver la base de datos)
-   [Postman](https://www.postman.com/) o curl (opcional, para probar endpoints)

---

## 1. Levantar infraestructura (PostgreSQL y Kafka)

Desde la raíz del proyecto:

```sh
docker compose -f docker-compose-yaml up -d
```

Esto levantará:

-   PostgreSQL en el puerto 5432
-   Zookeeper y Kafka en el puerto 9092

---

## 2. Aplicar migraciones de la base de datos

Desde la raíz del proyecto:

```sh
dotnet ef database update --project Infrastructure --startup-project Api
```

Si es la primera vez, crea la migración inicial:

```sh
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api
dotnet ef database update --project Infrastructure --startup-project Api
```

---

## 3. Crear los topics de Kafka (si es necesario)

Entra al contenedor de Kafka:

```sh
docker exec -it <nombre_contenedor_kafka> bash
```

Crea los topics:

```sh
kafka-topics --create --topic transactions --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
kafka-topics --create --topic transactions-status --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```

---

## 4. Levantar los servicios

En dos terminales diferentes, ejecuta:

**API:**

```sh
dotnet run --project Api
```

**AntiFraudService:**

```sh
dotnet run --project AntiFraudService
```

---

## 5. Probar los endpoints

### Crear una transacción

POST a `http://localhost:5012/api/transactions`:

```json
{
    "sourceAccountId": "GUID-VAL1",
    "targetAccountId": "GUID-VAL2",
    "transferTypeId": 1,
    "value": 120
}
```

### Consultar una transacción

GET a `http://localhost:5012/api/transactions/{transactionId}`

---

## 6. Verificar la base de datos

Puedes usar **pgAdmin** o cualquier cliente PostgreSQL:

-   Host: `localhost`
-   Puerto: `5432`
-   Usuario: `postgres`
-   Contraseña: `postgres`
-   Base de datos: `postgres`

Consulta la tabla:

```sql
SELECT * FROM "Transactions";
```

El campo `Status` indica el estado de la transacción (`0=Pending`, `1=Approved`, `2=Rejected`).

---

## 7. Detener los servicios

```sh
docker compose -f docker-compose-yaml down
```

---

## Notas

-   Si cambias la configuración de Kafka o la base de datos, actualiza los archivos `appsettings.json` y `appsettings.Development.json`.
-   Puedes modificar el mapeo del enum `Status` para que se guarde como texto en vez de entero si lo prefieres.

---
