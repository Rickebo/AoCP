# AoCP Agent Guidelines

## Repository overview
- The backend lives in `src/backend`. It is a .NET 8 solution composed of the ASP.NET Core host in `Backend/`, shared problem domain types in `Common/`, and general-purpose helpers in `Lib/`.
- The desktop frontend lives in `src/frontend`. It is an Electron application that serves a React/TypeScript renderer (`src/frontend/src/renderer`) and a Node-based main process (`src/frontend/src/main`).

## General expectations
- Prefer small, focused commits and describe the impact clearly in the PR message.
- When you touch backend and frontend code in the same change, call out cross-cutting implications explicitly in reviews and documentation updates.
- Keep documentation up to date whenever you add new features or change existing behaviour.

## Keeping this guide current
- Treat this file as the source of truth for agent workflow: update it whenever processes, entry points, or conventions change.
- Mirror any new setup steps or defaults you introduce elsewhere (e.g., README, scripts) so instructions stay aligned.
- When adding a new tech stack area (packages, build targets, environments), add the expectations and testing commands here.
- If guidance becomes outdated during a task, fix it before handing off your change set.

## Backend (.NET) guidelines
- Target .NET 8.0 and keep the existing C# 12 style: file-scoped namespaces, top-level statements where already used, `var` for obvious type inference, collection expressions (e.g. `[]`) instead of explicit constructors, and expression-bodied members when they improve readability. Mirror the patterns used in existing files like `Backend/Program.cs`, `Backend/Startup.cs`, and the problem definitions under `Backend/Problems`.
- Place new Advent of Code solutions under `src/backend/Backend/Problems/YearYYYY/<author>/`. Derive from `ProblemSet` and nested `Problem` types, set a matching `ReleaseTime`, and provide a non-empty `Name`. Use `Reporter` helpers from `Common/Reporter.cs` to stream updates/results back to the client.
- If you introduce a new year, add a `YearYYYYMetadata` class mirroring `Year2024Metadata` and register it inside `Startup.AddAocPlatform` so the loader and API can discover the new problems.
- The backend hot-reloads problem assemblies at runtime via `ProblemLoaderService`. When changing how problems are compiled or loaded, make sure the watcher still monitors `Backend/Problems` recursively and consider performance implications.
- Add new third-party dependencies through the relevant `.csproj` file and keep `aocp.sln` consistent.
- Validate new API routes or services with integration or unit tests when feasible.

## Frontend (Electron + React) guidelines
- Use functional React components and hooks, following the structure in `src/frontend/src/renderer`. Keep shared state inside React context providers (e.g. `BackendContext`, `SettingsContext`) instead of ad-hoc globals.
- Type everything with TypeScript interfaces/types. When introducing new services, group them under `src/frontend/src/renderer/src/services` and expose small, composable APIs similar to `ProblemService` and `MetadataService`.
- Run `npm run lint` and `npm run typecheck` after making TypeScript changes, and let Prettier (`npm run format`) handle formatting.
- Update Electron configuration (`electron.vite.config.ts`, builder configs) only when necessary, and keep platform-specific packaging scripts in sync.

## Testing & tooling
- For backend changes, restore and run from `src/backend/Backend` (`dotnet restore`, `dotnet run --urls https://localhost:8080`). Use `dotnet test` if/when tests are added.
- For frontend work, install dependencies with `npm install` in `src/frontend`, then use `npm run dev` for the live Electron app or `npm run build` for production builds. Keep the backend running so the renderer can reach the API (`https://localhost:8080` by default).
- Document any new manual setup steps (certificates, environment variables, etc.) in the root README when they affect contributors or users.
