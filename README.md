# AoCP Platform

AoCP is a desktop platform for exploring Advent of Code solutions. The backend is an ASP.NET Core service that hot-loads C# solutions on the fly, and the frontend is an Electron application that visualises problem metadata, solver progress, AI-assisted summaries/discussions, and renderable grid outputs.

## Key features

- **Multiple sources and authors**: problems are grouped by source (Advent of Code, Codelight, etc.) and year, then filtered by author in the navigation bar.
- **Live solving experience**: submit puzzle input once to solve all parts of a day, watch structured logs, view grids, copy solutions, and see elapsed timings inline.
- **Grid capture**: render grid-based visualisations to WebM videos with custom resolution and playback speed, saved via the desktop shell.
- **AI assistance**: optionally fetch raw Advent of Code descriptions, summarise them with OpenRouter, and chat about each part using the bundled AI Discussion tab (with optional live reload of your solution file for context).

## Architecture at a glance

- **Backend** (`src/backend`) - ASP.NET Core 8 host (`Backend/`) that exposes metadata and solving endpoints, with shared domain types under `Common/` and reusable helpers in `Lib/`.
- **Problem loader** - `ProblemLoaderService` watches the `Backend/Problems/Year*/<author>` folders present when the service starts, compiles `.cs` files it finds, and injects new problem sets without restarting the server. If you add a new author or year directory while the backend is running, restart it so the watcher discovers the new path.
- **Problem collections** - each `ProblemCollection` declares a source and year; the loader indexes them so metadata and solving lookups resolve by `(source, year, author, set, problem)`.
- **Frontend** (`src/frontend`) - Electron main process plus a React/TypeScript renderer that talks to the backend over HTTPS and WebSockets.

## Prerequisites

Install the following tools before starting:

- .NET SDK 8.0
- Node.js 20+ (needed for Electron and Vite)
- npm (bundled with Node.js)

Optional tokens:

- Advent of Code session token if you want the app to fetch puzzle input or the raw HTML description automatically.
- OpenRouter API token if you want AI summaries or the discussion tab to work.

## Backend setup

1. `cd src/backend/Backend`
2. Trust the local HTTPS certificate (first time only): `dotnet dev-certs https --trust`
3. Restore dependencies: `dotnet restore`
4. Run the service on the port expected by the desktop app: `dotnet run --urls https://localhost:8080`

The server serves REST endpoints (e.g. `https://localhost:8080/metadata`) and WebSocket solve streams. `ProblemLoaderService` will watch the existing `Backend/Problems` subdirectories for modified C# files and recompile them while the app is running. If you change the URL or port, update the renderer settings to match.

## Frontend setup

1. `cd src/frontend`
2. Install packages: `npm install`
3. Start the Electron app in development mode: `npm run dev`

Electron will open a desktop window and connect to `https://localhost:8080` by default. If you run the backend on a different URL (or on HTTP), open **Settings -> Backend** in the UI and add/activate the correct endpoint.

To fetch puzzle input automatically, paste your Advent of Code session cookie (`session=<token>`) into **Settings -> AoC token**. To enable AI-powered summaries and discussions, add an OpenRouter API token and configure the desired models/prompts in the **AI Summarization** and **AI Discussion** accordions inside Settings. The app stores settings locally.

Puzzle descriptions can be fetched live from adventofcode.com when **Retrieve description** is enabled; AI summaries can then condense them for quicker reading, and the Discussion tab can answer questions using both the description and your solution file (if available).

When viewing a problem, you can:

- Solve all parts with one input submission, copy solutions, and inspect logs, grids, and elapsed times in dedicated tabs.
- Render grid animations to WebM with custom resolution and speed settings directly from the solver toolbar.

## Adding your own solutions

1. Navigate to `src/backend/Backend/Problems` and pick the correct year folder. If the year does not exist yet:
   - Create `YearYYYYMetadata.cs` (copy `Year2024Metadata.cs`) and adjust the `Year` value.
   - Register the new year in `Startup.AddAocPlatform()`.
2. Inside the target year, create a folder named after your handle (e.g. `YourName`). If you create a new folder while the backend is running, restart it so the watcher picks it up. Add a new C# file for the puzzle day (for example, `Day01.cs`).
3. Implement a class that derives from `ProblemSet`:
   - Set `ReleaseTime` to the correct date.
   - Fill the `Problems` list with nested classes that derive from `Problem`. Override `Name`, `Description`, and `Solve`.
   - Use the helpers on `Reporter` to stream progress, grids, and final answers back to the frontend.
   - Leave the class in the same file you edit; `ProblemSet` automatically captures the solution file path so the Discussion tab can read it later.
4. Save the file. The backend watcher will compile it immediately; refresh the metadata list in the Electron app and your problem set will appear under your name for that year and source.

Solutions can rely on utilities in `src/backend/Common` and `src/backend/Lib`. If you add new shared helpers, keep them general-purpose and document them briefly in code comments.

## Building for production

- Backend: `dotnet publish -c Release` from `src/backend/Backend`.
- Frontend: `npm run build` creates production bundles; run `npm run build:win`, `npm run build:mac`, or `npm run build:linux` to package the Electron app for a specific platform.

## Troubleshooting

- **Certificate or mixed-content errors**: run `dotnet dev-certs https --trust`, or switch the backend to HTTP and update the renderer URL accordingly.
- **Backend changes not appearing**: confirm you are editing files under an existing `Backend/Problems/Year*/<author>` directory. If you added a new author or year folder, restart the backend so the loader can index it.
- **Fetching AoC input fails**: double-check that your session token is exactly 128 hexadecimal characters (without the `session=` prefix) and that the cookie has not expired.
