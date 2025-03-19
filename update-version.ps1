#!/usr/bin/env pwsh

# Script to extract latest version from README.md and update it in all project files
Write-Host "GalacticScale Version Updater" -ForegroundColor Cyan

# Get the project root directory
$rootDir = $PSScriptRoot

# Read the README.md file to extract the latest version
$readmePath = Join-Path -Path $rootDir -ChildPath "Package\README.md"
if (-not (Test-Path -Path $readmePath)) {
    Write-Host "README.md not found at path: $readmePath" -ForegroundColor Red
    exit 1
}

$readmeContent = Get-Content -Path $readmePath -Raw

# Extract the latest version using regex
# Looking for the pattern "Version X.Y.Z - " at the beginning of a line
$versionMatch = [regex]::Match($readmeContent, "- Version (\d+\.\d+\.\d+) -")
if (-not $versionMatch.Success) {
    Write-Host "Could not find version pattern in README.md" -ForegroundColor Red
    exit 1
}

$latestVersion = $versionMatch.Groups[1].Value
Write-Host "Latest version found in README.md: $latestVersion" -ForegroundColor Green

# Update GalacticScale3.csproj
$csprojPath = Join-Path -Path $rootDir -ChildPath "GalacticScale3.csproj"
if (Test-Path -Path $csprojPath) {
    $csprojContent = Get-Content -Path $csprojPath -Raw
    
    if ($csprojContent) {
        # Update version only, not the description
        $updatedCsprojContent = [regex]::Replace($csprojContent, '<Version>([^<]+)</Version>', "<Version>$latestVersion</Version>")
        Set-Content -Path $csprojPath -Value $updatedCsprojContent -NoNewline
        Write-Host "Updated version in GalacticScale3.csproj" -ForegroundColor Green
    } else {
        Write-Host "GalacticScale3.csproj file is empty" -ForegroundColor Yellow
    }
} else {
    Write-Host "GalacticScale3.csproj file not found" -ForegroundColor Yellow
}

# Update thunderstore.toml if it exists
$tomlPath = Join-Path -Path $rootDir -ChildPath "thunderstore.toml"
if (Test-Path -Path $tomlPath) {
    $tomlContent = Get-Content -Path $tomlPath -Raw
    
    if ($tomlContent) {
        # Update version number
        $updatedTomlContent = [regex]::Replace($tomlContent, 'versionNumber = "([^"]+)"', "versionNumber = `"$latestVersion`"")
        
        # Set the standard description for the TOML file
        $standardDesc = "v$latestVersion Galaxy Customization. New Planets. Different Sized Planets. Up to 100 Planets/star and 1024 Stars. DF is more or less balanced. See GS Discord Server"
        
        # Update the description in the TOML file
        $updatedTomlContent = [regex]::Replace($updatedTomlContent, 
            'description = "([^"]*)"', 
            "description = `"$standardDesc`"")
        
        Write-Host "Updated description in thunderstore.toml to standard format" -ForegroundColor Green
        
        Set-Content -Path $tomlPath -Value $updatedTomlContent -NoNewline
        Write-Host "Updated version in thunderstore.toml" -ForegroundColor Green
    } else {
        Write-Host "thunderstore.toml file is empty" -ForegroundColor Yellow
    }
} else {
    Write-Host "thunderstore.toml file not found" -ForegroundColor Yellow
}

# Update Bootstrap.cs if it exists
$bootstrapPath = Join-Path -Path $rootDir -ChildPath "Bootstrap.cs"
if (Test-Path -Path $bootstrapPath) {
    $bootstrapContent = Get-Content -Path $bootstrapPath -Raw
    
    if ($bootstrapContent) {
        $updatedBootstrapContent = [regex]::Replace($bootstrapContent, '\[BepInPlugin\("dsp\.galactic-scale\.2", "Galactic Scale 2 Plug-In", "([^"]+)"\)\]', "[BepInPlugin(`"dsp.galactic-scale.2`", `"Galactic Scale 2 Plug-In`", `"$latestVersion`")]")
        Set-Content -Path $bootstrapPath -Value $updatedBootstrapContent -NoNewline
        Write-Host "Updated version in Bootstrap.cs" -ForegroundColor Green
    } else {
        Write-Host "Bootstrap.cs file is empty" -ForegroundColor Yellow
    }
} else {
    Write-Host "Bootstrap.cs file not found" -ForegroundColor Yellow
}

Write-Host "All files have been updated to version $latestVersion" -ForegroundColor Cyan 