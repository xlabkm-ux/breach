# TACTICAL BREACH: Enemy Archetypes

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/systems_and_level_design/enemy_archetypes_v1.md`

## 1. Overview
- **Roles:**
  - **Patroller:** Moves on waypoints, sounds the alarm.
  - **Guard:** Static, holds narrow sectors.
  - **Anchor:** Protects the hostage, uses them as a live shield.
  - **Assaulter:** Flanks the player aggressively upon alert.


## 2. State Transition Triggers (Переходы состояний)
Враги работают как State Machine (FSM). Переход между их ролями происходит реактивно.

### Таблица реакций:
| Current Role | Trigger Event | New Role | Description |
| :--- | :--- | :--- | :--- |
| **Patroller** | Услышал шаги (`SoundRadius > 0`) | **Suspicious** | Идет к источнику звука, оружие опущено. |
| **Patroller** | Увидел игрока / Выстрел | **Assaulter** | Поднимает тревогу, ищет укрытие, начинает обход (Flank). |
| **Guard** | Услышал шаги / шум двери | **Alert Guard** | Направляет оружие на дверь, не сходит с места. |
| **Anchor** | Игрок вошел в комнату | **Hostage Shield**| Хватает заложника, стреляет только если дистанция сокращается. |
| **Assaulter** | Получил ранение / Потерял LoS | **Defender** | Отступает в ближайшую нишу (Fallback Point), переходит в засаду. |

## 3. Behavior Trees & Interface Mapping
Основное поведение реализуется через `IEnemyAI`.
* Метод `void OnHearSound(Vector3 origin, float volume)` 
  * Если `volume > Threshold`, `ChangeState(AIState.Searching)`
* Метод `void OnSeeTarget(Transform target)`
  * Мгновенный вызов `GlobalAlert.Trigger()`, все `Patroller` в радиусе 15м переключаются на `Assaulter`.
