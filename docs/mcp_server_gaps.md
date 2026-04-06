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

3. Test result contract mismatch:
- `tests.results` is declared in server tool list.
- Unity Editor bridge does not implement `tests.results`.
- Runtime result from bridge: `Unsupported command: tests.results`.

4. EditMode tests execution dependency gap:
- `tests.run_editmode` is implemented in bridge.
- Runtime result indicates missing Unity package: `com.unity.test-framework`.
- Without this package, step-level automated verification for tests is blocked.

5. Bridge responsiveness instability:
- Intermittent `timeout waiting response` on basic runtime probes (`editor.state`, `asset.refresh`, `editor.compile_status`).
- Heartbeat file may stop updating while command queue still receives requests.
- This blocks reliable per-step verification even when tools exist in contract.

6. UI/localization bridge mismatch:
- `ui.create_or_edit` and `localization.key_add` exist in server tool list and server handlers.
- Direct Unity bridge command channel returns `Unsupported command` for both.
- Result: UI/localization steps cannot be fully verified via bridge-only flow.

## Required MCP improvements
1. Implement `graph.*` handlers in `McpBridgeProcessor` (Unity side) with real Visual Scripting asset operations.
2. Add path-aware graph arguments:
- `graphPath` (full asset path under `Assets/VisualScripting`)
- optional `graphType` (`flow`, `state`, etc.).
3. Return stable IDs/paths for created graph assets to support follow-up connect/edit operations.
4. Add validation that ensures graph assets are Unity-compatible (not raw JSON placeholders).
5. Extend preflight with `tools.list` runtime probe from live bridge layer, not only server declaration.
6. Implement bridge support for `tests.results` with stable summary payload (`passed`, `failed`, `skipped`, `duration`).
7. Ensure `com.unity.test-framework` is part of baseline project dependencies for testable steps.
8. Add bridge watchdog + recovery path:
- stale heartbeat detection,
- queue timeout diagnostics,
- optional auto-restart instruction path for editor-side bridge processor.
9. Decide one canonical execution mode for tools:
- either run all tool calls through MCP server (stdio dispatcher),
- or implement missing handlers in editor bridge for parity (`ui.create_or_edit`, `localization.key_add`, etc.).

## Impact on steps
- Step 9 can complete C# command logic and scene wiring.
- Full Visual Scripting orchestration part remains blocked until `graph.*` bridge support is added.
- Step 12 test automation remains partially blocked until test-framework dependency and `tests.results` support are in place.
- Any step requiring live Unity mutation/validation is at risk while bridge responsiveness is unstable.
- UI/localization automation remains partially blocked until command-parity decision is implemented.
