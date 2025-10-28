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

## TodoWarrior - User Guide
### Getting Started
TodoWarrior is a retro-styled task management application with real-time reminders. Access the app at http://localhost:5173 (frontend) after starting the services.

### Core Features
Creating Tasks
1. Quick Create: Press the N key anywhere in the app, or click the "+ New Task" button

2. Fill in the form:

* Title (required): Brief task name (max 200 characters)
* Description (optional): Detailed notes (max 1000 characters)
* Due Date: When the task should be completed
* Reminder At: Set a specific date and time for a notification
* Done checkbox: Mark completion status (can also toggle from list view)
* Click "Save" to create the task

Managing Tasks
From the task list, you can:

* Mark as Done/Undo: Click the "Done" button to toggle completion status
* Edit: Click "Edit" to modify any task details
* Delete: Click "Delete" (you'll be asked to confirm)

Viewing Tasks

Filter options (top toolbar):

* Date Filter: Click the date picker to show only tasks due on a specific date
  * Click "Clear" to remove the filter
* Show Completed: Toggle the checkbox to include/exclude completed tasks in the list


Real-Time Reminders

When a task's reminder time arrives (within 5 minutes):

* You'll receive a toast notification in the top-right corner
* The notification includes the task title
* Notifications auto-dismiss after 5 seconds, or click "×" to dismiss manually


