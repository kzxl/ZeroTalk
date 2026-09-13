<#
    publish.ps1 — Publish script for ZeroTalk (Dual Mode: Full & Lite)
    Adheres to AgentOption .NET Publish Release standard & ZeroUniverse rules.
#>
[CmdletBinding()]
param(
    [ValidateSet('Full', 'Lite', 'All')]
    [string]$Mode = 'All',
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$Root = $PSScriptRoot
$Sln = Join-Path $Root "ZeroTalk.sln"
$Dist = Join-Path $Root "publish"

if (Test-Path $Dist) {
    Remove-Item $Dist -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host ">>> Building ZeroTalk solution ($Configuration)..." -ForegroundColor Cyan
dotnet build $Sln -c $Configuration
if ($LASTEXITCODE -ne 0) { throw "Build failed!" }

if ($Mode -eq 'Full' -or $Mode -eq 'All') {
    Write-Host ">>> Publishing ZeroTalk FULL..." -ForegroundColor Cyan
    $outFull = Join-Path $Dist "full"
    $serverFull = Join-Path $outFull "Server"
    $clientFull = Join-Path $outFull "Client"
    
    New-Item -ItemType Directory -Path $serverFull -Force | Out-Null
    New-Item -ItemType Directory -Path $clientFull -Force | Out-Null
    
    Copy-Item -Path "$Root\ZeroTalk.Server\bin\$Configuration\*" -Destination $serverFull -Recurse -Force
    Copy-Item -Path "$Root\ZeroTalk.Client\bin\$Configuration\*" -Destination $clientFull -Recurse -Force
    
    Write-Host "  ✔ Full Server generated at: $serverFull\ZeroTalk.Server.exe" -ForegroundColor Green
    Write-Host "  ✔ Full Client generated at: $clientFull\ZeroTalk.Client.exe" -ForegroundColor Green
}

if ($Mode -eq 'Lite' -or $Mode -eq 'All') {
    Write-Host ">>> Publishing ZeroTalk LITE (Cleaned symbols)..." -ForegroundColor Cyan
    $outLite = Join-Path $Dist "lite"
    $serverLite = Join-Path $outLite "Server"
    $clientLite = Join-Path $outLite "Client"
    
    New-Item -ItemType Directory -Path $serverLite -Force | Out-Null
    New-Item -ItemType Directory -Path $clientLite -Force | Out-Null
    
    Get-ChildItem -Path "$Root\ZeroTalk.Server\bin\$Configuration\*" -File | Where-Object { $_.Extension -notin @('.pdb', '.xml') } | Copy-Item -Destination $serverLite -Force
    Get-ChildItem -Path "$Root\ZeroTalk.Client\bin\$Configuration\*" -File | Where-Object { $_.Extension -notin @('.pdb', '.xml') } | Copy-Item -Destination $clientLite -Force
    
    Write-Host "  ✔ Lite Server generated at: $serverLite\ZeroTalk.Server.exe" -ForegroundColor Green
    Write-Host "  ✔ Lite Client generated at: $clientLite\ZeroTalk.Client.exe" -ForegroundColor Green
}

Write-Host ">>> ZeroTalk publish completed successfully!" -ForegroundColor Green
