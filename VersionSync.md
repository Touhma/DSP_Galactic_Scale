# GalacticScale Version Synchronization

This system automatically synchronizes version numbers across multiple project files to save you from manually updating each file when you increment the version.

## How It Works

1. The system reads the first release heading in `Package/CHANGELOG.md`.
2. It then updates the version in:
   - `GalacticScale3.csproj` (in the `<Version>` tag)
   - `thunderstore.toml` (in the `versionNumber` field)
   - `Bootstrap.cs` (in the BepInPlugin attribute)

## Files

- `update-version.ps1`: The main script that performs the version extraction and updating
- `sync-version.ps1`: A helper script that can be called from other build processes
- `GalacticScale3.csproj` runs the updater before a build.

## Usage

### Manually Updating Versions

Run the updater to synchronize all version numbers from `Package/CHANGELOG.md`.

```powershell
./update-version.ps1
```

### Building with Version Synchronization

Build with either configuration to synchronize versions automatically.

```powershell
dotnet build -c Release
dotnet build -c Debug
```

## Version Format

Add each new release at the top of `Package/CHANGELOG.md` with a three-part version heading and a list of changes.

```
## 2.80.12
- Describe the change
```

Thunderstore displays this file in the package's Changelog tab.

## Important Notes

1. Add the new version to `Package/CHANGELOG.md` before building.
2. Keep the newest release at the top of the changelog.

## Troubleshooting

If the script fails to find the version, check that `Package/CHANGELOG.md` is readable and that the release heading uses the `## X.Y.Z` format.
