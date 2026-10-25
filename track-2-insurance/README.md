# Harbourline Insurance - Claims Platform

Harbourline is a mid-sized UK home and motor insurer. Its claims team of about a dozen handlers uses this internal app to register claims, move them through assessment and settlement, keep notes, import monthly bordereaux from delegated-authority partners and watch the loss ratio by product. It was built quickly three years ago, nobody owns it any more, and the Head of Claims has just asked for a proper claims workflow on top of it.

## Run it

Needs: .NET 8 SDK, Node 20 and Docker (or any MySQL 8 server on port 3306 that you can reach as root).

```bash
docker run -d --name harbourline-mysql -e MYSQL_ROOT_PASSWORD='Root!Pass1' -p 3306:3306 mysql:8.4
docker exec -i harbourline-mysql mysql -uroot -p'Root!Pass1' < db-setup/01-create-users.sql
cd backend && dotnet run --project Harbourline.Api
cd backend && dotnet test
cd frontend && npm install && npm run dev
```

Wait about 30 seconds after the first command for MySQL to start. The API and the web app each keep running, so give them their own terminals.

API on http://localhost:5102 (Swagger at /swagger), UI on http://localhost:5173.

MySQL on another host or port: set the environment variable `ConnectionStrings__Harbourline` to a full connection string (server, port, database, user, password) before starting the API. The existing tests use an in-memory database and do not need MySQL.
