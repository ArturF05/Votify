# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build ProjVotify.sln

# Run the API (HTTP: localhost:5129, HTTPS: localhost:7053)
dotnet run --project Vofity.API/Votify.API.csproj

# Run all tests
dotnet test Votify.Core.Tests/Votify.Core.Tests.csproj

# Run a single test class
dotnet test Votify.Core.Tests/Votify.Core.Tests.csproj --filter "ClassName=<TestClass>"

# Apply EF Core migrations
dotnet ef database update --project Votify.Infrastructure --startup-project Vofity.API

# Add a new migration
dotnet ef migrations add <MigrationName> --project Votify.Infrastructure --startup-project Vofity.API
```

For AI synthesis features: Ollama must be running (`ollama serve`) with `llama3.2` pulled.

## Architecture

Three-layer architecture:

- **Votify.Core** — Domain layer: entities, DTOs, interfaces, services, factories, Observer pattern. No infrastructure dependencies.
- **Votify.Infrastructure** — Data layer: EF Core `AppDbContext`, repository implementations, migrations (PostgreSQL via Neon).
- **Vofity.API** — Presentation layer: ASP.NET Core controllers, Blazor components, service registration in `Program.cs`.
- **Votify.Core.Tests** — MSTest + Moq + EF Core InMemory, organized by feature.

## Key Design Patterns

- **Repository Pattern**: All data access goes through interfaces in `Votify.Core/Interfaces/` with implementations in `Votify.Infrastructure/Repositorios/`.
- **Factory Pattern**: `EventoFactory` (abstract) with `ConcreteEventFactory` creates `EventoHackathon` or `EventoConcurso`.
- **Proxy Pattern**: `AzureStorageProxy` wraps `AzureStorageService` to add validation before blob operations.
- **Observer Pattern**: `Votify.Core/Observer/` — `Subject`/`Observer` interfaces used for notifications (`NotificationEvent`).
- **TPH Inheritance (EF Core)**: `Evento` hierarchy (EventoHackathon, EventoConcurso) and `Votacion` hierarchy (VotacionPublico=0, VotacionJurado=1, VotacionMixta=2) use a discriminator column.

## Domain Model

- **Evento**: The central aggregate — has a Votacion, Proyectos, Participantes (EventoUsuarioRol), and DimensionesDeValor.
- **Votacion** subtypes control who can vote: Publico (anyone), Jurado (assigned jurors), Mixta (both).
- **Voto** → **VotoCriterio**: Each vote scores multiple `DimensionesDeValor` dimensions.
- **Resultado**: Aggregated scores per project, computed by `VotacionService`/`DashboardService`.
- **EventoUsuarioRol**: Composite join table — one user can hold multiple roles (Organizador, Jurado, Participante) in the same event.

## Testing Conventions

Tests use EF Core InMemory for isolation; no external DB needed. Test folders map to features:
- `LoginRegisterUser/` — auth
- `Votacion/` — voting logic, criteria, permissions
- `Participantes/` — event membership
- `Proyectos/` — project/material management
- `Patrones/` — design pattern implementations (e.g., Proxy)

## Configuration

- Database and Azure Blob Storage connection strings live in `Vofity.API/appsettings.json` (production) and `appsettings.Development.json`.
- OpenAI/Gemini API keys also go in `appsettings.json`.
- Swagger UI is available at `/swagger` in development.
