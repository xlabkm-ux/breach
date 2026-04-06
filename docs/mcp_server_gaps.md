# MCP Server Gaps (Breach Integration)

## Current status
This project runs against `XLab.UnityMcp.Server` + Unity Editor bridge (`Library/XLabMcpBridge`).

## Confirmed contract mismatches
1. `graph.open_or_create`, `graph.connect`, `graph.edit`, `graph.validate`:
- Present in server tool list.
- Not implemented in Unity Editor bridge command switch.
- Runtime result from bridge: `Unsupported command: graph.*`.

2. Graph output target mismatch:
- Server-side graph helpers currently generate JSON files in `Assets/Graphs/*.json`.
- Project contract expects Unity Visual Scripting assets under `Assets/VisualScripting/...`.

## Required MCP improvements
1. Implement `graph.*` handlers in `McpBridgeProcessor` (Unity side) with real Visual Scripting asset operations.
2. Add path-aware graph arguments:
- `graphPath` (full asset path under `Assets/VisualScripting`)
- optional `graphType` (`flow`, `state`, etc.).
3. Return stable IDs/paths for created graph assets to support follow-up connect/edit operations.
4. Add validation that ensures graph assets are Unity-compatible (not raw JSON placeholders).
5. Extend preflight with `tools.list` runtime probe from live bridge layer, not only server declaration.

## Impact on steps
- Step 9 can complete C# command logic and scene wiring.
- Full Visual Scripting orchestration part remains blocked until `graph.*` bridge support is added.
