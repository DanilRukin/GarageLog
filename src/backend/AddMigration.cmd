@echo off
dotnet ef migrations add %1 ^
  --project GarageLog.Infrastructure ^
  --startup-project GarageLog.Api ^
  --output-dir Persistence/Migrations

pause
