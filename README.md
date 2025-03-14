# Photo Studio Manager

System do zarządzania studiem fotograficznym i sesjami zdjęciowymi.

## Technologie
- Backend:
  - .NET 8
  - ASP.NET Core Web API
  - Entity Framework Core
  - SQL Server
  - Clean Architecture
  - JWT Authentication
- Frontend:
  - React
  - TypeScript
  - Material-UI
  - React Query
  - React Router

## Funkcjonalności
- Zarządzanie sesjami fotograficznymi
- Zarządzanie klientami
- System rezerwacji terminów
- Zarządzanie portfolio
- Panel administracyjny
- System powiadomień

## Struktura Projektu
- `PhotoStudioManager.API` - Warstwa API
- `PhotoStudioManager.Core` - Warstwa domeny (encje, interfejsy)
- `PhotoStudioManager.Application` - Warstwa aplikacji (serwisy, DTOs)
- `PhotoStudioManager.Infrastructure` - Warstwa infrastruktury (baza danych, zewnętrzne serwisy)

## Jak uruchomić
1. Sklonuj repozytorium
2. Zainstaluj .NET 8 SDK
3. Zainstaluj SQL Server
4. Uruchom migracje bazy danych
5. Uruchom backend: `dotnet run --project PhotoStudioManager.API`
6. Uruchom frontend (instrukcje w katalogu ClientApp)

## Development
- Backend API URL: `https://localhost:7001`
- Frontend URL: `http://localhost:3000`
