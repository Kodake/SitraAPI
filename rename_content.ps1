Get-ChildItem -Path . -Include *.cs,*.csproj,*.sln,*.json,*.xml,*.html,*.cshtml,*.md,*.txt,*.http -Recurse -Exclude .git,.vs,bin,obj,.gemini | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "PosAPI") {
        Write-Host "Replacing in: $($_.FullName)"
        $content = $content -replace "PosAPI", "SITRA"
        Set-Content -Path $_.FullName -Value $content -NoNewline
    }
}
