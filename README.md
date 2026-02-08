# SolidWorks Custom Panel Add-in

A SolidWorks add-in that adds a custom task pane (panel) to the SOLIDWORKS application. Built with [Xarial.XCad](https://github.com/xarial/xcad) for SolidWorks, .NET Framework 4.7.2, and WPF.

## Features

- **Custom task pane** — WPF-based panel shown in the SolidWorks task pane.
- **Xarial.XCad** — Uses Xarial.XCad.SolidWorks and related packages for add-in lifecycle and UI.
- **Standalone registration** — `RegisterAddin.exe` registers the add-in and resolves SolidWorks interop DLLs from the local SolidWorks installation (no need to copy interop DLLs).

## Requirements

- **SolidWorks** — Installed with API redistributables (typically `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\`).
- **.NET Framework 4.7.2** (x64).
- **Visual Studio** or MSBuild to build the solution.

## Building

1. Open the solution or project in Visual Studio, or build from the project directory:
   ```bash
   dotnet build -c Release
   ```
2. Main output: `bin\Release\net472\` (or the path referenced by your scripts).
3. The build also compiles and copies `RegisterAddin.exe` into the output folder.

## Registration

Register the add-in so SolidWorks loads it at startup.

### Option 1: RegisterAddin.exe (recommended)

Run as **Administrator** from the folder that contains `SolidWorksExportAddin.dll` (e.g. `bin\Release\net472\` or your deployment folder):

```cmd
RegisterAddin.exe
```

To unregister:

```cmd
RegisterAddin.exe /u
```

`RegisterAddin.exe` resolves `SolidWorks.Interop.*` from the SolidWorks installation, so you do not need to copy those DLLs next to the add-in.

### Option 2: RegAsm

From the folder that contains `SolidWorksExportAddin.dll`:

```cmd
%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /codebase SolidWorksExportAddin.dll
```

Unregister:

```cmd
%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /codebase /u SolidWorksExportAddin.dll
```

### Add-in manifest (optional)

To set description and load-on-startup behavior, merge `Panel\AddinManifest.reg` into the registry (adjust paths/names if your deployment differs). The add-in GUID is `{12345678-1234-1234-1234-123456789ABC}`.

## Project structure

| Path | Description |
|------|-------------|
| `Core\` | Add-in entry (`SwAddin.cs`), COM registration, SolidWorks interop loader |
| `Views\` | WPF task pane UI (`ExportTaskPaneControl.xaml`) |
| `RegisterAddin\` | Standalone registration tool (`RegisterAddin.exe`) |
| `Panel\` | Deployment assets (interop DLLs, scripts, manifest, icon) |
| `Resources\` | Icons and embedded resources |

## License

See repository or project metadata for license information.
