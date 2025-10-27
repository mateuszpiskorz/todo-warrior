# TodoWarrior

Tech stack
- Backend: .NET 8 (ASP.NET Core minimal APIs), Entity Framework Core (SQLite), SignalR, FluentValidation
- Frontend: Vue 3, Vite, TypeScript, Pinia
- Tests: xUnit (backend)
- Dev tooling: dotnet CLI, npm, dotnet-ef
- Containers: Docker, docker-compose

## Run with Docker (recommended)
- From repository root:
  ```bash
  docker-compose up --build
  ```
- Notes:
  - Backend Dockerfile lives under `backend/` and listens on port 8080.
  - Frontend Dockerfile lives under `frontend/TodoWarrior.Client/` and exposes Vite on 5173.
  - Persistent SQLite volume: `sqldata` (declared in docker-compose).


## Quick start — prerequisites
1. .NET SDK 8.0+
2. Node.js 18+ / npm or PNPM
3. dotnet-ef tool (for migrations): `dotnet tool install --global dotnet-ef`
4. Docker & docker-compose (optional, for containers)

Run locally (development)
1. Backend
   - Restore & build:
     ```bash
     cd backend/TodoWarrior.Api
     dotnet restore
     dotnet build
     ```
   - Create initial EF migration (only once):
     ```bash
     dotnet ef migrations add InitialCreate
     ```
     This creates `Migrations/` files. The app calls `Database.Migrate()` on startup to apply migrations.
   - Run:
     ```bash
     dotnet run
     ```
   - Defaults:
     - HTTP port: 8080 (ASPNETCORE_URLS=http://0.0.0.0:8080)
     - SQLite DB path can be configured via `SQLITE_PATH` env var (default: `data/app.db`)

2. Frontend
   - Install deps and run dev server:
     ```bash
     cd frontend/TodoWarrior.Client
     npm ci
     npm run dev -- --host 0.0.0.0
     ```
   - Default Vite port: 5173 (can be configured in `vite.config.ts` or via `--port`)


