# MCP Tools By Step

## Policy
Before **every** implementation step we run MCP preflight and proceed only if all required tools are available and healthy.

## Mandatory MCP Preflight (every step)
1. `project_root.set` -> `E:/ИГРЫ/BREACH`
2. `editor.state` -> must be ready, not compiling
3. `tools.list` -> verify required tools for current step
4. Execute step actions
5. `console.read` -> verify no new errors
6. Verify target artifact exists
7. Commit/push only if checks pass

## Step-to-Tools Matrix (23 steps)
1. Structure folders:
- `project_root.set`, `editor.state`, `asset.create_folder`, `asset.exists`, `console.read`

2. Create `VS01_Rescue` scene:
- `project_root.set`, `editor.state`, `scene.create`, `scene.save`, `asset.exists`, `console.read`

3. Create `Bootstrap` scene:
- `project_root.set`, `editor.state`, `scene.create`, `scene.save`, `asset.exists`, `console.read`

4. Apartment layout placeholders:
- `scene.open`, `gameobject.create`, `gameobject.modify`, `scene.save`, `console.read`, `screenshot.scene`

5. Add `MissionDirector` object:
- `scene.open`, `gameobject.create`, `component.add`, `component.set`, `scene.save`, `console.read`

6. Implement `MissionStateService`:
- `script.create_or_edit`, `editor.compile_status`, `console.read`, `asset.refresh`

7. Implement `ObjectiveService`:
- `script.create_or_edit`, `editor.compile_status`, `console.read`

8. Add two operative prefabs and active switch:
- `prefab.create`, `prefab.open`, `component.add`, `prefab.save`, `scene.open`, `prefab.instantiate`, `scene.save`

9. Commands `move/hold/follow`:
- `script.create_or_edit`, `graph.open_or_create`, `graph.connect`, `editor.compile_status`, `console.read`

10. Command `attack-target`:
- `script.create_or_edit`, `graph.edit`, `editor.compile_status`, `console.read`

11. Combat baseline:
- `script.create_or_edit`, `component.add`, `scene.save`, `editor.compile_status`, `console.read`

12. Friendly fire + tests:
- `script.create_or_edit`, `tests.run_editmode`, `tests.results`, `console.read`

13. Enemy prefab + movement:
- `prefab.create`, `component.add`, `prefab.save`, `scene.open`, `prefab.instantiate`, `scene.save`

14. Vision cone:
- `script.create_or_edit`, `component.set`, `scene.save`, `console.read`, `screenshot.scene`

15. Gunshot noise pathway:
- `script.create_or_edit`, `component.add`, `editor.compile_status`, `console.read`

16. Alert FSM:
- `graph.edit`, `script.create_or_edit`, `scene.save`, `console.read`

17. Hostage free/follow:
- `prefab.create_or_edit`, `script.create_or_edit`, `scene.save`, `console.read`

18. Extraction + resolver:
- `gameobject.create`, `component.add`, `script.create_or_edit`, `scene.save`, `console.read`

19. Result UI + localization keys:
- `ui.create_or_edit`, `localization.key_add`, `scene.save`, `console.read`

20. Save schema + autosave:
- `script.create_or_edit`, `scriptableobject.create_or_edit`, `editor.compile_status`, `console.read`

21. Load/restore:
- `script.create_or_edit`, `playmode.enter`, `playmode.exit`, `console.read`

22. Full verification:
- `scene.validate_refs`, `prefab.validate`, `graph.validate`, `tests.run_all`, `console.read`, `screenshot.scene`

23. Final cleanup:
- `asset.list_modified`, `change.summary`, `project.docs_update`, `console.read`

## Notes
- Tool names are canonical API targets for our in-house MCP design.
- If any required tool is missing for a step, we stop and implement/fix tooling first.
