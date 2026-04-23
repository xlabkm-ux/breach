# TACTICAL BREACH AGENTS.md — Правила для всех AI-агентов

## Участники команды
- **xlabkm-ux** (xlabkm@gmail.com) — Owner
- **loginovfedor-spec** (loginovfedor@gmail.com) — Maintain

## AI-агенты
- **Antigravity** (Google) — Основной агент на стороне xlabkm (Mission Control, Code, Tests, Localization)
- **Codex** (OpenAI) — Основной агент на стороне loginovfedor (cloud / CLI)
- Каждый пользователь может запускать 2–3 агента одновременно

## Обязательные правила для ВСЕХ агентов (Antigravity + Codex)

### 1. Ветки
- Работать **только** в feature-ветках
- Префиксы:
  - `xlabkm/task-...` или `xlabkm/`
  - `fedor/task-...` или `fedor/`
  - `codex/`
  - `antigravity/`
- Перед началом:  
  `git pull origin main && git rebase main`

### 2. Задачи
- Брать задачи **только** из GitHub Project «Product Backlog» или Issues с label `ready`
- В PR обязательно указывать ссылку на задачу (#123)

### 3. Коммиты и PR
- Использовать **Conventional Commits**
- После изменений агент **обязан** открыть **Draft Pull Request**
- Название PR: `[Antigravity] / [Codex] Название задачи (#123)`

### 4. Работа на разных компьютерах
- Использовать `sparse-checkout` для нужных папок
- Агенты работают **только** внутри активных папок

### 5. Финальное ревью и слияние (Merge)
- Одобрение и слияние (merge) Pull Request'ов теперь делегируется агенту **Antigravity**.
- Обязательное финальное ревью человеком отменено.

### 6. Рабочие пространства (Workspaces)
- Рабочее пространство (Git Worktree):
  - **Antigravity**: работает в основной папке `E:\Games\Breach\BREACH`
- Синхронизация между пространствами происходит исключительно через `git push` / `git pull`.

# TACTICAL BREACH AGENTS.md

## Project Identity
This is a commercial Unity 6 project using:
- URP
- 2D production pipeline with HD-2D presentation
- Visual Scripting + C# hybrid architecture
- local Antigravity / Codex workflow
- Git as the source-control layer

v1 targets:
- Windows 10-11
- Android Mobile

v1 quality profiles:
- `PC_Default`
- `Android_Default`
- `Android_Low`

Game profile:
- mixed genre/system composition
- UI-heavy
- physics-heavy
- narrative
- roguelite
- tactical top-down combat with stealth and squad coordination

Core fantasy:
- player commands a 2-4 operator special-forces squad
- missions reward planning, synchronized execution, cover use, stealth, and controlled force
- each mission supports stealth, assault, or hybrid resolution

## Operating Mode
This project is primarily solo-developed.

Default assumptions:
- work locally
- use Git continuously
- keep changes reviewable and traceable
- use remote hosting mainly for backup, history, release tags, and disaster recovery
- do not assume collaborative PR flow is mandatory for daily development

## Core Principles
1. Prefer minimal, reviewable, testable changes.
2. Do not broad-refactor without a plan.
3. Use C# for reusable, performance-critical, validation-heavy, or hard-to-review logic.
4. Use Visual Scripting for orchestration, state flow, event composition, and fast iteration.
5. End every meaningful mutation with verification.
6. Distinguish functional change from serialization churn.
7. Keep the project localization-ready and save-safe by default.
8. Preserve mission readability: player intent, squad state, objective state, and combat feedback must stay legible.
9. Treat physics, friendly fire, ballistic logic, and mission-state logic as high risk.

## Mission Standards (ОБЯЗАТЕЛЬНО)

**Файл:** `E:\Games\Breach\BREACH\MISSION_STANDARDS.md`

Этот документ является **обязательным** стандартом для всех агентов при работе с любой из следующих областей:
- процедурная генерация уровней (комнаты, стены, двери, окна)
- расстановка объектов в сцене (мебель, укрытия, маркеры)
- настройка персонажей (PPU, NavMesh, спрайты, слои)
- импорт и настройка ассетов (текстуры, тайлы, пивот)

### Правила использования

**Перед реализацией любой новой фичи из перечисленных областей агент обязан:**
1. Прочитать `MISSION_STANDARDS.md` и убедиться, что реализация соответствует зафиксированным стандартам.
2. При добавлении новых правил или изменении существующих — **обновить** соответствующий раздел документа.

**После завершения реализации агент обязан:**
1. Проверить, не нарушает ли внесённое изменение ни один из стандартов.
2. Если стандарт был расширен или изменён — задокументировать это в `MISSION_STANDARDS.md` с описанием причины.

### Структура документа (краткое содержание)

| Раздел | Что фиксирует |
| :--- | :--- |
| **1. Структурные стандарты** | Толщина стен, Z-Sorting, коллизии, двери, окна |
| **2. Размещение объектов** | Мебель, укрытия, безопасные отступы от дверей |
| **3. Настройка персонажей** | PPU, NavMeshAgent параметры, маркеры |
| **4. Стандарты импорта ассетов** | Pivot, текстуры, фильтрация |
| **5. Процесс верификации** | Запуск `Day1Verification`, правила очистки сцены |

### Жёсткие правила (нельзя нарушать без обновления документа)

- **PPU = 128** для всех спрайтов и тайлов.
- **Z-Sorting**: Пол=0, Стены=1, Объекты=2, Персонажи=3.
- **NavMeshAgent.Radius = 0.4–0.5** (не больше).
- **Pivot = Center (0.5, 0.5)** для всех спрайтов.
- **Верификация** через `Day1Verification` обязательна после изменений генератора.
- **Очистка сцены** перед каждой генерацией обязательна.


## Change Scope
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

Keep changes scoped, justified, and easy to review.

## Plan Before Change
Before any significant change:
- state the implementation goal
- identify affected systems
- identify whether scenes, prefabs, ScriptableObjects, graphs, `ProjectSettings/`, or packages are touched
- identify likely regression zones
- call out serialization risk when Unity assets change

A significant change affects gameplay flow, data layout, build behavior, scene structure, prefab structure, graph flow, project settings, or packages.

## Unity Workflow
When working with Unity content:
- prefer MCP-driven editor operations over manual asset editing
- prefer targeted changes over broad scene/prefab churn
- avoid touching unrelated objects in scenes and prefabs
- avoid accidental reserialization noise
- record which Unity assets were intentionally changed

After meaningful Unity-side changes:
- refresh Unity if needed
- inspect Unity console
- run relevant validators
- run relevant tests
- summarize changed assets and residual risks

## Repository Layout
Use the actual repository structure if it already exists.
Do not force a new folder structure when a stable one is already in place.

If layout is unclear, infer it conservatively from the existing project.

Typical areas:
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

If creating new files, keep naming, placement, and ownership consistent.

## Gameplay Architecture
v1 uses a minimal pipeline built on:
- `state-flow`
- `objective-loop`
- `pacing`
- `reward templates`

When implementing or modifying a feature, map it to:
1. state transitions
2. objective progression
3. pacing role
4. reward role

Required pacing templates:
- `Safe_Start`
- `Escalation`
- `Intensity_Spike`
- `Short_Recovery`
- `Climax_Final_Push`

Required reward templates:
- `Immediate_Feedback_Reward`
- `Milestone_Reward`
- `Segment_Run_Completion_Reward`
- `Unlock_Meta_Progress_Reward`

Optional roguelite extension:
- `Choice_Reward`

Do not add complex pacing or reward frameworks unless clearly required.

Mission assumptions:
- squad size: 2-4 operators
- command modes: direct control + issued orders
- command primitives: follow, hold position, cover fire, move, interact, grenade-to-point
- mission archetypes: hostage rescue, sabotage, defense, VIP escort
- execution styles: stealth, assault, hybrid
- tactical readability: cover, line of fire, noise, visibility cones, explosive radius, and objective state must be understandable on a top-down 2D battlefield

## Visual Scripting and C# Split
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

If a graph accumulates repeated logic, opaque flow, or hard-to-review branching, move that logic into C# custom units or core systems.

## Graph-Semantic Policy
Treat gameplay graphs semantically, not as passive assets.

Graphs are expected to encode:
- control flow
- reactions
- visual feedback hooks
- audio feedback hooks
- state-driven transitions

Core semantic reaction patterns:
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

Prefer structured composition over ad hoc graph growth.

## Scenes, Prefabs, and Data
Scenes:
- modify only the target objects and flows
- avoid unrelated asset churn
- verify missing references after edits
- call out high-risk scene edits explicitly

Prefabs:
- prefer minimal changes
- preserve nested prefab integrity
- explain overrides when they matter
- verify reference integrity after edits

ScriptableObjects:
- preserve stable data schemas
- mark breaking changes explicitly
- check save/load compatibility when schema changes

## Save Policy
Saves are mandatory in v1.

Required behavior:
- local save
- autosave on key state transitions
- separate meta-progression persistence
- schema versioning
- safe fallback for incompatible or corrupted save data

Priority:
- reliability
- predictability
- recoverability

Do not prioritize multi-slot complexity or cloud sync over correctness and stability.

When gameplay data contracts change, evaluate:
- save compatibility
- schema version impact
- autosave trigger validity
- meta-progression separation
- fallback behavior

## Localization Policy
The project must remain localization-ready from v1 onward.

Rules:
- all UI text uses string keys
- all narrative text uses string keys
- a fallback language exists
- new screens are localization-safe
- narrative templates are localization-safe
- account for line length, wrapping, variables, and font fallback

Do not hardcode user-facing text into gameplay logic, scene logic, or graph logic unless there is a deliberate technical exception.

## Achievements Policy
Achievements are not blocking for v1.

However:
- progression systems must expose integration points for future achievements
- achievement integration must not require redesigning core gameplay logic
- achievement hooks must not destabilize gameplay if platform achievements are absent

## Narrative Policy
Keep narrative support compact and reusable.

Required template library:
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

## UI and Content Policy
Because this project is UI-heavy and narrative-aware:
- prioritize readability and hierarchy
- guard against text overflow and localization breakage
- verify input/navigation integrity when UI changes
- ensure gameplay state and UI state stay synchronized
- keep critical player feedback legible under platform quality profiles
- highlight cones of vision, cover value, grenade danger area, and objective status without clutter

## Physics Policy
Because this project is physics-heavy:
- avoid silent changes to timing-sensitive systems
- treat physics-related configuration changes as high risk
- validate collisions, triggers, timing, and state transitions after relevant edits
- do not change physics timing or related settings without explicit need
- separately validate ballistic traces, penetration rules, friendly fire, and cover interaction

## ProjectSettings and Packages
Changes to `ProjectSettings/` and packages are allowed, but controlled.

Every such change must include:
- technical reason
- impacted platforms
- expected behavior change
- build-impact note
- post-change validation summary

Safe-by-default changes:
- build profiles
- quality matrix
- platform overrides
- Android-oriented performance reductions
- basic app metadata

Confirm-required changes:
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

## Quality Profiles
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

## Git Workflow
Branch naming:
- `feat/<area>-<slug>`
- `fix/<area>-<slug>`
- `refactor/<area>-<slug>`
- `chore/<area>-<slug>`
- `release/<version>`
- `hotfix/<version>-<slug>`

Commit format:
- `type(scope): summary`

Keep change sets reviewable.
Prefer local review against the base branch before merging to `main`.
Create tagged snapshots before release.

## Review Guidelines
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

## Done Criteria
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

## Verification Matrix
Code changes:
- compile status
- relevant tests
- integration risk summary

Scene changes:
- missing references
- target object integrity
- unintended scene churn
- gameplay flow sanity

Prefab changes:
- nested prefab integrity
- overrides sanity
- reference integrity

Graph changes:
- binding validity
- execution flow sanity
- semantic reaction density
- repeated-logic smell

ScriptableObject/data changes:
- schema stability
- save compatibility
- meta-progression partition impact

UI/content changes:
- navigation/input sanity
- string key usage
- fallback language compatibility
- overflow/wrapping risk

ProjectSettings/package changes:
- reason for change
- impacted profiles/platforms
- dependency/build impact
- safe-by-default vs confirm-required classification

Release-sensitive changes:
- Windows 10-11 impact
- Android impact
- active quality profile impact
- save readiness
- localization readiness

## Response Contract
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

## Release Priorities
The first commercial release prioritizes:
- stable core loop
- reliable saves
- localization-ready content architecture
- compact narrative library
- required baseline pacing/reward structure
- readable tactical control of a small squad in top-down 2D combat

Treat these as more important than feature excess or system overexpansion.
