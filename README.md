# 🍼 Baby Food Checklist API

A production-grade **ASP.NET Core 8 Web API** backend for the *"100 First Foods" (100 Перших Продуктів)* baby food checklist mobile application. Helps parents track which foods their baby has tried, with ratings, notes, and progress statistics.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Tech Stack](#tech-stack)
3. [Architecture](#architecture)
4. [Prerequisites](#prerequisites)
5. [Getting Started on Windows](#getting-started-on-windows)
6. [API Documentation](#api-documentation)
7. [Database](#database)
8. [Testing](#testing)
9. [Configuration](#configuration)
10. [Project Structure](#project-structure)
11. [Seed Data](#seed-data)
12. [Localization](#localization)
13. [Docker](#docker)
14. [Contributing](#contributing)
15. [License](#license)

---

## Project Overview

This API serves as the backend for an Expo/React Native mobile app. Features:

- 🥦 **100+ default food products** organized into 9 categories (seeded automatically)
- ✅ **User entries** — track tried status, first-tried date, rating (Liked/Neutral/Disliked), notes
- 📊 **Statistics** — overall and per-category progress percentage
- 🌍 **Bilingual** — Ukrainian & English names for all products and categories
- 📄 **OData support** — `$filter`, `$select`, `$orderby`, `$top`, `$skip`, `$count` on list endpoints
- 🏥 **Health check** endpoint for monitoring

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 + Npgsql |
| Architecture | Clean Architecture (Domain → Application → Infrastructure → API) |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| Mapping | AutoMapper 13 |
| OData | Microsoft.AspNetCore.OData 9 |
| Logging | Serilog (structured, console + file sinks) |
| API Versioning | Asp.Versioning 8 |
| Documentation | Swagger / OpenAPI (Swashbuckle 6) |
| Testing | xUnit + FluentAssertions + Moq + AutoFixture |
| Containerization | Docker + docker-compose |

---

## Architecture

The solution follows **Clean Architecture** with strict dependency rules:

```
┌─────────────────────────────────────────────────────────────────┐
│  API (BabyFoodChecklist.API)                                    │
│  • Controllers, Middleware, Program.cs                          │
├─────────────────────────────────────────────────────────────────┤
│  Infrastructure (BabyFoodChecklist.Infrastructure)              │
│  • EF Core DbContext, Entity Configurations, Repositories, Seeder │
├─────────────────────────────────────────────────────────────────┤
│  Application (BabyFoodChecklist.Application)                    │
│  • CQRS Queries/Commands (MediatR), Validators, DTOs, Mapping   │
├─────────────────────────────────────────────────────────────────┤
│  Domain (BabyFoodChecklist.Domain)                              │
│  • Entities (Product, UserProductEntry), Events, Enums, Interfaces│
└─────────────────────────────────────────────────────────────────┘
```

- **Domain** has no dependencies on any other layer.
- **Application** depends only on Domain.
- **Infrastructure** depends on Application + Domain.
- **API** depends on Application + Infrastructure (for DI wiring only).

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) (8.0.419 or later)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- *(Optional)* [pgAdmin](https://www.pgadmin.org/) for database management (included in docker-compose)

---

## Getting Started on Windows

### 1. Clone the repository

```bash
git clone https://github.com/romanferents/baby-food-checklist-api.git
cd baby-food-checklist-api
```

### 2. Start PostgreSQL with Docker

```bash
docker-compose up postgres -d
```

### 3. Run the API

```bash
dotnet run --project src/BabyFoodChecklist.API
```

The API will:
- Connect to PostgreSQL
- Auto-create the database schema (`EnsureCreated`)
- Seed all 100+ default products

### 4. Open Swagger UI

Navigate to: **http://localhost:5247** (or the HTTPS port shown in console output — check `Properties/launchSettings.json`)

---

## API Documentation

Interactive Swagger UI is available at the root URL when the API is running.

### Endpoints (v1)

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/v1/products/{id}` | Get product by ID |
| `POST` | `/api/v1/products` | Create custom product |
| `PUT` | `/api/v1/products/{id}` | Update custom product |
| `DELETE` | `/api/v1/products/{id}` | Delete custom product |
| `POST` | `/api/v1/entries` | Create or update entry (upsert) |
| `DELETE` | `/api/v1/entries/{id}` | Delete entry |
| `GET` | `/api/v1/statistics` | Get overall + per-category stats |
| `GET` | `/api/v1/categories` | List all categories with UA/EN names |
| `GET` | `/health` | Database health check |

### OData Endpoints

| Method | Path | Description |
|---|---|---|
| `GET` | `/odata/v1/Products` | Query products (supports `$filter`, `$select`, `$orderby`, `$top`, `$skip`, `$count`, `$expand`) |
| `GET` | `/odata/v1/Entries` | Query user entries (same OData operators) |

### OData Query Parameters

List endpoints use OData for filtering, sorting, and pagination:

```
/odata/v1/Products?$filter=Category eq 'Fruits'&$orderby=SortOrder&$top=20&$skip=0&$count=true
```

### Error Responses (RFC 7807)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Entity \"Product\" (3fa85f64...) was not found."
}
```

---

## Database

### Schema

| Table | Description |
|---|---|
| `Products` | All food products (default + custom) |
| `UserProductEntries` | User's tracking entries per product (1:1) |

### Migrations

The app uses `EnsureCreated()` for development simplicity. For production, run migrations:

```bash
dotnet ef migrations add InitialCreate --project src/BabyFoodChecklist.Infrastructure --startup-project src/BabyFoodChecklist.API
dotnet ef database update --project src/BabyFoodChecklist.Infrastructure --startup-project src/BabyFoodChecklist.API
```

---

## Testing

### Run All Tests

```bash
dotnet test BabyFoodChecklist.sln
```

### Test Coverage Areas

- ✅ Product query handlers (GetById)
- ✅ Product command handlers (Create)
- ✅ Entry command validators (UpsertEntry)
- ✅ Categories query handler
- ✅ Products controller (Create, GetById, Update, Delete)

---

## Configuration

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=babyfoodchecklist;Username=postgres;Password=postgres"
  },
  "Serilog": {
    "MinimumLevel": { "Default": "Information" }
  }
}
```

### Environment Variables (Docker / CI)

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` |

---

## Project Structure

```
baby-food-checklist-api/
├── src/
│   ├── BabyFoodChecklist.Domain/        # Entities, events, enums, interfaces
│   │   ├── Common/                      # BaseEntity, BaseAuditableEntity, NamedEntity
│   │   ├── Entities/                    # Product, UserProductEntry
│   │   ├── Enums/                       # ProductCategory, FoodRating
│   │   ├── Events/                      # BaseEvent (domain events)
│   │   └── Interfaces/                  # IApplicationDbContext, IBaseRepository
│   │
│   ├── BabyFoodChecklist.Application/   # CQRS, DTOs, validators, mapping, behaviors
│   │   ├── Common/                      # Exceptions, validation & logging behaviors
│   │   ├── DTOs/                        # ProductDto, UserProductEntryDto, etc.
│   │   ├── Features/                    # Products, Entries, Statistics, Categories
│   │   └── MappingProfiles/             # AutoMapper profiles
│   │
│   ├── BabyFoodChecklist.Infrastructure/  # EF Core, repositories, seeder
│   │   ├── Data/                        # DbContext, entity configurations, OData, seeders, interceptors
│   │   └── Repositories/               # BaseRepository<T>
│   │
│   └── BabyFoodChecklist.API/           # Controllers, middleware, startup
│       ├── Controllers/                 # Products, Entries, Statistics, Categories + OData
│       └── Middleware/                  # ExceptionHandlingMiddleware
│
├── tests/
│   └── BabyFoodChecklist.Tests/         # Unit tests (xUnit + Moq + AutoFixture)
│
├── docker-compose.yml
├── BabyFoodChecklist.sln
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
└── .editorconfig
```

---

## Seed Data

On first startup the API seeds **126 default products** across 9 categories:

| Category | Count |
|---|---|
| Vegetables & Greens (Овочі та зелень) | 37 |
| Fruits (Фрукти) | 26 |
| Dairy (Молочні продукти) | 11 |
| Meat & Animal Products (М'ясо) | 11 |
| Grains & Legumes (Крупи) | 13 |
| Nuts & Seeds (Горіхи, насіння) | 12 |
| Fish & Seafood (Риба) | 16 |
| Spices (Спеції) | 7 |

All seeded products have `IsDefault = true` and cannot be modified or deleted via the API.

---

## Localization

Every product and category has both **Ukrainian** and **English** names:

```json
{
  "nameUk": "Морква",
  "nameEn": "Carrot",
  "categoryNameUk": "Овочі та зелень",
  "categoryNameEn": "Vegetables & Greens"
}
```

The mobile app chooses which language to display based on the user's locale.

---

## Docker

### Start everything (API + PostgreSQL + pgAdmin)

```bash
docker-compose up -d
```

| Service | URL |
|---|---|
| API | http://localhost:5247 |
| pgAdmin | http://localhost:5050 (admin@admin.com / admin) |
| PostgreSQL | localhost:5432 |

### Start only the database (for local development)

```bash
docker-compose up postgres -d
```

---

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Follow the `.editorconfig` code style
4. Write tests for new functionality
5. Ensure all tests pass: `dotnet test BabyFoodChecklist.sln`
6. Submit a pull request

### Code Style

- Follow C# naming conventions (PascalCase for public members)
- XML documentation on all public APIs
- Prefer `async/await` over synchronous I/O
- Use `CancellationToken` in all async methods

---

## License

MIT License — see [LICENSE](LICENSE) for details.
