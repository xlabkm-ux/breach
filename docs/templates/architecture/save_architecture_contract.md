# TACTICAL BREACH: Save Architecture Contract v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0
> **Status:** Template / Contract
> **Path:** $relPath

## Objectives
- local reliability
- predictable autosave
- separation of run state and meta progression
- recoverability on incompatible or corrupt data

## Required behavior
- local save is mandatory
- autosave triggers on key state transitions
- meta progression persists separately from mission/run session state
- schema versioning is mandatory
- incompatible/corrupt data must enter a safe fallback path

## Suggested top-level model
- `SaveHeader`
- `ProfileMetaProgression`
- `CurrentRunState`
- `MissionCheckpointState`
- `SettingsState`

## Versioning rules
- every persisted root carries schema version
- migrations are explicit
- if migration fails, preserve backup and fall back safely

## Autosave triggers
- mission start confirmed
- objective milestone completed
- extraction reached
- safe menu/upgrade transition
- meta progression unlock granted

## Never silently mix
- profile progression and transient mission state
- old schema payloads and new runtime assumptions

## Validation checklist
- compatibility checked after any data contract change
- autosave triggers reviewed after state-flow changes
- fallback path tested for corrupt/incompatible payloads

