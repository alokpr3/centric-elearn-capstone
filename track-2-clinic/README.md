# Clinic Appointment Booking

A small app to practise Claude Code. Read PROBLEM.md first.

## How to run

First run `dotnet build` in the backend folder. It will fail. That is expected. It is part of Step 1 in PROBLEM.md.

1. Terminal 1 (backend):

   ```
   cd backend
   dotnet run --project Clinic.Api
   ```

   The API runs at http://localhost:5080.

2. Terminal 2 (frontend):

   ```
   cd frontend
   npm install && npm run dev
   ```

3. Open http://localhost:5173 in your browser.

## Run the tests

```
cd backend
dotnet test
```

## Folders

- `backend/Clinic.Api` - the .NET 8 API (data is kept in memory, it resets on restart)
- `backend/Clinic.Tests` - xUnit tests
- `frontend` - React + Vite app
