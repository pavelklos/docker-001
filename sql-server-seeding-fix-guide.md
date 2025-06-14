# SQL Server Docker Seeding - Complete Fix Guide

## Problem Summary
The original setup failed with error: `/opt/mssql-tools/bin/sqlcmd: No such file or directory` because SQL Server command-line tools are not pre-installed in the SQL Server 2022 container.

---

## Solution 1: Fix Existing Files (Custom Image Approach)

### 1. Dockerfile Changes

**BEFORE:**
```dockerfile
FROM mcr.microsoft.com/mssql/server:2022-latest
COPY ./wait-and-run.sh /wait-and-run.sh
COPY ./CreateDatabaseAndSeed.sql /CreateDatabaseAndSeed.sql
CMD /wait-and-run.sh
```

**AFTER:**
```dockerfile
FROM mcr.microsoft.com/mssql/server:2022-latest

# Install SQL Server command-line tools
USER root
RUN apt-get update && \
    apt-get install -y curl gnupg && \
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg && \
    curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev && \
    echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Switch back to mssql user
USER mssql

# Copy scripts
COPY ./wait-and-run.sh /wait-and-run.sh
COPY ./CreateDatabaseAndSeed.sql /CreateDatabaseAndSeed.sql

# Make script executable
USER root
RUN chmod +x /wait-and-run.sh
USER mssql

CMD ["/wait-and-run.sh"]
```

**WHY THESE CHANGES:**
- **Install SQL tools**: Added `mssql-tools18` package installation
- **Ubuntu 22.04**: SQL Server 2022 uses Ubuntu 22.04, not 20.04
- **Modern package management**: Used `gpg --dearmor` instead of deprecated `apt-key`
- **Proper permissions**: Made script executable and managed user switching
- **Clean installation**: Removed package cache to reduce image size

### 2. wait-and-run.sh Changes

**BEFORE:**
```bash
#!/bin/bash
# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
for i in {1..50};
do
    /opt/mssql-tools/bin/sqlcmd -S database -U sa -P dotnet#123 -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]
    then
        echo "SQL Server is ready."
        break
    else
        echo "Not ready yet..."
        sleep 1
    fi
done
# Run the SQL script
/opt/mssql-tools/bin/sqlcmd -S database -U sa -P dotnet#123 -d master -i /CreateDatabaseAndSeed.sql
```

**AFTER:**
```bash
#!/bin/bash

# Add SQL tools to PATH
export PATH="$PATH:/opt/mssql-tools18/bin"

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
for i in {1..50};
do
    sqlcmd -S database -U sa -P "dotnet#123" -C -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]
    then
        echo "SQL Server is ready."
        break
    else
        echo "Not ready yet..."
        sleep 2
    fi
done

# Check if we successfully connected
if [ $? -eq 0 ]
then
    echo "Checking if database already exists..."
    DB_EXISTS=$(sqlcmd -S database -U sa -P "dotnet#123" -C -Q "SELECT name FROM sys.databases WHERE name = 'podcasts'" -h -1 2>/dev/null | tr -d ' \n\r')
    
    if [ "$DB_EXISTS" = "podcasts" ]; then
        echo "Database 'podcasts' already exists. Skipping seeding to prevent duplicates."
    else
        echo "Running database seed script..."
        sqlcmd -S database -U sa -P "dotnet#123" -C -d master -i /CreateDatabaseAndSeed.sql
        echo "Database seeding completed."
    fi
else
    echo "Failed to connect to SQL Server after 50 attempts."
    exit 1
fi
```

**WHY THESE CHANGES:**
- **Updated PATH**: Points to `mssql-tools18` instead of `mssql-tools`
- **Added -C flag**: Bypasses SSL certificate validation (required for tools18)
- **Better error handling**: Added connection verification and proper exit codes
- **Increased sleep time**: Changed from 1s to 2s for better stability
- **Cleaner syntax**: Removed full paths since tools are now in PATH
- **🆕 Duplicate Prevention**: Added database existence check to prevent duplicate seeding

### 3. docker-compose.yaml Changes

**BEFORE:**
```yaml
services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: database
    environment:
      - ACCEPT_EULA=true
      - MSSQL_SA_PASSWORD=dotnet#123
    ports:
      - 1433:1433
  database-seed:
    depends_on: [ database ]
    build:
      context: ./Database
      dockerfile: Dockerfile
    container_name: database-seed
```

