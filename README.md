# RecycleHub API

The .NET 8 backend for EcoFind Ghana. See the [root README](../README.md) for the full project overview and the API endpoint reference.

## Stack
- ASP.NET Core 8 Web API
- Entity Framework Core 8 + Npgsql (PostgreSQL)
- ASP.NET Identity + JWT bearer auth
- Mapster for DTO mapping
- Repository + Unit of Work pattern

## Projects
| Project | Role |
|---------|------|
| `RecycleHub.Api` | Controllers, services, DTOs, startup, auth |
| `RecycleHub.Pg.Sdk` | EF Core DbContext, entities, repositories, configurations |
| `RecycleHub.Utils` | `ApiResponse<T>`, JSON serialization, Haversine distance |

## Prerequisites
- .NET 8 SDK
- PostgreSQL (local instance, or use the root `docker compose`)

## Configuration
The connection string lives under `ConnectionStrings:RecycleHubDb`. For local dev it's in `src/RecycleHub.Api/appsettings.Development.json`:

```
Server=localhost;Port=5432;Database=recyclehub;User Id=postgres;Password=postgres;
```

JWT settings (`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, expirations) and `Cors:AllowedOrigins` are in `appsettings.json`. Any value can be overridden with environment variables (e.g. `ConnectionStrings__RecycleHubDb`, `Jwt__Key`).

## Run

```sh
dotnet run --project src/RecycleHub.Api
```

- API: http://localhost:5107 (https profile) — Swagger at `/swagger` (Development only)
- **Migrations are applied automatically on startup** (`ApplyPendingMigrations`), and the `Admin`/`User` roles are seeded (`SeedRolesAsync`).

## Migrations
To add a new migration after changing entities:

```sh
dotnet ef migrations add <Name> \
    --project src/RecycleHub.Pg.Sdk \
    --startup-project src/RecycleHub.Api
```

## Making a user an admin
Registration assigns the `User` role. To grant `Admin`, insert a row into `AspNetUserRoles` linking the user's id to the `Admin` role id (find both in `AspNetUsers` / `AspNetRoles`).

## NuGet
`Nuget.config` configures both nuget.org and a private Hubtel Azure DevOps feed. All current dependencies resolve from nuget.org.
