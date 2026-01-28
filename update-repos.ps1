# Script to update repository method signatures from Guid id to Guid id
$files = Get-ChildItem -Path "SITRA.Infrastructure\Repositories" -Recurse -Filter "*.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    # Replace method signatures with Guid id parameter
    $content = $content -replace '(\bpublic\s+(?:async\s+)?(?:new\s+)?Task(?:<[^>]+>)?\s+\w+\s*\([^)]*)\blong\s+id\b', '$1Guid id'
    
    # Replace method signatures with long? id parameter
    $content = $content -replace '(\bpublic\s+(?:async\s+)?(?:new\s+)?Task(?:<[^>]+>)?\s+\w+\s*\([^)]*)\blong\?\s+id\b', '$1Guid? id'
    
    Set-Content -Path $file.FullName -Value $content -NoNewline
}

Write-Host "Repository files updated successfully"
