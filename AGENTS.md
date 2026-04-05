# AGENTS.md

## Project identity
This repository contains a commercial Unity 6 game project using:
- URP
- 2D production pipeline with HD-2D visual presentation
- Visual Scripting + C# hybrid architecture
- local Codex workflow via Codex app and Codex CLI
- Unity MCP server for editor-side operations
- Git as the required source-control layer

Primary release targets for v1:
- Windows 10-11
- Android Mobile

Platform quality profiles for v1:
- `PC_Default`
- `Android_Default`
- `Android_Low`

Game profile:
- mixed genre/system composition
- UI-heavy
- Physics-heavy
- Narrative
- Roguelite
- tactical top-down combat with stealth and squad coordination

Core fantasy from current game brief:
- player commands a 2-4 operator special-forces squad
- missions reward planning, synchronized execution, cover use, stealth, and controlled force
- each mission supports stealth, assault, or hybrid resolution

Treat these tags as complementary. Some describe genre, some visual presentation, and some technical/system profile.

---

## Operating mode
This project is developed primarily by a single developer.

Default assumptions:
- work locally
- use Git continuously
- keep all changes reviewable
- use remote Git hosting mainly for backup, history, release tags, and disaster recovery
- do not assume collaborative PR workflow is mandatory for daily development

Even in solo mode, every meaningful change set must remain reviewable and traceable.

---

## Core agent principles
1. Prefer minimal, reviewable, testable changes.
2. Do not perform broad refactors without a plan.
3. Use Unity MCP tools for Unity-editor actions whenever possible instead of treating Unity assets as plain text blobs.
4. Use C# for reusable, performance-critical, validation-heavy, or difficult-to-review logic.
5. Use Visual Scripting for orchestration, state flow, event composition, and rapidly iterated gameplay logic.
6. End every meaningful mutation with verification.
7. Distinguish functional changes from serialization churn.
8. Keep the project localization-ready and save-safe by default.
9. Preserve mission readability: player intent, squad state, objective state, and combat feedback must remain legible.
10. Treat physics, friendly fire, ballistic logic, and mission-state logic as high-risk zones.

---

## What the agent may change
The agent may modify, create, or remove when justified:
- C# scripts
- custom units
- scenes
- prefabs
- ScriptableObjects
- Visual Scripting graphs
- UI configuration and related assets
- tests
- build scripts
- review and support docs
- `ProjectSettings/`
- `Packages/manifest.json` and related package settings

The agent must keep changes scoped, justified, and easy to review.

---

## Plan-before-change rule
Before any significant change, the agent must:
- state the implementation goal briefly
- identify affected systems
- identify whether scenes, prefabs, ScriptableObjects, graphs, `ProjectSettings/`, or packages will be touched
- identify likely regression zones
- call out serialization risk when Unity assets will change

A significant change includes any modification that affects gameplay flow, data layout, build behavior, scene structure, prefab structure, graph flow, project settings, or packages.

---

## Unity workflow policy
When working with Unity content:
- prefer MCP-driven editor operations over manual asset-file editing
- prefer targeted changes over broad scene/prefab churn
- avoid touching unrelated objects in scenes and prefabs
- avoid accidental reserialization noise
- capture which Unity assets were intentionally changed

After meaningful Unity-side changes, the agent must:
- refresh Unity if needed
- inspect Unity console
- run relevant validators
- run relevant tests
- summarize changed assets and residual risks

---

## Repository layout policy
Use the actual repository structure if it already exists.
Do not force a new folder structure when a stable one is already in place.

When the repository layout is unclear, infer it conservatively from the existing project.

Typical areas to reason about:
- runtime gameplay code
- editor tooling
- Visual Scripting graphs
- scenes
- prefabs
- ScriptableObjects
- UI
- localization data
- save/data contracts
- tests
- build/configuration files

If creating new files, prefer consistency with existing naming, placement, and ownership patterns.

---

## Gameplay architecture rules
This project does not use a universal genre framework in v1.
Gameplay design and implementation must revolve around a minimal pipeline built on:
- `state-flow`
- `objective-loop`
- `pacing`
- `reward templates`

When implementing or modifying a feature, map it explicitly to:
1. state transitions
2. objective progression
3. pacing role
4. reward role

Required pacing templates in v1:
- `Safe_Start`
- `Escalation`
- `Intensity_Spike`
- `Short_Recovery`
- `Climax_Final_Push`

Required reward templates in v1:
- `Immediate_Feedback_Reward`
- `Milestone_Reward`
- `Segment_Run_Completion_Reward`
- `Unlock_Meta_Progress_Reward`

