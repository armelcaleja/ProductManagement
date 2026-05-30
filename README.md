# ProductManagement API

A RESTful Web API for managing products, packages, items, and package types. Built with ASP.NET Core (.NET 10) using Entity Framework Core, JWT authentication, API versioning, and structured logging.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB or full instance)
- [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd ProductManagement
```

### 2. Configure the database connection

Update the connection string in `appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply database migrations

**Option A — Using .NET CLI:**

```bash
dotnet ef database update
```

**Option B — Using Visual Studio:**

1. Open NuGet Package Manager → Package Manager Console
2. Enter `Update-Database`

### 4. Run the application

```bash
dotnet run
```

The API will be available at `https://localhost:7218`.

On first run in Development mode, the application automatically seeds the database with sample data including products, packages, items, package types, and a default user. This only runs if the database is empty (no existing products).

### 5. Access Swagger UI

Navigate to Swagger UI in your browser to view the interactive API documentation. You can browse all endpoints, view request/response schemas, and test API calls directly from the UI.

- **V1:** `https://localhost:7218/swagger/index.html?urls.primaryName=ProductManagement+API+V1`
- **V2:** `https://localhost:7218/swagger/index.html?urls.primaryName=ProductManagement+API+V2`

## Authentication

The API uses JWT Bearer tokens. All endpoints (except register and login) require authentication.

**Workflow:**

1. Register a user: `POST /api/v1/Auth/register`
2. Login to get a token: `POST /api/v1/Auth/login`
3. Include the token in subsequent requests: `Authorization: Bearer <token>`

**Default seeded user:**
- Username: `armelcaleja`
- Password: `asdasdasd`

### JWT Configuration

JWT settings are in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Secret": "8eOYK1cAxZM37XobJbOb2qCdc93hkTya",
    "Issuer": "https://localhost:7218",
    "Audience": "https://localhost:7218",
    "ExpiryInMinutes": 60
  }
}
```

## API Versioning

The API uses URL segment-based versioning:

- **v1** — Products only (flat response)
- **v2** — Products, Packages, Items, Package Types (nested responses)

## Endpoints

### Auth

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/Auth/register` | Register a new user |
| POST | `/api/v1/Auth/login` | Login and receive JWT token |

### Products (v1)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Products` | Get all products |
| GET | `/api/v1/Products/{id}` | Get product by ID |
| POST | `/api/v1/Products` | Create a product |
| PUT | `/api/v1/Products/{id}` | Update a product |
| DELETE | `/api/v1/Products/{id}` | Soft delete a product |

### Products (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2/Products` | Get all products with nested packages |
| GET | `/api/v2/Products/{id}` | Get product by ID with nested packages |
| POST | `/api/v2/Products` | Create a product |
| PUT | `/api/v2/Products/{id}` | Update a product |
| DELETE | `/api/v2/Products/{id}` | Soft delete a product |

### Items (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2/Items` | Get all items |
| GET | `/api/v2/Items/{id}` | Get item by ID |
| POST | `/api/v2/Items` | Create an item |
| POST | `/api/v2/Items/assign-to-package` | Assign item to a package |
| DELETE | `/api/v2/Items/unassign-from-package` | Unassign item from a package (soft delete) |
| PUT | `/api/v2/Items/{id}` | Update an item |
| DELETE | `/api/v2/Items/{id}` | Soft delete an item |

### Packages (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2/Packages` | Get all packages |
| GET | `/api/v2/Packages/{id}` | Get package by ID |
| POST | `/api/v2/Packages` | Create a package |
| PUT | `/api/v2/Packages/{id}` | Update a package |
| DELETE | `/api/v2/Packages/{id}` | Soft delete a package |

### Package Types (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2/PackageTypes` | Get all package types |
| GET | `/api/v2/PackageTypes/{id}` | Get package type by ID |
| POST | `/api/v2/PackageTypes` | Create a package type |
| PUT | `/api/v2/PackageTypes/{id}` | Update a package type |
| DELETE | `/api/v2/PackageTypes/{id}` | Soft delete a package type |

## Soft Delete

All delete operations use soft delete. Records are not removed from the database — instead, an `IsDeleted` flag is set to `true`. Soft-deleted records are automatically excluded from all queries via EF Core global query filters.

This applies to: Products, Items, Packages, Package Types, and Package-Item assignments.

## Project Structure

```
ProductManagement/
├── Controllers/
│   ├── AuthController.cs
│   ├── v1/
│   │   └── ProductsController.cs
│   └── v2/
│       ├── ProductsController.cs
│       ├── PackagesController.cs
│       ├── ItemsController.cs
│       └── PackageTypesController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextFactory.cs
├── DTOs/
├── Extensions/
│   ├── DatabaseSeeder.cs
│   ├── DependencyInjection.cs
│   └── WebApiInfrastructure/
├── Interfaces/
├── Migrations/
├── Model/
│   ├── Product.cs
│   ├── Package.cs
│   ├── Item.cs
│   ├── PackageItem.cs
│   ├── PackageType.cs
│   ├── AuditLog.cs
│   └── User.cs
├── Services/
├── Logs/
├── Program.cs
├── appsettings.json
└── ProductManagement.postman_collection.json
```

## Key Dependencies

| Package | Purpose |
|---------|---------|
| Microsoft.EntityFrameworkCore.SqlServer | SQL Server database provider |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT authentication |
| Asp.Versioning.Mvc | API versioning |
| BCrypt.Net-Next | Password hashing |
| Serilog.AspNetCore | Structured logging (console + file) |
| Swashbuckle.AspNetCore | Swagger UI for interactive API documentation |

## Logging

Structured logging is configured via Serilog. Logs are written to:

- Console output
- Rolling file at `Logs/log-{date}.txt` (retained for 7 days)

## Postman Collection

A Postman collection is included at `ProductManagement.postman_collection.json`. Import it into Postman to test all endpoints. The login request automatically saves the JWT token to a collection variable for use in authenticated requests.

## Running Tests

```bash
dotnet test
```
