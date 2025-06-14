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



# FROM mcr.microsoft.com/mssql/server:2022-latest

# COPY ./wait-and-run.sh /wait-and-run.sh
# COPY ./CreateDatabaseAndSeed.sql /CreateDatabaseAndSeed.sql

# CMD /wait-and-run.sh