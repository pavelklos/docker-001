# docker-001 <small>(Docker Course steps)</small>

## Structure

[**docker-001**] [docker-compose.yaml](docker-compose.yaml)

- [**Database**]
  - (SQL Server)
  - Docker image: [Microsoft SQL Server](https://hub.docker.com/r/microsoft/mssql-server)
  - Container name: **database**
  - Ports: **1433:1433**
- [**DockerCourseApi**] [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
  - (Web API) ASP.NET Core Minimal API
  - Docker image: [.NET SDK](https://hub.docker.com/r/microsoft/dotnet-sdk) ➤ [ASP.NET Core Runtime](https://hub.docker.com/r/microsoft/dotnet-aspnet)
  - Container name: **api**
  - Ports: **5027:80**
- [**DockerCourseFrontend**] [Dockerfile](DockerCourseFrontend/DockerCourseFrontend/Dockerfile)
  - (Frontend SPA application) Blazor WebAssembly
  - Docker image: [.NET SDK](https://hub.docker.com/r/microsoft/dotnet-sdk) ➤ [nginx](https://hub.docker.com/_/nginx)
  - Container name: **frontend**
  - Ports: **1234:80**
- [**Database**] **database-seed** [Dockerfile](Database/Dockerfile)
  - (SQL Server) ➤ (Mssql Tools) sqlcmd
  - Docker image: [Microsoft SQL Server](https://hub.docker.com/r/microsoft/mssql-server) ➤ [Mssql Tools](https://hub.docker.com/r/microsoft/mssql-tools/)
  - Container name: **database-seed**
  - Seed: [wait-and-run.sh](Database/wait-and-run.sh) ➤ [CreateDatabaseAndSeed.sql](Database/CreateDatabaseAndSeed.sql)

## Build, Run & Stop Docker containers

```batch
# Start build & Run containers in foreground (attached mode)
docker compose up --build

# Open frontend SPA application (Blazor WebAssembly) in browser
http://localhost:1234/

# Open Web API (ASP.NET Core Minimal API) in browser
http://localhost:5027/podcasts
http://localhost:5027/test-connection

# Stop containers (Ctrl+C)
docker compose down
```

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
   - *Docker keywords:* FROM, WORKDIR, COPY, RUN, EXPOSE, ENTRYPOINT, CMD
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
9. **docker compose CLI command**
   - from directory 'docker-001'
     - `docker compose up`
     - `docker compose up -d`
     - `docker compose logs`
     - `docker compose logs api`
     - `docker compose logs frontend`
     - `docker compose logs database`
     - `docker compose down`
     - Ctrl+C
10. **Services as DNS entries**
    - DNS entries in [docker-compose.yaml](docker-compose.yaml)
      - **frontend**, **api**, **database**
    - [**DockerCourseApi**] *update* [Program.cs](DockerCourseApi/DockerCourseApi/Program.cs)
      - *update Connecting String:* `Server=tcp:database`
11. **Using docker compose to build our images**
    - [**docker-001**] *update* [docker-compose.yaml](docker-compose.yaml)
    - from directory 'docker-001'
      - `docker compose build`
      - `docker compose up`
      ---
      - `docker compose up --build`
12. **Seeding our database**
    - [**Database**] *add* [wait-and-run.sh](Database/wait-and-run.sh) 
    - [**Database**] *add* [Dockerfile](Database/Dockerfile) 
    - [**docker-001**] *update* [docker-compose.yaml](docker-compose.yaml)
    - from directory 'docker-001'
      - `docker compose up --build`
      - !!! Database seeding IS NOT complete !!! 
        ```bash
        database-seed  | Not ready yet...
        database-seed  | /wait-and-run.sh: line 19: /opt/mssql-tools/bin/sqlcmd: No such file or directory
        database-seed exited with code 127
        ```
13. **Seeding our database - UPDATE** (resolved uncompleted seeding & docker compose issues)
    - *Resolved by [Claude Sonnet 4](https://claude.ai/) (changes in 3 files)*
      - [**Database**] *update* [wait-and-run.sh](Database/wait-and-run.sh)
      - [**Database**] *update* [Dockerfile](Database/Dockerfile)
      - [**docker-001**] *update* [docker-compose.yaml](docker-compose.yaml)
    - *There are 2 possible solutions*
      - **Solution 1**: change 2 files [**Database**] [wait-and-run.sh](Database/wait-and-run.sh) & [Dockerfile](Database/Dockerfile)
      - **Solution 2**: change 1 file [**docker-001**] [docker-compose.yaml](docker-compose.yaml)
    - *Fix guides by [Claude Sonnet 4](https://claude.ai/)*
      - **SQL Server Docker Seeding - Complete Fix Guide** [sql-server-seeding-fix-guide.md](sql-server-seeding-fix-guide.md)
      - **Docker Compose (.NET API + SQL Server) Fix Guide** [docker-compose-fix-guide.md](docker-compose-fix-guide.md)

## Docker Images

- **Microsoft SQL Server - Ubuntu based images** <small>for (Database)</small>
  - https://hub.docker.com/r/microsoft/mssql-server
  - `docker pull mcr.microsoft.com/mssql/server`
  - `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=dotnet#123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`
- **.NET SDK** <small>for (DockerCourseApi, DockerCourseFrontend)</small>
  - https://hub.docker.com/r/microsoft/dotnet-sdk
  - `docker pull mcr.microsoft.com/dotnet/sdk:9.0`
- **ASP.NET Core Runtime** <small>for (DockerCourseApi)</small>
  - https://hub.docker.com/r/microsoft/dotnet-aspnet
  - `docker pull mcr.microsoft.com/dotnet/aspnet:9.0`
- **nginx** <small>for (DockerCourseFrontend)</small>
  - https://hub.docker.com/_/nginx
  - `docker pull nginx:alpine`
- **Mssql Tools** <small>for database seeding (sqlcmd)</small>
  - https://hub.docker.com/r/microsoft/mssql-tools/
  - `docker pull mcr.microsoft.com/mssql/tools`

## Connecting String to SQL Server

Default:
- Server=tcp:**localhost**
```csharp
"Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
```
Services as DNS entries:
- Server=tcp:**database**
```csharp
"Server=tcp:database;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
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
docker compose up  // Run containers in foreground (attached mode)
docker compose up -d  // Run containers in background (detached mode)

docker builder prune  // Remove build cache
```