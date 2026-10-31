$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repositoryRoot

try {
    dotnet restore Recovery.BackOffice.slnx -warnaserror
    if ($LASTEXITCODE -ne 0) {
        throw "Restore failed."
    }

    dotnet format Recovery.BackOffice.slnx --verify-no-changes --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Formatting verification failed."
    }

    dotnet build Recovery.BackOffice.slnx `
        --configuration Release `
        --no-restore `
        -warnaserror

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed."
    }

    dotnet publish src/Host/Recovery.BackOffice.Api/Recovery.BackOffice.Api.csproj `
        --configuration Release `
        --no-build `
        --no-restore `
        --output artifacts/publish/api `
        -warnaserror

    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed."
    }

    Write-Host "All checks passed."
}
finally {
    Pop-Location
}