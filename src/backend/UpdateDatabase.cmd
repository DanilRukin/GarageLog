@echo off
dotnet ef database update %1 ^
  --project GarageLog.Infrastructure ^
  --startup-project GarageLog.Api
pause