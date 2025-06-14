# Docker Compose (.NET API + SQL Server) Fix Guide

## Overview
This guide documents the complete troubleshooting and fixes applied to resolve Docker Compose issues with a .NET 9 API, Blazor frontend, and SQL Server 2022 setup.

## Original Problems

1. **SQL Server Connection Error**: `Length specified in network packet payload did not match number of bytes read`
2. **API Connection Failed**: `net::ERR_EMPTY_RESPONSE` when accessing API endpoints
3. **DNS Resolution Issues**: Cannot access `tcp:database` service
4. **Database Seeding Failures**: SQL tools compatibility issues

---

## Fix #1: Updated SQL Client Package

### BEFORE
```xml
<!-- DockerCourseApi/DockerCourseApi.csproj -->
<ItemGroup>
  <PackageReference Include="Dapper" Version="2.1.66" />
  <PackageReference Include="System.Data.SqlClient" Version="4.8.6" />
</ItemGroup>
```

### AFTER
```xml
<!-- DockerCourseApi/DockerCourseApi.csproj -->
<ItemGroup>
  <PackageReference Include="Dapper" Version="2.1.66" />
  <PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" />
</ItemGroup>
```

### WHY WE CHANGED
- `System.Data.SqlClient` is deprecated and has compatibility issues with SQL Server 2022
- `Microsoft.Data.SqlClient` is the modern, actively maintained package with full SQL Server 2022 support
- Resolves the "Length specified in network packet payload" error

---

## Fix #2: Updated Connection String and Using Statement

### BEFORE
```csharp
// DockerCourseApi/DockerCourseApi/Program.cs
using System.Data.SqlClient;

var connectionString = "Server=database;Initial Catalog=podcasts;User ID=sa;Password=dotnet#123;";
using var db = new SqlConnection(connectionString);
```

### AFTER
```csharp
// DockerCourseApi/DockerCourseApi/Program.cs
using Microsoft.Data.SqlClient;
using Dapper;

var connectionString = "Server=tcp:database,1433;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
using var db = new SqlConnection(connectionString);
```

### WHY WE CHANGED
- **Explicit Port**: Added `:1433` for better DNS resolution in Docker networks
- **SSL Configuration**: `Encrypt=False` and `TrustServerCertificate=True` for development environment
- **Modern Using**: Updated to use `Microsoft.Data.SqlClient` namespace
- **Connection Reliability**: Added timeout and security settings for container networking

---

## Fix #3: Docker Compose Port Mapping

### BEFORE
```yaml
# docker-compose.yaml
api:
  build:
    context: ./DockerCourseApi
    dockerfile: DockerCourseApi/Dockerfile
  image: api
  container_name: api
  ports:
    - 5027:80  # WRONG PORT MAPPING
```

### AFTER
```yaml
# docker-compose.yaml
api:
  build:
    context: ./DockerCourseApi
    dockerfile: DockerCourseApi/Dockerfile
  image: api
  container_name: api
  ports:
    - 5027:8080  # CORRECT PORT MAPPING
```

### WHY WE CHANGED
- **.NET 9 Default**: ASP.NET Core 9.0 applications default to port 8080 in containers
- **Port Mismatch**: The original mapping `5027:80` was trying to map to port 80, but the app was listening on 8080
- **Connection Failure**: This mismatch caused `net::ERR_EMPTY_RESPONSE` errors

---

## Fix #4: Database Seeding Dockerfile

### BEFORE
```dockerfile
# Database/Dockerfile
FROM mcr.microsoft.com/mssql/server:2022-latest
# ... existing code ...
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg
# ... basic mssql-tools installation
```

### AFTER
```dockerfile
# Database/Dockerfile
FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

# Install mssql-tools18 (latest version compatible with SQL Server 2022)
RUN apt-get update && \
    apt-get install -y curl apt-transport-https gnupg lsb-release && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Add SQL Server tools to PATH
ENV PATH="$PATH:/opt/mssql-tools18/bin"

COPY init-db.sql /usr/src/app/
COPY init-db.sh /usr/src/app/

RUN chmod +x /usr/src/app/init-db.sh

USER mssql

CMD /usr/src/app/init-db.sh
```

### WHY WE CHANGED
- **Version Compatibility**: `mssql-tools18` is compatible with SQL Server 2022
- **Proper Installation**: Added Microsoft package repository for official tools
- **Environment Setup**: Added tools to PATH and proper permissions
- **Clean Installation**: Added cleanup to reduce image size

---

## Alternative Solutions

### Alternative 1: Configure ASP.NET Core for Port 80

If you prefer to keep port 80 in docker-compose, modify the Dockerfile instead:

```dockerfile
# DockerCourseApi/DockerCourseApi/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80  # Force port 80
# ... rest of dockerfile
```

### Alternative 2: Use Environment Variables for Connection

Make the connection string configurable:

```csharp
// DockerCourseApi/DockerCourseApi/Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=tcp:database,1433;Initial Catalog=podcasts;User ID=sa;Password=dotnet#123;Encrypt=False;TrustServerCertificate=True;";
```

```yaml
# docker-compose.yaml
api:
  environment:
    - ConnectionStrings__DefaultConnection=Server=tcp:database,1433;Initial Catalog=podcasts;User ID=sa;Password=dotnet#123;Encrypt=False;TrustServerCertificate=True;
```

### Alternative 3: Health Checks

Add health checks to ensure services are ready:

```yaml
# docker-compose.yaml
database:
  image: mcr.microsoft.com/mssql/server:2022-latest
  healthcheck:
    test: ["CMD-SHELL", "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P dotnet#123 -Q 'SELECT 1' || exit 1"]
    interval: 30s
    timeout: 10s
    retries: 5

api:
  depends_on:
    database:
      condition: service_healthy
```

### Alternative 4: Development vs Production Configurations

Use different docker-compose files:

```yaml
# docker-compose.dev.yaml
version: '3.8'
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
    volumes:
      - ./DockerCourseApi:/app
    command: dotnet watch run
```

```bash
# Run development environment
docker compose -f docker-compose.yaml -f docker-compose.dev.yaml up
```

---

## Final Working Solution Summary

✅ **SQL Server 2022 Compatibility**: Modern `Microsoft.Data.SqlClient` package  
✅ **Correct Port Mapping**: ASP.NET Core 9.0 default port 8080  
✅ **SSL Configuration**: Disabled encryption for development  
✅ **Database Tools**: Latest `mssql-tools18` for seeding  
✅ **Container Networking**: Proper DNS resolution with explicit ports  
✅ **Connection Reliability**: Comprehensive connection string with timeouts  

The application now runs successfully with:
- Frontend: http://localhost:1234
- API: http://localhost:5027
- Database: SQL Server 2022 with proper seeding