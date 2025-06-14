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



# #!/bin/bash

# # Wait for SQL Server to be ready
# echo "Waiting for SQL Server to be ready..."
# for i in {1..50};
# do
#     /opt/mssql-tools/bin/sqlcmd -S database -U sa -P dotnet#123 -Q "SELECT 1" > /dev/null 2>&1
#     if [ $? -eq 0 ]
#     then
#         echo "SQL Server is ready."
#         break
#     else
#         echo "Not ready yet..."
#         sleep 1
#     fi
# done

# # Run the SQL script
# /opt/mssql-tools/bin/sqlcmd -S database -U sa -P dotnet#123 -d master -i /CreateDatabaseAndSeed.sql
