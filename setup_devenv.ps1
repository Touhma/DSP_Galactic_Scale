Add-Type -AssemblyName System.Windows.Forms

function Find-DSPPlugins {
    $commonPaths = @(
    "$env:USERPROFILE\AppData\Roaming\r2modmanPlus-local\DysonSphereProgram\profiles\Default\BepInEx\plugins",
        "$env:ProgramFiles\Steam\steamapps\common\Dyson Sphere Program\BepInEx\plugins",
        "${env:ProgramFiles(x86)}\Steam\steamapps\common\Dyson Sphere Program\BepInEx\plugins"
        
    )
    
    foreach ($path in $commonPaths) {
        if (Test-Path $path) {
            return $path
        }
    }
    return $null
}

function Select-PluginsFolder {
    $dialog = New-Object System.Windows.Forms.FolderBrowserDialog
    $dialog.Description = "Select your DSP BepInEx plugins folder"
    $dialog.ShowNewFolderButton = $false
    
    # Try to start in a sensible location
    $initialPath = Find-DSPPlugins
    if ($initialPath) {
        $dialog.SelectedPath = $initialPath
    }
    
    if ($dialog.ShowDialog() -eq 'OK') {
        return $dialog.SelectedPath
    }
    return $null
}

function Test-PluginsDirectory {
    param([string]$Path)
    
    if (-not (Test-Path $Path)) {
        return $false
    }
    
    # Check if this looks like a plugins directory
    $parentDir = Split-Path $Path -Parent
    $bepinexDir = Split-Path $parentDir -Leaf
    return $bepinexDir -eq "BepInEx"
}

# Main script
Write-Host "GalacticScale 3 Development Environment Setup" -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor Cyan

$pluginsPath = Find-DSPPlugins
if (-not $pluginsPath) {
    Write-Host "Could not automatically find DSP plugins directory."
    Write-Host "Please select it manually..."
    $pluginsPath = Select-PluginsFolder
}

if (-not $pluginsPath) {
    Write-Host "No plugins directory selected. Setup cancelled." -ForegroundColor Red
    exit 1
}

if (-not (Test-PluginsDirectory $pluginsPath)) {
    Write-Host "Warning: Selected directory doesn't appear to be a BepInEx plugins folder." -ForegroundColor Yellow
    $confirm = Read-Host "Continue anyway? (y/N)"
    if ($confirm -ne "y") {
        Write-Host "Setup cancelled." -ForegroundColor Red
        exit 1
    }
}

# Create DevEnv.targets with the correct format
$devEnvContent = @"
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <PluginDir>$pluginsPath</PluginDir>
  </PropertyGroup>
</Project>
"@

$devEnvContent | Out-File "DevEnv.targets" -Encoding UTF8

Write-Host "`nSetup complete!" -ForegroundColor Green
Write-Host "Created DevEnv.targets with plugins path: $pluginsPath"
Write-Host "You can now build the project in Visual Studio." 