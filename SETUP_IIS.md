# IIS Setup (Local)

## Prerequisites
- IIS installed (Windows Features)
- ASP.NET Core Hosting Bundle installed for .NET 10
- SQL Server available locally/network

## Backend (ASP.NET Core API)

1. Publish API:
   - `dotnet publish Backend/Backend/Backend.csproj -c Release -o publish/api`
2. In IIS:
   - Create a site or app pointing to `publish/api`
   - App Pool: `No Managed Code`
3. Configure connection string:
   - Edit `appsettings.json` inside the publish folder (or set environment variable `Db__ConnectionString`)

## Frontend (Static)

1. Build:
   - `cd Frontend`
   - `npm run build`
2. In IIS:
   - Create a separate site pointing to `Frontend/dist`
3. API base:
   - If hosting frontend and backend on different origins, ensure backend CORS is allowed (it is currently open for local use).

## Optional: Same Site (Reverse Proxy)

If you want `/api` to route to the backend while serving the frontend from the same site, use IIS URL Rewrite + ARR (reverse proxy).

