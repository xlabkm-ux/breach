# Breach Game Workflows Backlog

Date: 2026-04-06
Project: `BREACH`
Scope: game development backlog organized as workflows instead of a flat step list.

## Goal

Represent the game roadmap as workflows that mirror how the game is actually built:

- project bootstrap
- mission slice gameplay
- visual productionization
- readability and UX
- save/localization hardening
- release verification

This backlog consolidates the legacy planning docs, which are now archived:

- `archive/2026-04-06/vertical_slice_rescue_01_implementation_plan.md`
- `archive/2026-04-06/v1_resource_expansion_steps.md`
- `archive/2026-04-06/v1_steps_24_48_status.md`

## Workflow Map

1. `Project Bootstrap`
2. `Mission Slice Core Loop`
3. `Combat & Tactical Systems`
4. `Mission Actors & Objective Flow`
5. `Player Feedback & UI`
6. `Visual Productionization`
7. `Persistence & Localization`
8. `Release Verification`

## Status Model

- `DONE`
- `PARTIAL`
- `READY FOR VERIFICATION`
- `BLOCKED`
- `PLANNED`

## Workflow 1 - Project Bootstrap

Purpose:

- establish repository, scenes, folders, bootstrap routing, and mission shell

Items:

### BWF-001 - Project folder structure

- Steps: `1`
- Status: `DONE`
- Outcome:
  - vertical slice folders created under `Assets/`

### BWF-002 - Slice scene shell

- Steps: `2`
- Status: `DONE`
- Outcome:
  - `VS01_Rescue.unity` scene shell exists

### BWF-003 - Bootstrap scene shell

- Steps: `3`
- Status: `DONE`
- Outcome:
  - `Bootstrap.unity` scene shell exists

### BWF-004 - Play-to-mission startup

- Steps: `3`, `5`, follow-up runtime bootstrapping work
- Status: `DONE`
- Outcome:
  - Play enters mission flow instead of idle empty setup

## Workflow 2 - Mission Slice Core Loop

Purpose:

- establish the playable rescue loop from infiltration to extraction/fail

Items:

### BWF-005 - Apartment mission layout

- Steps: `4`
- Status: `DONE`
- Outcome:
  - apartment placeholder layout with entries, hostage room, extraction

### BWF-006 - Mission state flow backbone

- Steps: `5`, `6`
- Status: `DONE`
- Outcome:
  - `MissionDirector`
  - `MissionStateService`

### BWF-007 - Objective loop backbone

- Steps: `7`
- Status: `DONE`
- Outcome:
  - objective flags and completion/fail evaluation

### BWF-008 - Mission success/fail resolution

- Steps: `18`
- Status: `DONE`
- Outcome:
  - extraction and fail resolution wired

## Workflow 3 - Combat & Tactical Systems

Purpose:

- deliver squad control, shooting, perception, alert escalation, and friendly-fire behavior

Items:

### BWF-009 - Two-operative control model

- Steps: `8`
- Status: `DONE`
- Outcome:
  - active operative switching

### BWF-010 - Squad command workflow

- Steps: `9`, `10`
- Status: `DONE IN CODE`
- Outcome:
  - C# move/hold/follow/attack-target implemented
  - Visual Scripting relay graphs and ScriptMachine bridges seeded for parity

### BWF-011 - Combat baseline

- Steps: `11`
- Status: `DONE`
- Outcome:
  - shooting, damage, death, hit resolution

### BWF-012 - Friendly fire rules

- Steps: `12`
- Status: `DONE`
- Outcome:
  - friendly fire logic and tests baseline

### BWF-013 - Enemy baseline

- Steps: `13`
- Status: `DONE`
- Outcome:
  - enemy prefab/runtime presence and movement baseline

### BWF-014 - Vision and noise perception

- Steps: `14`, `15`
- Status: `DONE`
- Outcome:
  - vision cone + gunshot noise pathway

### BWF-015 - Alert escalation

- Steps: `16`
- Status: `DONE IN CODE`
- Outcome:
  - runtime alert FSM works in C#
  - Visual Scripting relay graphs and alert-state event bridge seeded

## Workflow 4 - Mission Actors & Objective Flow

Purpose:

- complete hostage-centered mission logic

Items:

### BWF-016 - Hostage interaction flow

- Steps: `17`
- Status: `DONE`
- Outcome:
  - hostage free/follow baseline

### BWF-017 - Extraction gameplay flow

- Steps: `18`
- Status: `DONE`
- Outcome:
  - extraction trigger + success candidate logic

### BWF-018 - Mission critical fail states

- Steps: `7`, `18`, later fixes
- Status: `DONE`
- Outcome:
  - hostage death fail
  - squad wipe fail

## Workflow 5 - Player Feedback & UI

Purpose:

- keep state, danger, and results readable

Items:

### BWF-019 - Result screen baseline

- Steps: `19`
- Status: `DONE`
- Outcome:
  - result screen runtime with key-based strings

### BWF-020 - Runtime HUD and mission readability

- Source steps: runtime follow-up implementation
- Status: `DONE`
- Outcome:
  - debug HUD
  - mission state feedback
  - control hint surface

### BWF-021 - Tactical feedback overlays

- Steps: `31`, `32`, `33`, `34`, `35`
- Status: `DONE`
- Outcome:
  - hit feedback
  - vision cone overlay
  - noise visualization
  - alert labels
  - friendly-fire risk line

### BWF-022 - Result screen art pass

- Steps: `37`
- Status: `DONE`
- Outcome:
  - result screen presentation works with key-based text

## Workflow 6 - Visual Productionization

Purpose:

- replace bare placeholders with production-leaning visual contracts while keeping gameplay stable

