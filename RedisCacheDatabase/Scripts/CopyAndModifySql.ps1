# PowerShell script to copy SQL files, add database creation, OBJECT_ID check, and USE statement
param (
    [string]$sourcePath,
    [string]$destinationPath,
    [string]$databaseName = "Employee"  # Default to Employee database, can be changed if needed
)

# Create destination folder if it doesn't exist
if (-not (Test-Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

# Get all SQL files in the source directory
Get-ChildItem $sourcePath -Filter *.sql | ForEach-Object {
    $content = Get-Content $_.FullName -Raw  # Use -Raw to get the full content as a single string

    # Add the logic to create the database if it doesn't exist
    $createDatabaseCheck = @"
-- Create the $databaseName database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName')
BEGIN
    CREATE DATABASE $databaseName;
END
GO
"@

    # Add USE [DatabaseName] statement
    $useDatabase = "USE [$databaseName];"

    # For table scripts (add OBJECT_ID check)
    if ($_.Name -like "*Table*.sql") {
        $tableName = $_.BaseName  # Extract the table name from the file name
        $objectIdCheck = @"
IF OBJECT_ID('$tableName', 'U') IS NOT NULL
BEGIN
    DROP TABLE $tableName;
END
GO
"@
        # Prepend the database creation, USE statement, and OBJECT_ID logic to the file content
        $newContent = $createDatabaseCheck + "`r`n" + $useDatabase + "`r`nGO`r`n" + $objectIdCheck + "`r`n" + $content
    }
    else {
        # For stored procedure scripts (no OBJECT_ID logic)
        $newContent = $createDatabaseCheck + "`r`n" + $useDatabase + "`r`nGO`r`n" + $content
    }

    # Save the modified content to the destination, ensuring correct line endings
    $newFileName = Join-Path $destinationPath $_.Name
    Set-Content -Path $newFileName -Value $newContent -Encoding UTF8
}
