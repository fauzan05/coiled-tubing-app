# ============================================================================
# Coiled Tubing App - Automated Build and Package Script
# ============================================================================

param(
    [string]$Configuration = "Release",
    [string]$Platform = "win-x64",
    [switch]$SkipBuild = $false,
    [switch]$SkipInstaller = $false,
    [switch]$OpenOutput = $true
)

# Configuration
$ProjectPath = "..\coiled-tubing-app\coiled-tubing-app.csproj"
$InstallerScript = "CoiledTubingApp-Professional.iss"
$OutputDir = "Output"

# Try multiple possible Inno Setup locations
$InnoSetupPaths = @(
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
)

$InnoSetupPath = $null
foreach ($path in $InnoSetupPaths) {
    if (Test-Path $path) {
        $InnoSetupPath = $path
        break
    }
}

Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "Coiled Tubing App - Build and Package Script" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan

# Check if Inno Setup is installed
if (-not $InnoSetupPath) {
    Write-Host "ERROR: Inno Setup not found in any of these locations:" -ForegroundColor Red
    foreach ($path in $InnoSetupPaths) {
        Write-Host "  - $path" -ForegroundColor Yellow
    }
    Write-Host "Please install Inno Setup 6 from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
    exit 1
}

Write-Host "Using Inno Setup at: $InnoSetupPath" -ForegroundColor Green

# Step 1: Clean previous builds
Write-Host "`n[1/5] Cleaning previous builds..." -ForegroundColor Green
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
    Write-Host "? Cleaned output directory" -ForegroundColor Green
}

# Step 2: Build the application
if (-not $SkipBuild) {
    Write-Host "`n[2/5] Building application..." -ForegroundColor Green
    
    # Clean project
    & dotnet clean $ProjectPath --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to clean project" -ForegroundColor Red
        exit 1
    }
    
    # Restore packages
    & dotnet restore $ProjectPath
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to restore packages" -ForegroundColor Red
        exit 1
    }
    
    # Build project
    & dotnet build $ProjectPath --configuration $Configuration --runtime $Platform --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to build project" -ForegroundColor Red
        exit 1
    }
    
    # Publish project
    & dotnet publish $ProjectPath --configuration $Configuration --runtime $Platform --self-contained
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to publish project" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "? Application built successfully" -ForegroundColor Green
} else {
    Write-Host "`n[2/5] Skipping build (using existing build)..." -ForegroundColor Yellow
}

# Step 3: Verify published files
Write-Host "`n[3/5] Verifying published files..." -ForegroundColor Green
$PublishPath = "..\coiled-tubing-app\bin\$Configuration\net8.0-windows10.0.19041.0\$Platform\publish"

if (-not (Test-Path "$PublishPath\coiled-tubing-app.exe")) {
    Write-Host "? Main executable not found at: $PublishPath" -ForegroundColor Red
    exit 1
}

$FileCount = (Get-ChildItem $PublishPath -Recurse | Measure-Object).Count
$TotalSize = [math]::Round((Get-ChildItem $PublishPath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)

Write-Host "? Found $FileCount files, total size: $TotalSize MB" -ForegroundColor Green

# Step 4: Create installer
if (-not $SkipInstaller) {
    Write-Host "`n[4/5] Creating installer..." -ForegroundColor Green
    
    # Create output directory
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    
    # Run Inno Setup compiler
    & $InnoSetupPath $InstallerScript
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to create installer" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "? Installer created successfully" -ForegroundColor Green
} else {
    Write-Host "`n[4/5] Skipping installer creation..." -ForegroundColor Yellow
}

# Step 5: Display results
Write-Host "`n[5/5] Build Summary" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green

if (Test-Path "$OutputDir\CoiledTubingApp-Setup-v1.0.0.exe") {
    $InstallerSize = [math]::Round((Get-Item "$OutputDir\CoiledTubingApp-Setup-v1.0.0.exe").Length / 1MB, 2)
    Write-Host "? Installer: $OutputDir\CoiledTubingApp-Setup-v1.0.0.exe ($InstallerSize MB)" -ForegroundColor Green
}

Write-Host "? Application: $PublishPath\coiled-tubing-app.exe" -ForegroundColor Green
Write-Host "? Configuration: $Configuration" -ForegroundColor Green
Write-Host "? Platform: $Platform" -ForegroundColor Green
Write-Host "? Build completed successfully!" -ForegroundColor Green

# Open output folder
if ($OpenOutput -and (Test-Path $OutputDir)) {
    Write-Host "`nOpening output folder..." -ForegroundColor Cyan
    Start-Process explorer.exe -ArgumentList $OutputDir
}

Write-Host "`nInstaller ready for distribution!" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan