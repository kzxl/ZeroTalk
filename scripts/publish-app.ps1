param (
    [ValidateSet("Full", "Lite")]
    [string]$Mode = "Full",
    [string]$Version = "1.1.0",
    [string]$OutputDir = "$PSScriptRoot\..\publish"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   SimpleChatBox Build & Publish Script           " -ForegroundColor Cyan
Write-Host "   Target Mode: $Mode                             " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

$config = "Release"
$targetDir = Join-Path $OutputDir $Mode

if (Test-Path $targetDir) {
    Remove-Item $targetDir -Recurse -Force
}
New-Item -ItemType Directory -Path $targetDir | Out-Null

Write-Host "[1/3] Building solution in $config configuration..." -ForegroundColor Yellow
& dotnet build "$RootDir\ZeroTalk.sln" -c $config
if ($LASTEXITCODE -ne 0) {

    Write-Error "Build failed!"
    exit 1
}

Write-Host "[2/3] Running tests..." -ForegroundColor Yellow
& "$RootDir\ChatBox.Tests\bin\$config\ChatBox.Tests.exe"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Test suite failed!"
    exit 1
}

Write-Host "[3/3] Packaging distribution..." -ForegroundColor Yellow

$serverOut = Join-Path $targetDir "ChatBox.Server"
$clientOut = Join-Path $targetDir "ChatBox.Client"

New-Item -ItemType Directory -Path $serverOut | Out-Null
New-Item -ItemType Directory -Path $clientOut | Out-Null

# Copy Server binaries
Get-ChildItem "$RootDir\ChatBox.Server\bin\$config" -File | ForEach-Object {
    if ($Mode -eq "Lite" -and ($_.Extension -eq ".pdb" -or $_.Extension -eq ".xml")) { return }
    Copy-Item $_.FullName -Destination $serverOut
}

# Copy Client binaries
Get-ChildItem "$RootDir\ChatBox.Client\bin\$config" -File | ForEach-Object {
    if ($Mode -eq "Lite" -and ($_.Extension -eq ".pdb" -or $_.Extension -eq ".xml")) { return }
    Copy-Item $_.FullName -Destination $clientOut
}

$zipFile = Join-Path $OutputDir "SimpleChatBox-v$Version-$Mode.zip"
if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}
Compress-Archive -Path "$targetDir\*" -DestinationPath $zipFile

Write-Host "`nPublish complete!" -ForegroundColor Green
Write-Host "Server:  $serverOut"
Write-Host "Client:  $clientOut"
Write-Host "Archive: $zipFile"
