#!/bin/bash
# Wait for SQL Server to be up
sleep 30s

# Run the SQL script to create the Employee database and table
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword123! -d master -i /var/opt/mssql/scripts/init.sql