Optional roguelite-oriented extension:
- `Choice_Reward`

Do not add complex pacing or reward frameworks unless they are clearly required.

### Mission-layer implications for current game
Default mission design assumptions:
- squad size: 2-4 operators
- command modes: direct control + issued orders
- command primitives: follow, hold position, cover fire, move, interact, grenade-to-point
- mission archetypes: hostage rescue, sabotage, defense, VIP escort
- execution styles: stealth, assault, hybrid
- tactical readability: cover, line of fire, noise, visibility cones, explosive radius, and objective state must be understandable on a top-down 2D battlefield

---

## Visual Scripting and C# split
Use this default split:

Prefer Visual Scripting for:
- orchestration
- event sequencing
- state transitions
- interaction flow
- composition of control/reaction logic
- quick gameplay iteration

Prefer C# for:
- reusable systems
- performance-critical logic
- complex calculations
- data modeling
- save/load logic
- validation-heavy logic
- custom units
- integrations and platform-sensitive systems
- ballistics, penetration, damage model, perception math, and mission scoring

If a graph starts to accumulate repeated logic, opaque flow, or hard-to-review branching, move that logic into C# custom units or core systems.

---

## Graph-semantic layer policy
The agent must treat gameplay graphs semantically, not just as passive assets.

Graphs are expected to encode:
- control flow
- reactions
- visual feedback hooks
- audio feedback hooks
- state-driven transitions

Core semantic reaction patterns supported in v1:
- `hit`
- `heavy_hit`
- `crit`
- `damage_taken`
- `death`
- `pickup`
- `objective_progress`
- `objective_complete`
- `low_health`
- `danger`
- `wave_start`
- `wave_end`
- `run_success`
- `run_fail`
- `suppressed`
- `detected`
- `order_confirmed`
- `hostage_secured`

Reaction-density rule:
- each semantic event gets exactly one primary A/V pattern by default
- at most one enhancer may be added
- do not stack extra reaction effects without explicit justification

When working on graphs, the agent should prefer structured composition over ad hoc graph growth.

---

## Scene and prefab policy
### Scenes
- modify only the target objects and flows
- avoid unrelated asset churn
- verify missing references after edits
- call out high-risk scene edits explicitly

### Prefabs
- prefer minimal changes
- preserve nested prefab integrity
- explain overrides when they matter
- verify reference integrity after edits

### ScriptableObjects
- preserve stable data schemas
- mark breaking changes explicitly
- check save/load compatibility when schema changes

---

## Save architecture policy
Saves are mandatory in v1.

Minimum required save behavior:
- local save
- autosave on key state transitions
- separate meta-progression persistence
- schema versioning
- safe fallback for incompatible or corrupted save data

v1 priority:
- reliability
- predictability
- recoverability

Do not prioritize multi-slot complexity or cloud sync over correctness and stability.

Whenever gameplay data contracts change, evaluate:
- save compatibility
- schema version impact
- autosave trigger validity
- meta-progression separation
- fallback behavior

---

## Localization policy
The project must remain localization-ready from v1 onward.

Rules:
- all UI text must use string keys
- all narrative text must use string keys
- a fallback language must exist
- new screens must be localization-safe
- narrative templates must be localization-safe
- account for line length, wrapping, variables, and font fallback

Do not hardcode user-facing text into gameplay logic, scene logic, or graph logic unless there is a deliberate technical exception.

---

## Achievements policy
Achievements are not a blocking requirement for core release v1.

However:
- event/progression systems must expose integration points for future achievements
- future achievement integration must not require redesigning core gameplay logic
- achievement hooks must not destabilize gameplay if platform achievements are absent

---

## Narrative template policy
Narrative support in v1 should remain compact and reusable.
Do not build an oversized narrative system for first release.

Required narrative template library:
- `Intro_Context`
- `Objective_Brief`
- `Progress_Update`
- `Warning_Critical_State`
- `Encounter_Event_Trigger`
- `Success_Resolution`
- `Failure_Resolution`
- `Unlock_Reward_Line`
- `Optional_Flavor_Lore_Snippet`

Use narrative templates as short reusable beats that support core loop readability across scenes, run segments, and events.

---

## UI and content policy
Because this project is UI-heavy and narrative-aware:
- prioritize readability and hierarchy
- guard against text overflow and localization breakage
- verify input/navigation integrity when UI changes
- ensure gameplay state and UI state stay synchronized
- ensure critical player feedback remains legible under platform quality profiles
- highlight cones of vision, cover value, grenade danger area, and objective status without clutter

