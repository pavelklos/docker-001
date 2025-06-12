# docker-001

[**docker-001**] [docker-compose.yaml](docker-compose.yaml)
- [**Database**] [CreateDatabaseAndSeed.sql](Database/CreateDatabaseAndSeed.sql)
  - (SQL Server)
- [**DockerCourseApi**] [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
  - (Web API) ASP.NET Core Minimal API
- [**DockerCourseFrontend**] [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
  - (Frontend SPA application) Blazor WebAssembly

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
   - docker run -p 1234:80 api
     - *Now listening on: http://[::]:8080 ?(http://[::]:80)*
   - http://localhost:1234/podcasts
     - *ERROR: System.Data.SqlClient.SqlException*
5. **Dockerfile 101**
   - [**DockerCourseApi**] *update* [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
   - from directory 'DockerCourseApi'
     - docker build -f .\DockerCourseApi\Dockerfile -t api .
       - *create docker image 'api'*
   - *Docker keywords:* FROM, WORKDIR, COPY, RUN, EXPOSE, ENTRYPOINT
6. **Building Frontend image**
   - [**DockerCourseFrontend**] *add* [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
7. **Building Frontend image - UPDATE**
   - [**DockerCourseFrontend**] *update* [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
   - from directory 'DockerCourseFrontend'
     - docker build -f DockerCourseFrontend/Dockerfile -t frontend .
       - *create docker image 'frontend'*
   - docker run -p 1234:80 frontend
     - *nginx/1.27.5*
   - http://localhost:1234
     - *DockerCourseFrontend*
8. **docker-compose YAML file**
   - [**docker-001**] *add* [docker-compose.yaml](docker-compose.yaml)




## Docker Images

- **Microsoft SQL Server - Ubuntu based images** <small>for (Database)</small>
  - https://hub.docker.com/r/microsoft/mssql-server
  - docker pull mcr.microsoft.com/mssql/server
  - docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=dotnet#123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
- **.NET SDK** <small>for (DockerCourseApi, DockerCourseFrontend)</small>
  - https://hub.docker.com/r/microsoft/dotnet-sdk
  - docker pull mcr.microsoft.com/dotnet/sdk:9.0
- **ASP.NET Core Runtime** <small>for (DockerCourseApi)</small>
  - https://hub.docker.com/r/microsoft/dotnet-aspnet
  - docker pull mcr.microsoft.com/dotnet/aspnet:9.0
- **nginx** <small>for (DockerCourseFrontend)</small>
  - https://hub.docker.com/_/nginx
  - docker pull nginx:alpine

## Connecting String to SQL Server

```csharp
"Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
```

## Docker commands
- [CLI reference](https://docs.docker.com/reference/cli/docker/)

```csharp
docker ps  // List containers
docker run  // Create and run new container from image
docker stop  // Stop one or more running containers
docker rm  // Remove one or more containers
docker logs  // Fetch the logs of a container
docker attach  // Attach local standard input, output, and error streams to running container
docker exec  // Execute command in running container
docker commit  // Create new image from container's changes
docker pull  // Download image from registry
docker push  // Upload image to registry
docker images  // List images
docker rmi  // Remove one or more images
docker tag  // Create tag TARGET_IMAGE that refers to SOURCE_IMAGE
docker inspect  // Return low-level information on Docker objects

docker build  // Start build
docker compose  // Define and run multi-container applications with Docker

docker builder prune  // Remove build cache
```