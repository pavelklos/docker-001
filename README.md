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
14. **Pushing images to Docker Hub**
    - [Docker Hub (Repositories)](https://hub.docker.com/repositories/)
      - [Create repository] **dotnet-001-api**
      - [Create repository] **dotnet-001-frontend**
    - from directory 'docker-001'
      - `docker login`
    - from directory 'DockerCourseApi'
      - `docker build -f .\DockerCourseApi\Dockerfile -t pavelklos/dotnet-001-api .`
      - `docker push pavelklos/dotnet-001-api`
    - from directory 'DockerCourseFrontend' 
      - `docker build -f .\DockerCourseFrontend\Dockerfile -t pavelklos/dotnet-001-frontend .`
      - `docker push pavelklos/dotnet-001-frontend`
    - [**docker-001**] *update* [docker-compose.yaml](docker-compose.yaml)
      - *update image: for **frontend** & **api***
        - `image: pavelklos/dotnet-001-frontend`
        - `image: pavelklos/dotnet-001-api`
    - from directory 'docker-001'
      - `docker compose build`
      - `docker compose push`
15. **Docker and CI/CD**
    - Docker docs:
      - [Continuous integration with Docker](https://docs.docker.com/build/ci/)
      - [Docker Build GitHub Actions](https://docs.docker.com/build/ci/github-actions/)
    - from directory 'docker-001'
      - *add folder* [.github/workflows/](.github/workflows/)
        - *add files* [build-api.yml](.github/workflows/build-api.yml), [build-frontend.yml](.github/workflows/build-frontend.yml) *to the folder*
    - [GitHub Actions](https://github.com/pavelklos/docker-001/actions/)
    - **docker buildx** is extended version of **docker build** command, providing enhanced features for building Docker images using **BuildKit** backend
      - `docker buildx`
16. **Leveraging Docker for Testing** ⚠️TODO: **TRY TESTS & GITHUB ACTIONS BY CI/CD**⚠️
    - [**DockerCourseApi**] *add test project* [**DockerCourseApi.Tests**]
    - [**DockerCourseApi**] *update* [Dockerfile](DockerCourseApi/DockerCourseApi/Dockerfile)
    - [**.github/workflows/**] *update* [build-api.yml](.github/workflows/build-api.yml)
    - from directory 'docker-001'
      - `docker compose up database`
      - `docker compose up database-seed`
    - from directory 'DockerCourseApi'
      - `docker build -f .\DockerCourseApi\Dockerfile --network host .`
17. **Spikes**
    - **Redis (Remote Dictionary Server)** [redis](https://hub.docker.com/_/redis)
      - `docker run -p 6379:6379 redis`
    - [**LINQPad**](https://www.linqpad.net/) (.NET Programmer’s Playground)
      - *popular tool for .NET developers to interactively query databases, test code snippets, and explore data using LINQ*
      - *execute query* [Spikes.linq](LINQPad/Spikes.linq)
18. **Introduction and types of persistent storage in Docker**
    - **Host**
      - Host file system (**bind mount**) ./Some/Directory
      - Host memory (**tmpfs**)
    - **Docker**
      - **Container** (SQL Server), Container, Container, Container
      - **volume**, volume
    ---
    - **Persistent storage** is essential for maintaining data integrity and availability in containerized applications.
    - Docker provides several options for persistent storage, including:
      - **Volumes**: Managed by Docker, suitable for sharing data between containers.
      - **Bind mounts**: Maps host directory to container directory, allowing direct access to host files.
      - **tmpfs mounts**: Stores data in memory, useful for temporary data that doesn't need to persist after container stops.
    - Docker **Volumes** can be used to persist data generated by and used by Docker containers.
19. **Creating Volumes**
    - Docker [Volumes](https://docs.docker.com/engine/storage/volumes/)
    - `docker volume create dotnet-001-podcasts`
    - `docker volume ls`
    - `docker volume inspect dotnet-001-podcasts`
    - `docker volume rm dotnet-001-podcasts`
    - `docker volume prune`
20. **Mounting volumes in containers**
    - from directory 'docker-001'
      - *add folder* [Volumes](Volumes/)
        - *add files* [SqlServerBindMountDemo.ps1](Volumes/SqlServerBindMountDemo.ps1), [SqlServerVolumeDemo.ps1](Volumes/SqlServerVolumeDemo.ps1) *to the folder*
    - [SqlServerVolumeDemo.ps1](Volumes/SqlServerVolumeDemo.ps1)
      - `-v sqldb-data:/var/opt/mssql` *-v <volume_name>:<container_path>*
    - from directory 'Volumes' ⚠️TODO: **TRY SCRIPT EXECUTION**⚠️
      - `Unblock-File -Path .\SqlServerVolumeDemo.ps1` *Unblock script execution*
      - `.\SqlServerVolumeDemo.ps1`
21. **Mounting bind mounts in containers**
    - [SqlServerBindMountDemo.ps1](Volumes/SqlServerBindMountDemo.ps1)
      - `-v ${pwd}/html:/usr/share/nginx/html` *-v <volume_path>:<container_path>*
    - from directory 'Volumes' ⚠️TODO: **TRY SCRIPT EXECUTION**⚠️
      - `Unblock-File -Path .\SqlServerBindMountDemo.ps1` *Unblock script execution*
      - `.\SqlServerBindMountDemo.ps1`
    - from directory 'docker-001/Volumes'
      - *add folder* [html](Volumes/html/)
        - *add file* [index.html](Volumes/html/index.html) *to the folder*
    - http://localhost:1234
22. **Volumes in Docker compose**
    - *update* [docker-compose.yaml](docker-compose.yaml) *add volumes:*
      ```
      volumes:
        sqldb-data:
      ```
      ```
      volumes:
      - sqldb-data:/var/opt/mssql
      ```
    - from directory 'docker-001'
      - `docker compose up`
      - `docker compose down`
      - `docker compose up database`  *seeded data are still available by volume*
23. **Backing up volumes**
    - from directory 'docker-001/Volumes'
      - *add file* [VolumeBackup.ps1](Volumes/VolumeBackup.ps1)
    - from directory 'Volumes' ⚠️TODO: **TRY SCRIPT EXECUTION**⚠️
      - `Unblock-File -Path .\VolumeBackup.ps1` *Unblock script execution*
      - `.\VolumeBackup.ps1`
24. **Backing up volumes (sqldb-data)**
    - from directory 'docker-001/Volumes'
      - *add file* [VolumeBackup-sqldb-data.ps1](Volumes/VolumeBackup-sqldb-data.ps1)
    - from directory 'Volumes' ⚠️TODO: **TRY SCRIPT EXECUTION**⚠️
      ```bash
      # Backup the volume
      .\VolumeBackup-sqldb-data.ps1 -Action backup
      # Restore the volume
      .\VolumeBackup-sqldb-data.ps1 -Action restore
      # Backup to specific location
      .\VolumeBackup-sqldb-data.ps1 -Action backup -BackupPath "./my-backups"
      ```
25. **Anonymous volumes and Dockerfile VOLUME instruction**
    - [rabbitmq (Dockerfile)](https://github.com/docker-library/rabbitmq/blob/master/4.1/alpine/Dockerfile)
      - `VOLUME $RABBITMQ_DATA_DIR`
    ```bash
    docker run -v /mydata alpine  # Create anonymous volume
    docker run --name rabbitmq rabbitmq  # Create anonymous volume
    docker volume ls
    ```
26. **Default bridge network**
    - `docker network ls`
    ```bash
    NETWORK ID     NAME                 DRIVER    SCOPE
    8d5c00d59d03   bridge               bridge    local
    fb6c7168c99c   host                 host      local
    a925db299a5c   none                 null      local
    ```
    - *Containers can communicate with each other by IP address*
    ```bash
    docker network inspect bridge  # "Subnet": "172.17.0.0/16", "Gateway": "172.17.0.1"
    docker run -d --name container-a nginx  # create container-a
    docker run -d --name container-b nginx  # create container-b
    docker network inspect bridge
      # "Containers": {...} "Name": "container-a", "IPv4Address": "172.17.0.2/16"
      # "Containers": {...} "Name": "container-b", "IPv4Address": "172.17.0.3/16"
    docker inspect container-a  # "Networks": {...} "bridge": {...} "IPAddress": "172.17.0.2"
    docker inspect container-b  # "Networks": {...} "bridge": {...} "IPAddress": "172.17.0.3"
    docker run -it --rm --name container-c alpine  # get shell
      > ping 172.17.0.2  # container-c communicates with container-a by IP address
    ```
27. **Custom bridge networks**
    - **VS Code (Containers-Networks)**
    ---
    - `docker network create network-a`
    - `docker network create network-b`
    - `docker network ls`
    ```bash
    8d5c00d59d03   bridge               bridge    local
    4fea74ee4ed7   network-a            bridge    local
    2e93bd66f197   network-b            bridge    local
    fb6c7168c99c   host                 host      local
    a925db299a5c   none                 null      local
    ```
    ```bash
    docker network inspect bridge  # "Subnet": "172.17.0.0/16"
    docker network inspect network-a  # "Subnet": "172.19.0.0/16"
    docker network inspect network-b  # "Subnet": "172.20.0.0/16"
    ```
    ```bash
    docker run -it --rm --name container-a1 --network network-a alpine  # get shell
    docker run -it --rm --name container-a2 --network network-a alpine  # get shell
    docker run -it --rm --name container-b1 --network network-b alpine  # get shell
    docker run -it --rm --name container-b2 --network network-b --hostname dotnet alpine  # get shell
    docker ps  # 4x CONTAINER ID
    ```
    ```bash
    # from shell
      > ping CONTAINER ID  # by CONTAINER ID (in the same network)  
      > ping dotnet  # by hostname (in the same network)
    ```
    ```bash
    docker network connect network-a container-b1

    # from shell
      > ping dotnet  # by hostname (in other network)
    
    docker network disconnect network-a container-b1
    ```



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
Saved connection string by: 
  - [**DockerCourseApi**] [appsettings.json](DockerCourseApi/DockerCourseApi/appsettings.json) 
  - [**DockerCourseApi**] [Settings.cs](DockerCourseApi/DockerCourseApi/Settings.cs)

Default:
- Server=tcp:**localhost**
```csharp
"Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
```
Services as DNS entries:
- Server=tcp:**database**
```csharp
"Server=tcp:database;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"

// updated (port 1433, Encrypt=False)
"Server=tcp:database,1433;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;"
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
docker compose down  // Stop and remove containers, networks, volumes, and images

docker builder prune  // Remove build cache

docker volume  // Manage volumes

docker network  // Manage networks
```