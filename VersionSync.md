# GalacticScale Version Synchronization

This system automatically synchronizes version numbers across multiple project files to save you from manually updating each file when you increment the version.

## How It Works

1. The system extracts the latest version number from `Package/README.md`
2. It then updates the version in:
   - `GalacticScale3.csproj` (in the `<Version>` tag)
   - `thunderstore.toml` (in the `versionNumber` field)
   - `Bootstrap.cs` (in the BepInPlugin attribute)

## Files

- `update-version.ps1`: The main script that performs the version extraction and updating
- `sync-version.ps1`: A helper script that can be called from other build processes
- `build.ps1`: An example build script that synchronizes versions before building

## Usage

### Manually Updating Versions

To manually synchronize all version numbers from `README.md`:

```powershell
./update-version.ps1
```

### Building with Version Synchronization

To build your project with automatic version synchronization:

```powershell
./build.ps1        # For Release build
./build.ps1 debug  # For Debug build
```

## Version Format

The script expects the version in `README.md` to be in the format:

```
- Version X.Y.Z - Description of changes
```

Where X.Y.Z is the semantic version (e.g., 2.72.3).

## Important Notes

1. Always update the version in `README.md` first
2. The script assumes all files are in the project root directory
3. Make sure the scripts have execution permissions (`chmod +x *.ps1` on Unix-based systems)

## Troubleshooting

If the script fails to find the version in `README.md`, ensure:
1. The version follows the expected format: `- Version X.Y.Z - `
2. The path to `README.md` is correct (`Package/README.md`)
3. The file can be read by the current user 