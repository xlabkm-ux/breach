# V1 Resource Expansion Steps (After Vertical Slice 23)

## Goal
Convert the vertical slice baseline into a production-ready v1 gameplay slice using imported resource packs without breaking save/localization/stability constraints.

## Scope policy
- Keep gameplay contracts stable.
- Integrate visuals through prefabs/data hooks, not hardcoded scene-only edits.
- Validate each step with compile + console + scene integrity.
- Separate infrastructure/resource commits from gameplay-functional commits.

## Step pack A - Visual world productionization
24. Create tilemap pipeline:
- `World_Base`, `World_Collision`, `World_Decor`, `World_Interactables`.
- Source from imported tiles under `Assets/_Core/Graphics/Tiles/...` and `Assets/World/...`.

25. Build apartment layout art pass:
- Replace placeholder geometry with tilemap + prop prefabs.
- Keep collision and line-of-fire readability.

26. Cover-system authoring pass:
- Tag cover props (half/full cover marker components).
- Build deterministic cover metadata for AI + squad command logic.

27. Entry/extraction visual pass:
- Distinct visual language for entry points and extraction zone.
- Ensure accessibility for Android_Low readability.

## Step pack B - Character and combat visuals
28. Operative visual prefabs v1:
- Introduce `OperativeVisualSet` and bind imported sprites.
- Keep command and combat components unchanged.

29. Enemy visual prefabs v1:
- Enemy sprite variants + team-color readability.
- Keep AI/perception scripts intact.

30. Weapon visual binding:
- Bind imported weapon sprites to `SimpleShooter`/future weapon components.
- Add minimal muzzle flash hook.

31. Hit/death feedback pass:
- One primary feedback pattern per event (`hit`, `death`).
- No stacked effect spam.

## Step pack C - Tactical readability and stealth/assault support
32. Vision cone visual overlay:
- Runtime cone mesh/sprite for enemy perception debugging and gameplay readability.

33. Noise propagation visualization:
- Short-lived ring indicator when shots occur.
- Link to alert escalation pipeline.

34. Alert-state readability:
- Enemy state marker (`idle/suspicious/alert`) with minimal UI above unit.

35. Friendly-fire readability:
- Fire line indicator + ally-at-risk warning signal.

## Step pack D - Mission flow completeness
36. Hostage visual/interaction prefab:
- `Hostage_Civilian` with free/follow feedback states.

37. Result screen art pass:
- Key-based strings only.
- Visual styling from imported pack without hardcoded text.

38. Localization-safe UI pass:
- Check overflow/wrapping for RU/EN fallback pair.

39. Save and restore visual consistency pass:
- Rebind restored scene state to visual variants and overlays.

40. Build-profile visual optimization:
- `PC_Default`: full sprites/effects budget.
- `Android_Default`: reduced effect intensity.
- `Android_Low`: strict effect culling and compressed sprite path.

## Step pack E - Release preflight for v1 candidate
41. Scene/prefab reference sweep.
42. Combat/perception regression scenarios.
43. Hostage success/fail scenario sweep.
44. Save/load resilience sweep.
45. Localization key coverage sweep.
46. Android memory/perf quick pass.
47. Windows readability/input sanity pass.
48. v1 candidate baseline commit + changelog snapshot.

## MCP dependency notes
Current blockers to fully automate all steps in Codex:
- `graph.*` not implemented on Unity bridge level.
- `tests.results` not implemented on bridge level.
- `com.unity.test-framework` missing for reliable EditMode automation.

Until fixed:
- Prioritize C# and scene/prefab steps with available tools.
- Keep Visual Scripting edits minimal and explicitly marked as pending MCP-graph parity.
