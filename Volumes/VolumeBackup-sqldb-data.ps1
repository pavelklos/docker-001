# BackupRestore.ps1
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("backup", "restore")]
    [string]$Action,
    
    [string]$BackupPath = "./sqldb-backup"
)

switch ($Action) {
    "backup" {
        Write-Host "Creating backup of docker-001_sqldb-data volume..."
        
        # Ensure backup directory exists
        New-Item -ItemType Directory -Force -Path $BackupPath
        
        # Create tar backup
        docker run --rm `
            -v docker-001_sqldb-data:/data `
            -v "${pwd}/${BackupPath}:/backup" `
            alpine tar czf /backup/sqldb-data.tar.gz -C /data .
            
        Write-Host "Backup completed: ${BackupPath}/sqldb-data.tar.gz"
    }
    
    "restore" {
        Write-Host "Restoring docker-001_sqldb-data volume from backup..."
        
        # Stop any running containers using the volume
        docker compose -f docker-compose.yaml down
        
        # Remove existing volume
        docker volume rm docker-001_sqldb-data -f
        
        # Create new volume
        docker volume create docker-001_sqldb-data
        
        # Restore from backup
        docker run --rm `
            -v docker-001_sqldb-data:/data `
            -v "${pwd}/${BackupPath}:/backup" `
            alpine tar xzf /backup/sqldb-data.tar.gz -C /data
            
        Write-Host "Restore completed. You can now run 'docker compose up' to start your services."
    }
}