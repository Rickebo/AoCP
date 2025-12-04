# AoCP Platform

AoCP is a desktop platform for exploring Advent of Code solutions. The backend is an ASP.NET Core service that hot-loads C# solutions on the fly, and the frontend is an Electron application that visualises problem metadata, solver progress, AI-assisted summaries/discussions, and renderable grid outputs.

## Key features

- **Multiple sources and authors** – problems are grouped by source (e.g., Advent of Code, Codelight) and year, then filtered by author in the navigation bar.【F:src/frontend/src/renderer/src/components/NavigationBar.tsx†L24-L104】
- **Live solving experience** – submit puzzle input once to solve all parts of a day, watch structured logs, view grids, copy solutions, and see elapsed timings inline.【F:src/frontend/src/renderer/src/components/ProblemSet.tsx†L63-L190】
- **Grid capture** – render grid-based visualisations to WebM videos with custom resolution and playback speed, saved via the desktop shell.【F:src/frontend/src/renderer/src/components/ProblemSet.tsx†L66-L152】【F:src/frontend/src/renderer/src/services/GifService.ts†L7-L66】
- **AI assistance** – optionally fetch raw Advent of Code descriptions, summarise them with OpenRouter, and chat about each part using the bundled AI Discussion tab (with optional live reload of your solution file for context).【F:src/frontend/src/renderer/src/components/ProblemDiscussion.tsx†L1-L218】【F:src/frontend/src/renderer/src/components/SettingsModal.tsx†L50-L210】

## Architecture at a glance

- **Backend** (`src/backend`) &mdash; ASP.NET Core 8 host (`Backend/`) that exposes metadata and solving endpoints, with shared domain types under `Common/` and reusable helpers in `Lib/`.
- **Problem loader** &mdash; `ProblemLoaderService` watches `Backend/Problems`, compiles any `.cs` file it finds, and injects new problem sets (across all registered sources) without restarting the server.【F:src/backend/Backend/Services/ProblemLoaderService.cs†L69-L140】
- **Problem collections** &mdash; each `ProblemCollection` declares a source and year; the loader indexes them so metadata and solving lookups resolve by `(source, year, author, set, problem)`.【F:src/backend/Backend/Services/ProblemService.cs†L12-L78】【F:src/backend/Backend/Problems/ProblemCollection.cs†L9-L93】
- **Frontend** (`src/frontend`) &mdash; Electron main process plus a React/TypeScript renderer that talks to the backend over HTTP and WebSockets.

## Prerequisites

Install the following tools before starting:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [Node.js 20+](https://nodejs.org/) (needed for Electron and Vite)
- npm (bundled with Node.js)

Optional tokens:

- Advent of Code session token if you want the app to fetch puzzle input or the raw HTML description automatically.
- [OpenRouter](https://openrouter.ai/) API token if you want AI summaries or the discussion tab to work.

## Backend setup

1. `cd src/backend/Backend`
2. Restore dependencies: `dotnet restore`
3. Run the service on the port expected by the desktop app: `dotnet run --urls http://localhost:8080`

The server serves REST endpoints (e.g. `http://localhost:8080/metadata`) and WebSocket solve streams. `ProblemLoaderService` will automatically watch `Backend/Problems` (including new sources such as `Year2025Codelight`) for new or modified C# files and recompile them while the app is running.【F:src/backend/Backend/Services/ProblemLoaderService.cs†L24-L105】【F:src/backend/Backend/Problems/Year2025Codelight/Year2025CodelightMetadata.cs†L1-L17】

## Frontend setup

1. `cd src/frontend`
2. Install packages: `npm install`
3. Start the Electron app in development mode: `npm run dev`

Electron will open a desktop window and connect to `http://localhost:8080` by default. If you run the backend on a different URL, open **Settings → Backend** in the UI and add/activate the correct endpoint.【F:src/frontend/src/renderer/src/components/SettingsModal.tsx†L175-L210】

To fetch puzzle input automatically, paste your Advent of Code session cookie (`session=…`) into **Settings → AoC token**. To enable AI-powered summaries and discussions, add an OpenRouter API token and configure the desired models/prompts in the **AI Summarization** and **AI Discussion** accordions inside Settings.【F:src/frontend/src/renderer/src/components/SettingsModal.tsx†L50-L174】 The app stores settings locally.

Puzzle descriptions can be fetched live from adventofcode.com when `Retrieve description` is enabled; AI summaries can then condense them for quicker reading, and the Discussion tab can answer questions using both the description and your solution file (if available).【F:src/frontend/src/renderer/src/components/ProblemDiscussion.tsx†L19-L120】

When viewing a problem, you can:

- Solve all parts with one input submission, copy solutions, and inspect logs, grids, and elapsed times in dedicated tabs.【F:src/frontend/src/renderer/src/components/ProblemSet.tsx†L76-L190】
- Render grid animations to WebM with custom resolution and speed settings directly from the solver toolbar.【F:src/frontend/src/renderer/src/components/ProblemSet.tsx†L80-L153】【F:src/frontend/src/renderer/src/services/GifService.ts†L7-L66】

## Adding your own solutions

1. Navigate to `src/backend/Backend/Problems` and pick the correct year folder. If the year does not exist yet:
   - Create `YearYYYYMetadata.cs` (copy `Year2024Metadata.cs`) and adjust the `Year` value.
   - Register the new year in `Startup.AddAocPlatform()`.
2. Inside the target year, create a folder named after your handle (e.g. `YourName`). Add a new C# file for the puzzle day (for example, `Day01.cs`).
3. Implement a class that derives from `ProblemSet`:
   - Set `ReleaseTime` to the correct date.
   - Fill the `Problems` list with nested classes that derive from `Problem`. Override `Name`, `Description`, and `Solve`.
   - Use the helpers on `Reporter` to stream progress, grids, and final answers back to the frontend.
   - Leave the class in the same file you edit; `ProblemSet` automatically captures the solution file path so the Discussion tab can read it later.【F:src/backend/Backend/Problems/ProblemSet.cs†L9-L33】
4. Save the file. The backend watcher will compile it immediately; open the Electron app, refresh the metadata list, and your problem set will appear under your name for that year and source.【F:src/backend/Backend/Services/ProblemLoaderService.cs†L69-L140】【F:src/backend/Backend/Problems/ProblemCollection.cs†L9-L93】

Solutions can rely on utilities in `src/backend/Common` and `src/backend/Lib`. If you add new shared helpers, keep them general-purpose and document them briefly in code comments.

## Building for production

- Backend: `dotnet publish -c Release` from `src/backend/Backend`.
- Frontend: `npm run build` to produce packaged Electron artifacts (platform-specific targets are defined in `package.json`).

## Troubleshooting

- **Switching to HTTPS later**: the recommended development setup uses plain HTTP so you can run the app without provisioning certificates. If you prefer HTTPS, trust the development certificate (`dotnet dev-certs https --trust`) and update the backend URL in the Electron settings accordingly.
- **Backend changes not appearing**: confirm you are editing files under `Backend/Problems`. Other directories are ignored by the watcher; restarting the backend will pick up files that were added while it was offline.
- **Fetching AoC input fails**: double-check that your session token is exactly 128 hexadecimal characters (without the `session=` prefix) and that the cookie has not expired.
