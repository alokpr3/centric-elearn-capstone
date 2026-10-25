# Northgate Freight

Northgate Freight is a haulage and parcel carrier with 18 depots across Great Britain. Its operations team books consignments for around forty account customers, assigns them to depot drivers and watches a daily dispatch on-time report. The booking and depot system was written a couple of years ago by a contractor who has since moved on. You are the new engineer on the team, and Meredith Okafor, the Operations Director, is your stakeholder. Her request is in `docs/stakeholder-brief.md`.

## Run it

Needs: .NET 8 SDK, Node 20, and a MySQL 8 server you can reach as root.

```
mysql -h 127.0.0.1 -P 3306 -u root -p < db-setup/01-create-users.sql
cd backend && dotnet run --project Northgate.Api
cd frontend && npm install && npm run dev
```

The API listens on http://localhost:5080 (Swagger at `/swagger`) and the web app on http://localhost:5173.

Tests: `cd backend && dotnet test` (the MySQL server must be running; the tests use the same database).

MySQL on another host or port: set the environment variable `NORTHGATE_DB` to a full connection string (server, port, database, user, password) before starting the API or the tests. The tests need MySQL.
