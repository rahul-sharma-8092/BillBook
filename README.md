# BillBook (Cash Memo System)

Local cash memo billing system with:
- Backend: ASP.NET Core Web API (.NET 10) + Dapper + SQL Server + PuppeteerSharp PDF
- Frontend: React + TypeScript + Tailwind + Recharts

## 1) Database (SQL Server)

The database project is in `Backend/SqlDB`.

1. Create a database (example): `BillBook`
2. Publish `Backend/SqlDB/SqlDB.sqlproj` to your SQL Server (SSDT publish), or run the generated script.

This repo adds stored procedures and a TVP type under `Backend/SqlDB/dbo` used by the API.

## 2) Backend (API)

Config:
- `Backend/Backend/appsettings.json` -> `Db:ConnectionString`

Run:
- `dotnet run --project Backend/Backend/Backend.csproj`

Swagger:
- `http://localhost:5064/swagger`

PDF:
- `GET /api/invoices/{id}/pdf` (first call downloads a Chromium build via PuppeteerSharp)

## 3) Frontend (Web)

Dev:
- `cd Frontend`
- `npm install`
- `npm run dev`

Vite dev proxy is configured to forward `/api` to `http://localhost:5064`.

Build:
- `npm run build` (outputs `Frontend/dist`)

## 4) IIS (Local)

See `SETUP_IIS.md`.
