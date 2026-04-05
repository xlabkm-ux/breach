# Rescue Slice 01 - Unity Implementation Plan

## 1) Goal
Implement the first vertical slice: `Rescue Slice 01 - Single Hostage Apartment` as a playable mission-complete loop (stealth-capable + assault-capable) with minimal, testable architecture based on:
- state-flow
- objective-loop
- pacing
- reward templates

## 2) Plan
We execute in 23 small steps. Each step has:
- scoped implementation change
- verification gate
- git commit only after gate passes

## 3) Implementation Packet

### A. Feature breakdown
1. Mission state-flow backbone
2. Objective-loop backbone
3. Two-operator control model
4. Command system (move/hold/follow/attack-target)
5. Combat baseline (shooting/hit/damage/death)
6. Enemy perception (vision cone + noise)
7. Alert FSM (idle -> suspicious -> alert)
8. Hostage interaction (free/follow)
9. Extraction and mission result
10. Minimal save hooks
11. Localization-safe UI keys

### B. System contracts
- MissionStateService: authoritative mission phase transitions
- ObjectiveService: objective flags and completion/fail evaluation
- SquadCommandService: command dispatch and acknowledgment
- CombatResolver: hit validation, damage routing, friendly fire
- PerceptionService: vision/noise inputs to alert state outputs
- HostageService: captive/freed/following/extracted/dead state
- ResultService: success/fail contract and summary payload
- SaveService: versioned mission snapshot and restore
- LocalizationFacade: key-based UI text resolution with fallback

### C. Required Unity objects
- MissionDirector
- SquadManager
- 2x OperativeController
- 2-4x EnemyController
- HostageController
- ExtractionZoneTrigger
- 2x EntryPoint
- NoiseEmitter
- MissionUIRoot
- SaveBootstrap

### D. Required assets
Scenes:
- `Assets/Scenes/VerticalSlice/VS01_Rescue.unity`
- `Assets/Scenes/Bootstrap/Bootstrap.unity`

Prefabs:
- `Assets/Prefabs/Gameplay/Operatives/Operative_Player_A.prefab`
- `Assets/Prefabs/Gameplay/Operatives/Operative_Player_B.prefab`
- `Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab`
- `Assets/Prefabs/Gameplay/Hostage/Hostage_Civilian.prefab`
- `Assets/Prefabs/Gameplay/Zones/ExtractionZone.prefab`
- `Assets/Prefabs/Gameplay/Mission/MissionDirector.prefab`
- `Assets/Prefabs/UI/MissionUI.prefab`

ScriptableObjects:
- `Assets/Data/Mission/MissionConfig_VS01.asset`
- `Assets/Data/Combat/WeaponConfig_Rifle.asset`
- `Assets/Data/Combat/DamageRules.asset`
- `Assets/Data/AI/PerceptionConfig.asset`
- `Assets/Data/Localization/LocalizationTableRefs.asset`
- `Assets/Data/Save/SaveSchemaConfig.asset`

Graphs:
- `Assets/VisualScripting/Mission/MissionFlow.asset`
- `Assets/VisualScripting/Squad/SquadCommandFlow.asset`
- `Assets/VisualScripting/AI/EnemyAlertFlow.asset`
- `Assets/VisualScripting/Hostage/HostageFlow.asset`
- `Assets/VisualScripting/UI/ResultScreenFlow.asset`

### E. C# vs Visual Scripting split
C#:
- reusable systems and data contracts
- combat/perception math and validation
- save/versioning logic
- localization key resolution facade

Visual Scripting:
- orchestration and state transitions
- mission interaction flow
- command confirmation and UI reactions
- mission result screen flow

### F. Acceptance criteria
- stealth-capable mission completion path
- assault-capable mission completion path
- active operative switching works
- move/hold/follow/attack-target commands work
- shooting/damage/death work for all actors
- enemy vision cone detection works
- gunshot noise escalates alert
- friendly fire applies correctly
- extraction succeeds only with valid objective state
- fail on hostage death
- fail on whole squad death
- result UI is key-based (localization-safe)
- minimal save/load hooks restore valid state

### G. Likely regression zones
- mission state transition race conditions
- scene/prefab reference loss
- perception and alert state desync
- hostage follow/extract synchronization
- friendly fire filtering mistakes
- save schema drift without fallback
- UI state desync from objective state

### H. Serialization-risk notes
- scene/prefab YAML churn during bulk edits
- Visual Scripting graph noise masking functional changes
- serialized field additions without default compatibility
- ScriptableObject schema changes without version handling

### I. Recommended implementation order (23 steps)
1. Create vertical-slice folder structure under `Assets/`.
2. Create `VS01_Rescue.unity` scene shell.
3. Create `Bootstrap.unity` scene shell.
4. Add mission geometry/layout placeholders (apartment + 2 entries + extraction + hostage room).
5. Add `MissionDirector` GameObject and baseline mission state enum wiring.
6. Implement C# `MissionStateService` + compile pass.
7. Implement C# `ObjectiveService` + objective flags.
8. Add two operative prefabs and active-operative switch.
9. Implement `SquadCommandService` (move/hold/follow).
10. Extend commands with `attack-target`.
11. Implement combat baseline (`CombatResolver`, `HealthComponent`, death flow).
12. Add friendly fire validation and tests for same-team damage rules.
13. Add enemy prefab baseline and navigation behavior.
14. Implement perception vision-cone checks.
15. Implement gunshot noise emitter/listener pathway.
16. Implement alert FSM (`idle -> suspicious -> alert`) integration.
17. Add hostage prefab and free/follow interaction.
18. Add extraction trigger and success/fail resolver.
19. Add result screen UI prefab and key-based string bindings.
20. Implement minimal save schema + autosave on key mission transitions.
21. Implement load/restore path for slice-critical state.
22. Run full verification matrix (console, references, play scenarios, save/localization checks).
23. Final cleanup commit for slice baseline + update docs/manifest as needed.

## 4) Verification Plan (per step)
- Unity console: no errors introduced by step
- integrity checks: target scene/prefab refs valid
- flow checks: impacted mission path still playable
- tests: run relevant unit/integration checks where available
- localization/save checks: run when step touches these systems

## 5) Remaining Risks
- AI behavior depth may remain intentionally basic in slice v1
- Without early automated tests, later regressions are likely
- Android readability/performance requires device pass later
- Save schema must stay versioned from first persisted state
