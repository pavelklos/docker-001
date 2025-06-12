# docker-001

- [**DockerCourseFrontend**] [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
  - (Frontend SPA application) Blazor WebAssembly
- [**DockerCourseApi**] [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
  - (Web API) ASP.NET Core Minimal API
- [**Database**] [CreateDatabaseAndSeed.sql](Database/CreateDatabaseAndSeed.sql)
  - (SQL Server)

## Steps

1. [**DockerCourseFrontend**] *create solution*
2. [**DockerCourseApi**] *create solution*
3. **Simple database interaction**
   - *create script* [CreateDatabaseAndSeed.sql](Database/CreateDatabaseAndSeed.sql)
   - [**DockerCourseApi**]
     - *add packages (Dapper, System.Data.SqlClient)*
     - *add database connection and get data from database*
   - docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=dotnet#123" -p 1433:1433 mcr.microsoft.
4. **Building our API image**
   - [**DockerCourseApi**] *add* [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
   - from directory 'DockerCourseApi'
     - docker build -f .\DockerCourseApi\Dockerfile -t api .
       - *create docker image 'api'*
   - docker run -p 1234:8080 api
     - *Now listening on: http://[::]:8080*
   - http://localhost:1234/podcasts
     - *ERROR: System.Data.SqlClient.SqlException*
5. **Dockerfile 101**
   - [**DockerCourseApi**] *update* [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
   - docker build -f .\DockerCourseApi\Dockerfile -t api .
     - *create docker image 'api'*
6. **Building Frontend image**
   - [**DockerCourseFrontend**] *add* [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
7. **Building Frontend image - UPDATE**
   - [**DockerCourseFrontend**] *update* [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
   - from directory 'DockerCourseFrontend\DockerCourseFrontend'
     - docker build -t frontend .
       - *create docker image 'frontend'*
   - docker run -p 1234:80 frontend
     - *nginx/1.27.5*
   - http://localhost:1234
     - *DockerCourseFrontend*

## Docker Images

- **Microsoft SQL Server - Ubuntu based images** <small>for (Database)</small>
  - https://hub.docker.com/r/microsoft/mssql-server
  - docker pull mcr.microsoft.com/mssql/server
  - docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=dotnet#123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
- **.NET SDK** <small>for (DockerCourseApi, DockerCourseFrontend)</small>
  - https://hub.docker.com/r/microsoft/dotnet-sdk
  - docker pull mcr.microsoft.com/dotnet/sdk:9.0
- **ASP.NET Core Runtime** <small>for (DockerCourseApi, DockerCourseFrontend)</small>
  - https://hub.docker.com/r/microsoft/dotnet-aspnet
  - docker pull mcr.microsoft.com/dotnet/aspnet:9.0

## Connecting String to SQL Server

```csharp
"Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
```