---

## Physics-heavy policy
Because this project is physics-heavy:
- avoid silent changes to timing-sensitive systems
- treat physics-related configuration changes as high risk
- validate collisions, triggers, timing, and state transitions after relevant edits
- do not change physics timing or related settings without explicit need
- separately validate ballistic traces, penetration rules, friendly fire, and cover interaction

---

## ProjectSettings and Packages policy
Changes to `ProjectSettings/` and packages are allowed, but controlled.

Every such change must include:
- technical reason
- impacted platforms
- expected behavior change
- build-impact note
- post-change validation summary

### Safe-by-default changes
The agent may perform these when justified without extra confirmation:
- build profiles
- quality matrix
- platform overrides
- Android-oriented performance reductions
- basic app metadata

### Confirm-required changes
These require explicit confirmation before mutation:
- graphics API
- color space
- scripting backend
- input backend
- permissions
- min/target API
- signing
- physics/audio timing
- stripping
- package/app id
- any setting that changes UX, compatibility, store behavior, or compliance behavior

---

## Quality profile policy
v1 quality profiles:
- `PC_Default`
- `Android_Default`
- `Android_Low`

Use them to separate:
- quality levels
- texture/compression strategy
- post FX usage
- performance budgets

When making graphics, quality, or platform changes, specify which profile is affected.

---

## Git workflow rules
Branch naming:
- `feat/<area>-<slug>`
- `fix/<area>-<slug>`
- `refactor/<area>-<slug>`
- `chore/<area>-<slug>`
- `release/<version>`
- `hotfix/<version>-<slug>`

Commit format:
- `type(scope): summary`

Examples:
- `feat(combat): add dash cooldown controller`
- `fix(vsp): restore state transition guard`
- `refactor(save): extract profile serializer`
- `chore(build): update android profile rules`

Even without mandatory PR flow:
- keep change sets reviewable
- prefer local review against base branch before merging to `main`
- create tagged snapshots before release

---

## Review guidelines
When reviewing or summarizing changes, prioritize:
1. gameplay correctness
2. broken references
3. graph-flow regressions
4. save compatibility
5. localization regressions
6. unintended setting/package changes
7. platform-profile regressions
8. serialization noise vs real functional changes

Treat these as high-severity defects:
- broken gameplay progression
- missing references in scenes or prefabs
- invalid graph bindings
- save corruption risk
- schema/version mismatch without fallback
- localization breakage in critical UI or narrative flow
- project setting changes that alter compatibility without disclosure
- Android or PC profile regressions affecting basic usability

---

## Done criteria
A task is not done until all applicable checks are addressed.

Minimum done criteria:
- implementation matches requested goal
- affected systems are identified
- changed files/assets are identified
- Unity console is checked
- relevant tests are run or consciously noted as unavailable
- scene/prefab/graph/scriptable object integrity is checked when applicable
- save/localization impact is checked when applicable
- project settings/package impact is checked when applicable
- residual risks are stated clearly

---

## Verification matrix
### Code changes
- compile status
- relevant tests
- integration risk summary

### Scene changes
- missing references
- target object integrity
- unintended scene churn
- gameplay flow sanity

### Prefab changes
- nested prefab integrity
- overrides sanity
- reference integrity

### Graph changes
- binding validity
- execution flow sanity
- semantic reaction density
- repeated-logic smell

### ScriptableObject/data changes
- schema stability
- save compatibility
- meta-progression partition impact

### UI/content changes
- navigation/input sanity
- string key usage
- fallback language compatibility
- overflow/wrapping risk

### ProjectSettings/package changes
- reason for change
- impacted profiles/platforms
- dependency/build impact
- safe-by-default vs confirm-required classification

### Release-sensitive changes
- Windows 10-11 impact
- Android impact
- active quality profile impact
- save readiness
- localization readiness

---

## Response contract for the agent
For non-trivial work, respond in this order:
1. goal
2. plan
3. changes made
4. verification performed
5. remaining risks

For asset-heavy Unity work, explicitly list:
- changed scenes
- changed prefabs
- changed ScriptableObjects
- changed graphs
- changed settings/packages

Keep explanations concise, but do not omit risk-relevant details.

---

## Release priorities for v1
The first commercial release prioritizes:
- stable core loop
- reliable saves
- localization-ready content architecture
- compact narrative library
- required baseline pacing/reward structure
- readable tactical control of a small squad in top-down 2D combat

Treat these as more important than feature excess or system overexpansion.
