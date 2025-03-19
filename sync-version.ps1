#!/usr/bin/env pwsh

# This script is meant to be called from your build process
# It updates all version numbers based on README.md and then proceeds with the build

# Run the version updater
& "$PSScriptRoot\update-version.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Version update failed. Build aborted." -ForegroundColor Red
    exit $LASTEXITCODE
}

# After updating versions, you can call your build process here
# For example:
# & dotnet build
# or any other command that builds your project

Write-Host "Versions synchronized. Ready to build." -ForegroundColor Green 