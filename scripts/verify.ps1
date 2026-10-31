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

    $verificationDirectory = Join-Path $repositoryRoot "artifacts/verification"

    New-Item -ItemType Directory -Force `
        -Path $verificationDirectory | Out-Null

    $applicationProcess = $null

    try {
        $applicationProcess = Start-Process `
            -FilePath (Get-Command dotnet).Source `
            -ArgumentList @(
                "artifacts/publish/api/Recovery.BackOffice.Api.dll"
                "--environment"
                "Production"
                "--urls"
                "http://127.0.0.1:5080"
            ) `
            -WorkingDirectory $repositoryRoot `
            -RedirectStandardOutput "$verificationDirectory/api.stdout.log" `
            -RedirectStandardError "$verificationDirectory/api.stderr.log" `
            -WindowStyle Hidden `
            -PassThru

        $response = $null
        $deadline = [DateTime]::UtcNow.AddSeconds(30)

        while ([DateTime]::UtcNow -lt $deadline) {
            $applicationProcess.Refresh()

            if ($applicationProcess.HasExited) {
                throw "API exited before verification. Check artifacts/verification."
            }

            try {
                $response = Invoke-WebRequest `
                    -Uri "http://127.0.0.1:5080/" `
                    -UseBasicParsing `
                    -TimeoutSec 2
            }
            catch {
                Start-Sleep -Milliseconds 250
                continue
            }

            break
        }

        if ($null -eq $response) {
            throw "API did not respond within the startup deadline."
        }

        if ($response.StatusCode -ne 200) {
            throw "Unexpected HTTP status: $($response.StatusCode)."
        }

        if ($response.Content -cne "Hello World!") {
            throw "Unexpected response body: $($response.Content)"
        }

        $applicationProcess.Refresh()

        if ($applicationProcess.HasExited) {
            throw "API exited during verification."
        }

        Write-Host "HTTP smoke test passed."
    }
    finally {
        if ($null -ne $applicationProcess) {
            $applicationProcess.Refresh()

            if (-not $applicationProcess.HasExited) {
                Stop-Process -Id $applicationProcess.Id -Force
                $applicationProcess.WaitForExit()
            }

            $applicationProcess.Dispose()
        }
    }


    Write-Host "All checks passed."
}
finally {
    Pop-Location
}