Items:

### BWF-023 - Tilemap production pipeline

- Steps: `24`
- Status: `DONE`
- Outcome:
  - `World_Base`, `World_Collision`, `World_Decor`, `World_Interactables`

### BWF-024 - Apartment art pass

- Steps: `25`
- Status: `DONE`
- Outcome:
  - tilemap-driven apartment art baseline

### BWF-025 - Cover authoring pass

- Steps: `26`
- Status: `DONE`
- Outcome:
  - cover markers and deterministic cover metadata baseline

### BWF-026 - Entry/extraction visual language

- Steps: `27`
- Status: `DONE`
- Outcome:
  - readable entry and extraction visuals

### BWF-027 - Character visual prefabs

- Steps: `28`, `29`
- Status: `DONE`
- Outcome:
  - operative and enemy visual bindings

### BWF-028 - Weapon visual binding

- Steps: `30`
- Status: `DONE`
- Outcome:
  - weapon visual baseline and aim/readability integration

### BWF-029 - Hostage visual prefab pass

- Steps: `36`
- Status: `DONE`
- Outcome:
  - hostage visual/runtime prefab stabilization

### BWF-030 - Build-profile visual optimization

- Steps: `40`
- Status: `DONE`
- Outcome:
  - `PC_Default`, `Android_Default`, `Android_Low` effect scaling baseline

## Workflow 7 - Persistence & Localization

Purpose:

- make the slice save-safe and localization-ready by default

Items:

### BWF-031 - Save schema and autosave baseline

- Steps: `20`
- Status: `DONE`
- Outcome:
  - schema versioning baseline
  - autosave on mission state transitions
  - backup recovery path for broken primary save

### BWF-032 - Load/restore baseline

- Steps: `21`
- Status: `DONE`
- Outcome:
  - load path for critical mission state
  - restore from backup when primary save is corrupted

### BWF-033 - Visual save restoration

- Steps: `39`
- Status: `DONE`
- Outcome:
  - restored state rebinds runtime visuals and overlays

### BWF-034 - Localization-safe UI pass

- Steps: `19`, `38`
- Status: `DONE IN CODE`
- Outcome:
  - key-based UI/runtime text
  - RU/EN-ready resolution path
  - table-level fallback to English and key echo on missing entries

## Workflow 8 - Release Verification

Purpose:

- close the slice as a candidate build instead of just a code prototype

Items:

### Automated verification suite

- `Assets/Tests/EditMode/BreachVerificationTests.cs`
- Covers:
  - combat damage and friendly-fire rules
  - enemy perception cone detection
  - hostage free/escort/extraction flow
  - scene/prefab missing references
  - save autosave and backup recovery
  - localization fallback and required HUD/result keys
  - canonical squad command event names
  - mission success/fail resolution chains
  - `PC_Default`, `Android_Default`, `Android_Low` quality profile coverage
  - runtime overlay budget mapping for Windows/Android readability

### BWF-035 - Reference integrity sweep

- Steps: `22`, `41`
- Status: `READY FOR VERIFICATION`
- Assigned: `loginovfedor`
- Needs:
  - scene/prefab validation in Unity

### BWF-036 - Combat and perception regression sweep

- Steps: `42`
- Status: `READY FOR VERIFICATION`
- Assigned: `xlabkm-ux / Codex`
- Covered by:
  - combat resolver tests
  - enemy vision cone tests
  - enemy alert escalation tests

### BWF-037 - Hostage success/fail scenario sweep

- Steps: `43`
- Status: `READY FOR VERIFICATION`
- Assigned: `xlabkm-ux / Codex`
- Covered by:
  - hostage free/escort tests
  - extraction zone tests
  - hostage kill fail tests

### BWF-038 - Save/load resilience sweep

- Steps: `44`
- Status: `READY FOR VERIFICATION`
- Assigned: `xlabkm-ux / Antigravity`

### BWF-039 - Localization coverage sweep

- Steps: `45`
- Status: `READY FOR VERIFICATION`
- Assigned: `Codex`

### BWF-040 - Android perf/readability quick pass

- Steps: `46`
- Status: `READY FOR VERIFICATION`
- Assigned: `loginovfedor`
- Covered by:
  - quality profile checks
  - runtime overlay budget mapping
  - mobile-safe HUD/result width clamping
  - Android IL2CPP/ARM64 release sanity
  - canonical gameplay hotkey support via InputCompat

### BWF-041 - Windows readability/input sanity pass

- Steps: `47`
- Status: `READY FOR VERIFICATION`
- Assigned: `loginovfedor`
- Covered by:
  - quality profile checks
  - overlay readability checks
  - HUD/result panel width clamping
  - canonical gameplay hotkey support via InputCompat

### BWF-042 - v1 candidate baseline

- Steps: `48`
- Status: `BLOCKED`
- Assigned: `Antigravity / Team`
- Blocked by:
  - `BWF-035` through `BWF-041`

## Current Product Read

Already working as a playable mission baseline:

- Play enters mission
- operative movement/switching
- enemy presence
- shooting and damage
- hostage presence and extraction flow
- result state logic
- tactical readability overlays
- save/localization baseline

Still not fully closed:

- full Unity-side verification for release workflow
- final candidate commit after verification sweep

## Release Focus

Next highest-value workflow for the game is:

1. `Workflow 8 - Release Verification`
2. close remaining `PARTIAL` graph-parity items only if they are still part of v1 acceptance

## Done Criteria

The `Breach` workflow backlog reaches current v1 target when:

- `Workflow 1` through `Workflow 7` remain stable
- `Workflow 8` is completed in Unity
- release candidate baseline is committed after successful verification
