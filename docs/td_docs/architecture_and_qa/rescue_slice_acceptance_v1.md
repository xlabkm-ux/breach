# TACTICAL BREACH: Rescue Slice Acceptance v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/architecture_and_qa/rescue_slice_acceptance_v1.md`

## 1. Overview
- **Purpose:** Rigid "Quality Filter" to prevent feature creep.
- **Scope:** 1 mission (Hostage Rescue), 2-4 operators, 2-3 weapons, 5-7 AI enemies.
- **Non-negotiable:** If any checklist item fails (e.g., AI pathing in doors, hostage vulnerability logic), the release is blocked.


## 2. Acceptance Criteria (Given-When-Then)

Для допуска Vertical Slice к релизу должны стабильно проходить следующие тест-кейсы:

### Test 1: Door Funnel Geometry
*   **GIVEN** отряд стоит перед закрытой дверью
*   **WHEN** игрок командует Stack (ЛКМ по стене)
*   **THEN** ИИ автоматически занимает точки уступов (не ближе 0.7м от наличника) И не блокирует линию огня друг другу И не входит в зону Fatal Funnel.

### Test 2: Lethality and Sound
*   **GIVEN** скрытный оперативник и патрулирующий враг за стеной (расстояние = 5м)
*   **WHEN** оперативник использует команду `Kick Door` (Бег)
*   **THEN** враг моментально переходит в состояние Suspicious И поворачивается к источнику звука И укрытие оперативника считается раскрытым.

### Test 3: Hostage Anchor Protocol
*   **GIVEN** враг архетипа `Anchor` находится в комнате с заложником
*   **WHEN** отряд взрывает дверь (C2)
*   **THEN** `Anchor` должен немедленно переместиться за позицию заложника И использовать его как щит И стрелять только при пересечении оперативником минимальной дистанции (2м).

### Test 4: Rescue Verification
*   **GIVEN** все враги в области зачищены
*   **WHEN** игрок взаимодействует с заложником (Follow)
*   **THEN** заложник наследует маршрут движения лидера отряда И игнорирует агрессивные команды Ивентов.
