# AoCP Platform

AoCP is a desktop platform for exploring Advent of Code solutions. The backend is an ASP.NET Core service that loads C# solutions on the fly, and the frontend is an Electron application that visualises problem metadata, solver progress, and results.

## Architecture at a glance

- **Backend** (`src/backend`) &mdash; ASP.NET Core 8 host (`Backend/`) that exposes metadata and solving endpoints, with shared domain types under `Common/` and reusable helpers in `Lib/`.
- **Problem loader** &mdash; `ProblemLoaderService` watches `Backend/Problems`, compiles any `.cs` file it finds, and injects new problem sets without restarting the server.
- **Frontend** (`src/frontend`) &mdash; Electron main process plus a React/TypeScript renderer that talks to the backend over HTTP and WebSockets.

## Prerequisites

Install the following tools before starting:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [Node.js 20+](https://nodejs.org/) (needed for Electron and Vite)
- npm (bundled with Node.js)

You will also need an Advent of Code session token if you want the app to fetch puzzle input directly from adventofcode.com.

## Backend setup

1. `cd src/backend/Backend`
2. Restore dependencies: `dotnet restore`
3. Run the service on the port expected by the desktop app: `dotnet run --urls http://localhost:8080`

   The server serves REST endpoints (e.g. `http://localhost:8080/metadata`) and WebSocket solve streams. `ProblemLoaderService` will automatically watch `Backend/Problems` for new or modified C# files and recompile them while the app is running.

## Frontend setup

1. `cd src/frontend`
2. Install packages: `npm install`
3. Start the Electron app in development mode: `npm run dev`

Electron will open a desktop window and connect to `http://localhost:8080` by default. If you run the backend on a different URL, open **Settings → Backend** in the UI and add/activate the correct endpoint.

To fetch puzzle input automatically, paste your Advent of Code session cookie (`session=…`) into **Settings → AoC token**. The main process will store it securely and use it when calling `https://adventofcode.com`.

If you want AI help, add an OpenRouter API token under **Settings → OpenRouter**. The app uses it to summarize puzzle descriptions and to power the new Discussion tab that chats about each problem using both the raw description and your local solution file.

## Adding your own solutions

1. Navigate to `src/backend/Backend/Problems` and pick the correct year folder. If the year does not exist yet:
   - Create `YearYYYYMetadata.cs` (copy `Year2024Metadata.cs`) and adjust the `Year` value.
   - Register the new year in `Startup.AddAocPlatform()`.
2. Inside the target year, create a folder named after your handle (e.g. `YourName`). Add a new C# file for the puzzle day (for example, `Day01.cs`).
3. Implement a class that derives from `ProblemSet`:
   - Set `ReleaseTime` to the correct date.
   - Fill the `Problems` list with nested classes that derive from `Problem`. Override `Name`, `Description`, and `Solve`.
   - Use the helpers on `Reporter` to stream progress, grids, and final answers back to the frontend.
4. Save the file. The backend watcher will compile it immediately; open the Electron app, refresh the metadata list, and your problem set will appear under your name for that year.

Solutions can rely on utilities in `src/backend/Common` and `src/backend/Lib`. If you add new shared helpers, keep them general-purpose and document them briefly in code comments.

## Building for production

- Backend: `dotnet publish -c Release` from `src/backend/Backend`.
- Frontend: `npm run build` to produce packaged Electron artifacts (platform-specific targets are defined in `package.json`).

## Troubleshooting

- **Switching to HTTPS later**: the recommended development setup uses plain HTTP so you can run the app without provisioning certificates. If you prefer HTTPS, trust the development certificate (`dotnet dev-certs https --trust`) and update the backend URL in the Electron settings accordingly.
- **Backend changes not appearing**: confirm you are editing files under `Backend/Problems`. Other directories are ignored by the watcher; restarting the backend will pick up files that were added while it was offline.
- **Fetching AoC input fails**: double-check that your session token is exactly 128 hexadecimal characters (without the `session=` prefix) and that the cookie has not expired.
