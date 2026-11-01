param(
    [string]$Configuration = 'Release',
    [ValidateSet('All', 'Backend', 'Frontend')]
    [string]$Phase = 'All'
)
$ErrorActionPreference = 'Stop'
$workspaceRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = $workspaceRoot
$artifactRoot = Join-Path $workspaceRoot 'artifacts'

function Invoke-Checked {
    param([string]$Executable, [string[]]$Arguments)
    & $Executable @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Executable failed with exit code $LASTEXITCODE" }
}

Push-Location $projectRoot
try {
    if ($Phase -in @('All', 'Backend')) {
        Invoke-Checked -Executable dotnet -Arguments @('build', 'src/Recovery.BackOffice.slnx', '-c', $Configuration)
        foreach ($suite in @('Recovery.BackOffice.Architecture.Tests', 'Recovery.BackOffice.Identity.Tests', 'Recovery.BackOffice.Auditing.Tests', 'Recovery.BackOffice.Framework.Tests', 'Recovery.BackOffice.Multitenancy.Tests')) {
            $resultPath = Join-Path $artifactRoot "test-results/$suite"
            Invoke-Checked -Executable dotnet -Arguments @('test', "src/Tests/$suite", '-c', $Configuration, '--no-build', '--no-restore', '--logger', 'trx', '--results-directory', $resultPath, '--collect', 'XPlat Code Coverage', '--', 'DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover')
        }
        Invoke-Checked -Executable dotnet -Arguments @('publish', 'src/Host/Recovery.BackOffice.Api', '-c', $Configuration, '--no-build', '-o', (Join-Path $artifactRoot 'publish/api'))
    }
    if ($Phase -in @('All', 'Frontend')) {
        foreach ($client in @('admin', 'dashboard')) {
            Push-Location (Join-Path $projectRoot "clients/$client")
            try {
                Invoke-Checked -Executable npm.cmd -Arguments @('ci', '--no-audit', '--no-fund')
                Invoke-Checked -Executable npm.cmd -Arguments @('run', 'build')
                $destination = Join-Path $artifactRoot "publish/$client"
                New-Item -ItemType Directory -Path $destination -Force | Out-Null
                Copy-Item -Path 'dist/*' -Destination $destination -Recurse -Force
            } finally { Pop-Location }
        }
    }
} finally { Pop-Location }
