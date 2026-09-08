# SimpleChatBox Automated Demo Launcher
# Builds the solution, runs automated tests, and launches Server + 2 Pre-configured Demo Clients (Alice & Bob)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   SimpleChatBox Demo Launch Orchestrator          " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

# 1. Build Solution
Write-Host "[1/4] Building solution..." -ForegroundColor Yellow
& dotnet build "$RootDir\ChatBoxSimple.sln" -c Debug
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed! Please check compilation errors."
    exit 1
}

# 2. Run Automated Tests
Write-Host "[2/4] Running automated test suite..." -ForegroundColor Yellow
& "$RootDir\ChatBox.Tests\bin\Debug\ChatBox.Tests.exe"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Automated tests failed!"
    exit 1
}

# 3. Launch Server
Write-Host "[3/4] Starting Server Dashboard..." -ForegroundColor Yellow
$serverExe = "$RootDir\ChatBox.Server\bin\Debug\ChatBox.Server.exe"
Start-Process -FilePath $serverExe -ArgumentList "autostart" -WorkingDirectory (Split-Path -Parent $serverExe)

Start-Sleep -Milliseconds 1500

# 4. Launch 2 Clients with 1-Click Auto-Login
Write-Host "[4/4] Launching Demo Clients (Alice & Bob)..." -ForegroundColor Yellow
$clientExe = "$RootDir\ChatBox.Client\bin\Debug\ChatBox.Client.exe"
Start-Process -FilePath $clientExe -ArgumentList "demo_alice" -WorkingDirectory (Split-Path -Parent $clientExe)
Start-Sleep -Milliseconds 500
Start-Process -FilePath $clientExe -ArgumentList "demo_bob" -WorkingDirectory (Split-Path -Parent $clientExe)

Write-Host "`nAll demo instances are running!" -ForegroundColor Green
Write-Host "1. Alice and Bob will automatically log in."
Write-Host "2. Select Alice on Bob's client (or Bob on Alice's client) to exchange encrypted messages."
Write-Host "3. Click the video icon to test P2P Video Calling with the animated Synthetic Camera or Screen Share!"
Write-Host "4. Use Server Dashboard to observe real-time telemetry, routing, and client metrics."
