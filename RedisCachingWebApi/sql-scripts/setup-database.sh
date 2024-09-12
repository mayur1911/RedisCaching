#!/bin/bash
# Wait for SQL Server to be up
sleep 60s  # Increase the sleep time to give SQL Server more time to initialize

# Check if there are any .sql files in the /var/opt/mssql/scripts directory
if ls /var/opt/mssql/scripts/*.sql > /dev/null 2>&1; then
  # Loop through all SQL files in the directory and execute them
  for f in /var/opt/mssql/scripts/*.sql; do
    echo "Running script: $f"
    /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P YourPassword123! -d master -i "$f"  # Use 'sqlserver' instead of 'localhost'
  done
else
  echo "No SQL files found in /var/opt/mssql/scripts"
fi
