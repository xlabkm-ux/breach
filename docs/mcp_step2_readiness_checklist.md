# MCP Step 2 Readiness Checklist

## Goal
Enable Step 2 of vertical slice plan via our in-house MCP:
- create scene shell: `Assets/Scenes/VerticalSlice/VS01_Rescue.unity`

## What is missing now
Current repo has no in-house MCP implementation yet (only docs/policies).  
To execute Step 2 from Codex, we need the minimum MCP stack below.

## Required in-house MCP components (minimum)
1. `MCP server host` (C# .NET console, stdio transport) registered in Codex config.
2. `Unity Editor bridge` package in project (our package, not third-party), running inside Unity.
3. `Project root selection` call (server must target current Unity project path).
4. `Editor state` call (ready/compiling/busy status).
5. `Scene create tool` with parameters:
   - `sceneName`
   - `scenePath`
   - `addToBuildSettings` (bool)
6. `Scene save/verify tool`:
   - save created scene
   - verify file exists in AssetDatabase and disk
7. `Console read tool`:
   - return errors/warnings after operation
8. `Error contract`:
   - deterministic error payloads (busy/not-connected/invalid-path)

## Step 2 Done Criteria (for our MCP)
1. Codex calls `project_root.set` successfully.
2. Codex calls `editor.state` and gets `ready=true`.
3. Codex calls `scene.create` for `VS01_Rescue`.
4. Scene appears at:
   - `Assets/Scenes/VerticalSlice/VS01_Rescue.unity`
   - `Assets/Scenes/VerticalSlice/VS01_Rescue.unity.meta`
5. Unity console has no new errors from this action.
6. Change is committed to git with reviewable diff.

## Suggested implementation order (MCP-side)
1. Create protocol contracts (`requests/responses/errors`).
2. Implement Unity bridge bootstrap + heartbeat.
3. Implement `project_root.set`, `editor.state`.
4. Implement `scene.create` + `scene.save` + path validation.
5. Implement `console.read`.
6. Add smoke test script for Step 2 call chain.
