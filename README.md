# Shoko Release Importer/Exporter Plugin

A [Shoko](https://shokoanime.com/) plugin that imports releases from and exports releases to the file system near the video files.

## Features

- **Import Releases** — Automatically reads release info from JSON files stored near your video files.
- **Export Releases** — Saves release metadata to the file system whenever a release is matched or updated.
- **Relocation Support** — Moves exported release files alongside video files when they are relocated.
- **Deletion Cleanup** — Optionally deletes physical release files when a release or video is removed.
- **Bulk Actions** — Export all existing releases or purge all exported release files on demand.
- **Customizable Paths** — Configure release file extension and location templates to suit your library layout.

## Installation

### GUI (Recommended)

1. Open the Shoko Web UI and navigate to **Settings → Plugins → Repositories**.
2. Add the manifest URL:
   ```
   https://raw.githubusercontent.com/revam/dotnet-shoko-plugin-release-exporter/stable/manifest.json
   ```
3. Go to **Settings → Plugins → Browse** and find **Release Importer/Exporter**.
4. Click **Install** on the desired version.
5. Restart Shoko.

### Manual

1. Download the latest `Shoko.Plugin.ReleaseExporter-<version>-any.zip` from the [Releases](../../releases) page.
2. Extract the ZIP and place `Shoko.Plugin.ReleaseExporter.dll` into your Shoko **Plugins** folder.
3. Restart Shoko.

## Configuration

The plugin exposes the following settings in the Shoko UI:

| Setting | Default | Description |
|---|---|---|
| **Export Releases** | `false` | Enable saving release metadata to the file system. |
| **Relocate Releases** | `true` | Move exported release files when the associated video file is relocated. |
| **Delete Physical Release Files** | `false` | Delete the physical release file when a release is deleted from the system. |
| **Release Extension** | `.release.json` | The file extension used for exported release files. |
| **Release Location Templates** | `%managed_folder_root%%file_path_no_ext%%release_extension%` | Template paths determining where release files are stored. |

## Building from Source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet restore
dotnet build --configuration Release
```

The compiled assembly will be located at `source/bin/Release/net10.0/Shoko.Plugin.ReleaseExporter.dll`.

## License

This project is licensed under the MIT License.
