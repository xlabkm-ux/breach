# Unity Package Guide: xLabMcp

`xLabMcp` is optional. TACTICAL BREACH can be developed without it.

## Install

### From a local folder

1. Close Unity and Unity Hub.
2. Add this to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.xlabkm.unity-mcp": "file:../../xLabMcp/dotnet-prototype/unity/com.xlabkm.unity-mcp"
  }
}
```

3. Delete `Library/PackageCache`.
4. Reopen Unity and wait for reimport.

### From Unity Package Manager

1. Open `Window > Package Manager`.
2. Choose `Add package from disk...`.
3. Select the package `package.json`.

## Update

1. Close Unity.
2. Update the package source folder.
3. Delete `Library/PackageCache`.
4. Reopen Unity.

## Remove

1. Close Unity.
2. Remove `com.xlabkm.unity-mcp` from `Packages/manifest.json`.
3. Remove the same entry from `Packages/packages-lock.json` if present.
4. Delete `Library/PackageCache`.
5. Reopen Unity.

## TACTICAL BREACH without xLabMcp

- Do not add `com.xlabkm.unity-mcp` to `Packages/manifest.json`.
- Keep `packages-lock.json` free of that dependency.
- Use TACTICAL BREACH normally with only the project’s own scripts and Unity packages.
