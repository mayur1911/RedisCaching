# PowerShell script to copy SQL files and add OBJECT_ID check
param (
    [string]$sourcePath,
    [string]$destinationPath
)

# Create destination folder if it doesn't exist
if (-not (Test-Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

# Get all SQL files in the source directory
Get-ChildItem $sourcePath -Filter *.sql | ForEach-Object {
    $content = Get-Content $_.FullName -Raw  # Use -Raw to get the full content as a single string

    # Add OBJECT_ID logic at the beginning of the file
    $tableName = $_.BaseName  # Extract the table name from the file name
    $objectIdCheck = @"
IF OBJECT_ID('$tableName', 'U') IS NOT NULL
BEGIN
    DROP TABLE $tableName;
END
GO
"@

    # Prepend the OBJECT_ID logic to the file content
    $newContent = $objectIdCheck + "`r`n" + $content  # Use `r`n for proper newlines

    # Save the modified content to the destination, ensuring correct line endings
    $newFileName = Join-Path $destinationPath $_.Name
    Set-Content -Path $newFileName -Value $newContent -Encoding UTF8
}