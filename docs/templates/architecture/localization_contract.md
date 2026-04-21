# TACTICAL BREACH: Localization Contract v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0
> **Status:** Template / Contract
> **Path:** $relPath

## Core rules
- all UI text uses string keys
- all narrative text uses string keys
- fallback language is mandatory
- new screens are localization-safe by default
- variables and line wrapping are accounted for from day one

## Required coverage
- HUD
- mission briefing/debriefing
- objective tracker
- warnings and tactical notifications
- equipment/loadout UI
- squad upgrade UI

## Technical requirements
- no gameplay logic depends on literal user-facing strings
- key naming scheme must be stable and searchable
- dynamic values must use parameterized localization entries
- fonts must support fallback path for future expansion

## Validation checklist
- new keys added
- fallback entries added
- no clipped critical UI
- no hardcoded strings in graphs or gameplay scripts unless explicitly justified

