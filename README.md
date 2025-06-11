# docker-001

- [**DockerCourseFrontend**]
  - (Frontend SPA application) Blazor WebAssembly
- [**DockerCourseApi**]
  - (Web API) ASP.NET Core Minimal API
- [**Database**]
  - (SQL Server)

## Steps

1. [**DockerCourseFrontend**] *create solution*
2. [**DockerCourseApi**] *create solution*
3. **Simple database interaction**
   - *create script* [CreateDatabaseAndSeed.sql](Database/CreateDatabaseAndSeed.sql)
   - [**DockerCourseApi**]
     - *add packages (Dapper, System.Data.SqlClient)*
     - *add database connection and get data from database*

## Docker Images

- [Microsoft SQL Server - Ubuntu based images]
  - https://hub.docker.com/r/microsoft/mssql-server
  - docker pull mcr.microsoft.com/mssql/server
  - docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=dotnet#123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

## Connecting String to SQL Server

```csharp
"Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
```