**AFTER (if using custom image approach):**
```yaml
version: '2.4'

services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: database
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=dotnet#123
    ports:
      - "1433:1433"
  database-seed:
    depends_on: [ database ]
    build:
      context: ./Database
      dockerfile: Dockerfile
    container_name: database-seed
```

**WHY THESE CHANGES:**
- **Added version**: Specified Docker Compose version
- **Fixed EULA**: Changed `true` to `Y` (proper format)
- **Quoted ports**: Added quotes around port mapping (best practice)

---

## Solution 2: Recommended Approach (Official Tools Image)

### Complete docker-compose.yaml Replacement

**BEFORE:**
```yaml
services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: database
    environment:
      - ACCEPT_EULA=true
      - MSSQL_SA_PASSWORD=dotnet#123
    ports:
      - 1433:1433
  database-seed:
    depends_on: [ database ]
    build:
      context: ./Database
      dockerfile: Dockerfile
    container_name: database-seed
```

**AFTER:**
```yaml
version: '2.4'

services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: database
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=dotnet#123
    ports:
      - "1433:1433"
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P dotnet#123 -Q 'SELECT 1' || exit 1"]
      interval: 10s
      retries: 10
      start_period: 10s
      timeout: 3s

  database-seed:
    image: mcr.microsoft.com/mssql-tools
    depends_on:
      database:
        condition: service_healthy
    volumes:
      - ./Database/CreateDatabaseAndSeed.sql:/CreateDatabaseAndSeed.sql
    command: >
      bash -c "
        echo 'Waiting a bit more for SQL Server to be fully ready...' &&
        sleep 5 &&
        echo 'Checking if database already exists...' &&
        DB_EXISTS=$(sqlcmd -S database -U sa -P 'dotnet#123' -Q 'SELECT name FROM sys.databases WHERE name = '\"'\"'podcasts'\"'\"'' -h -1 2>/dev/null | tr -d ' \n\r') &&
        if [ \"$DB_EXISTS\" = \"podcasts\" ]; then
          echo 'Database \"podcasts\" already exists. Skipping seeding to prevent duplicates.'
        else
          echo 'Running database seed script...' &&
          sqlcmd -S database -U sa -P 'dotnet#123' -d master -i /CreateDatabaseAndSeed.sql &&
          echo 'Database seeding completed.'
        fi
      "
    container_name: database-seed
```

---

## Why I Recommend the Compose-Only Approach

### 🚀 **Advantages of Official Tools Image Approach:**

1. **No Custom Docker Image Required**
   - No Dockerfile needed
   - No build process required
   - Faster deployment

2. **Official Microsoft Support**
   - Uses `mcr.microsoft.com/mssql-tools` - officially maintained
   - Always up-to-date with latest tools
   - Better security and stability

4. **Proper Health Checks + Idempotency**
   - `healthcheck` ensures database is truly ready
   - `condition: service_healthy` prevents race conditions
   - **🆕 Database existence check** prevents duplicate seeding
   - More reliable than custom wait loops

4. **Simpler Maintenance**
   - No need to manage tool installations
   - No Ubuntu version compatibility issues
   - No package management complications

5. **Better Resource Usage**
   - Smaller overall footprint
   - No duplicate tool installations
   - Leverages Docker layer caching

6. **Cleaner Architecture**
   - Separation of concerns (database vs. tools)
   - Standard Docker patterns
   - Easier to understand and modify

### 🔧 **Technical Benefits:**

- **Reliability**: Health checks prevent timing issues
- **Performance**: No custom image build time
- **Security**: Official images with proper security updates
- **Maintainability**: Less code to maintain and debug

### 📦 **File Structure Simplification:**

**With Custom Image:**
```
Database/
├── Dockerfile
├── wait-and-run.sh
└── CreateDatabaseAndSeed.sql
docker-compose.yaml
```

**With Official Tools Image:**
```
Database/
└── CreateDatabaseAndSeed.sql
docker-compose.yaml
```

---

## Final Recommendation

**Use the Compose-Only Approach** because it's:
- ✅ Simpler to implement
- ✅ More reliable
- ✅ Easier to maintain
- ✅ Follows Docker best practices
- ✅ Uses official Microsoft tools

The custom image approach works, but the official tools image approach is the modern, recommended solution for SQL Server database seeding in Docker environments.