# TACTICAL BREACH: Level Design Rules

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/systems_and_level_design/level_design_rules_v1.md`

## 1. Overview
- **Geometry:** Doors must have 1.5m of hard cover on sides for stacking.
- **Markers:** Manual placement of CoverPoints and Safe Breach (C2) points.
- **Layout:** Corridors must have niches or pillars for tactical retreats.


## 2. Core Metrics (Фундаментальные параметры)
- **Базовая Единица (Grid):** `1 Unity Unit = 1 метр`. Эта метрика закреплена во всех коллайдерах и скриптах навигации (NavMesh).
- **Сетка прилегания (Snapping):** `0.5м` для стен и `0.25м` для объектов интерьера.
- **Двери (Fatal Funnel):** Дверные проемы должны обеспечивать **не менее 1.5м** глухого укрытия (Hard Cover) по обе стороны стены от наличника. Дизайн без места для "стека" (Stack) запрещен.

## 3. Geometry & Obstacles (Геометрия уровней)
- **Укрытия (Hard vs. Soft Cover):**
  - *Hard Cover* (бетон, сталь) - 100% останавливающее действие пули, `CoverPoint` блокирует весь конус зрения (FOV) NPC.
  - *Soft Cover* (гипсокартон, дерево) - пробивается пулями (с учетом баллистической потери урона).
- **Ниши и маршруты отступления:** Если длина прямого коридора превышает `10м`, в нем обязана присутствовать ниша/колонна (Fallback Point) для тактического отступления.

## 4. Line of Sight & Lighting (Видимость и Освещение)
- **Точки бросания лучей (Raycast Points):** Видимость ИИ проверяется броском 3-х лучей от "Головы" ИИ в Глаза, Центр Масс и Колени Игрока. Блокинг хотя бы одного луча уменьшает скорость обнаружения (Detection Rate) на 50%.
- **Свет и Тени (Скрытность):** 
  - Нахождение в "полной тени" (`LayerMask = ShadowArea`) срезает конус видимости врагов (Alert Range) в два раза. Тренированный ИИ по-прежнему слышит шаги.
  - Источники света можно выключать (рубильники) или разбивать, меняя `NavMeshModifierVolume` сектора на теневой.
