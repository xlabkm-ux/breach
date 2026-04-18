# TACTICAL BREACH Test Plan Checklist

Date: 2026-04-07
Scope: full project test checklist for TACTICAL BREACH v1 readiness.

## How to use

- Run the checks in order.
- Mark each item as `PASS`, `FAIL`, or `NOT RUN`.
- If a section fails, fix the issue before moving to the next section.
- Prefer Unity Editor validation for anything involving scenes, prefabs, graphs, UI, save data, or quality profiles.

## 1. Project Compile

- [ ] Open the project in Unity 6 without script compile errors.
- [ ] Confirm `Assets` runtime scripts compile successfully.
- [ ] Confirm `Assets/Tests/EditMode/TacticalBreachVerificationTests.cs` compiles.
- [ ] Confirm local packages do not block project compilation.

## 2. Scene And Prefab Integrity

- [ ] Run the missing-reference sweep over all scenes.
- [ ] Run the missing-reference sweep over all prefabs.
- [ ] Confirm `Bootstrap.unity` opens cleanly.
- [ ] Confirm `VS01_Rescue.unity` opens cleanly.
- [ ] Confirm no unintended prefab overrides were introduced.

## 3. Core Mission Flow

- [ ] Start a new mission from bootstrap.
- [ ] Confirm the mission begins in `Infiltration`.
- [ ] Confirm `Engagement`, `HostageSecured`, `Extraction`, `Success`, and `Failed` transitions work as intended.
- [ ] Confirm `MissionResultResolver` reaches success only after objective completion.
- [ ] Confirm fail states are triggered by squad wipe or hostage death.

## 4. Combat And Perception

- [ ] Confirm squad damage applies correctly.
- [ ] Confirm friendly fire can be blocked and enabled as designed.
- [ ] Confirm enemy vision cone detects targets in the forward arc.
- [ ] Confirm enemy alert escalates through noise and direct detection.
- [ ] Confirm combat overlays remain readable during aim and fire.

## 5. Hostage Scenarios

- [ ] Confirm hostage can be freed when in interaction range.
- [ ] Confirm freed hostage follows the active operative.
- [ ] Confirm hostage extraction marks the objective complete.
- [ ] Confirm hostage death marks mission failure.

## 6. Save And Load

- [ ] Confirm save writes the primary snapshot.
- [ ] Confirm autosave triggers on mission state transitions.
- [ ] Confirm backup save is written alongside the primary save.
- [ ] Confirm load restores from a healthy primary save.
- [ ] Confirm load recovers from a corrupted primary save using backup.
- [ ] Confirm save compatibility with current schema version.

## 7. Localization

- [ ] Confirm all player-facing UI text uses keys.
- [ ] Confirm default localization table covers HUD, result screen, warning, and alert keys.
- [ ] Confirm fallback language resolves to English when a target locale entry is missing.
- [ ] Confirm missing keys echo safely instead of breaking UI.
- [ ] Confirm RU text fits within UI constraints.

## 8. Visual Scripting And Runtime Hooks

- [ ] Confirm squad command graphs contain canonical command relays.
- [ ] Confirm enemy alert graphs contain canonical alert relays.
- [ ] Confirm ScriptMachine bridges are attached to the intended prefabs.
- [ ] Confirm graph parity survives scene reload.
- [ ] Confirm command and alert events still match the canonical list.

## 9. Readability And UI

- [ ] Confirm HUD fits on 16:9 and narrower Android-friendly resolutions.
- [ ] Confirm mission result screen wraps cleanly on narrow displays.
- [ ] Confirm tactical overlays do not obscure objective state.
- [ ] Confirm friendly-fire warning remains legible.
- [ ] Confirm result screen title/body/hint remain readable in RU and EN.

## 10. Input

- [ ] Confirm canonical hotkeys work on Windows.
- [ ] Confirm `Tab`, `H`, `F`, `M`, `T`, `E`, and `F1` are recognized by `InputCompat`.
- [ ] Confirm mouse aim and fire interactions work as expected.
- [ ] Confirm keyboard-only navigation is usable where intended.

## 11. Platform Sanity

- [ ] Confirm `PC_Default` quality profile exists and is used correctly.
- [ ] Confirm `Android_Default` quality profile exists and is used correctly.
- [ ] Confirm `Android_Low` quality profile exists and is used correctly.
- [ ] Confirm Windows readability remains acceptable at the default target resolution.
- [ ] Confirm Android readability remains acceptable on low and default profiles.
- [ ] Confirm Android build backend remains IL2CPP and ARM64 as configured.

## 12. Release Verification

- [ ] Run the full EditMode verification suite.
- [ ] Complete `BWF-035` reference integrity sweep.
- [ ] Complete `BWF-036` combat/perception regression sweep.
- [ ] Complete `BWF-037` hostage success/fail scenario sweep.
- [ ] Complete `BWF-038` save/load resilience sweep.
- [ ] Complete `BWF-039` localization coverage sweep.
- [ ] Complete `BWF-040` Android perf/readability quick pass.
- [ ] Complete `BWF-041` Windows readability/input sanity pass.
- [ ] Unblock `BWF-042` v1 candidate baseline.

## 13. Final Acceptance

- [ ] Mission loop starts reliably from bootstrap.
- [ ] Combat, hostage, save, localization, and UI flows are stable.
- [ ] Windows and Android sanity checks pass.
- [ ] No compile errors remain in the project.
- [ ] No critical regressions remain in scenes, prefabs, or graphs.
- [ ] Release candidate can be tagged.
