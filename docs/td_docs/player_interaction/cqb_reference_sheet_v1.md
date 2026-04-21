# TACTICAL BREACH: CQB Reference Sheet

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/player_interaction/cqb_reference_sheet_v1.md`

## 1. Overview
- **Rules:**
  - **Doughnut:** Avoid standing directly in front of doors (the fatal funnel).
  - **Clearance:** Every entry must be fast.
  - **Cross-cover:** Clean corners, overlap lines of sight.


## 2. Technical Mapping (Реализация Тактики в ИИ)
Реальная тактика спецназа жестко опирается на геометрию. В Unity мы реализуем это через систему точек и лучей.

- **Fatal Funnel ("Мертвая воронка"):** Кодом реализована как `BoxCollider` (Trigger) прямо за дверным проемом. ИИ-враги (Guard/Assaulter) запрограммированы держать эту зону на прицеле (LookAt). Оперативники, находящиеся в этом триггере более `1.5 секунд`, получают +50% к шансу получить критический урон.
- **Stacking (Построение):** Точки сбора перед входом (Stack Points) процедурно генерируются у стен вокруг дверей с отступом в `0.7м` от проема, чтобы находиться ВНЕ Fatal Funnel.
- **Cross-Cover (Перекрестное прикрытие):** При входе по Go-коду "Крест", пути оперативников должны пересекаться. Навигационный граф временно отключает `ObstacleAvoidance` между членами одной команды на 2 секунды.

## 3. Visual Slicing Diagram ("Нарезание Пирога")
Метод медленной зачистки углов без входа в комнату.

```text
    [ UNCLEARED ROOM ]
        Enemy(X)
           \  
            \ (Line of Sight)
             \
-------------+  [DOOR]  +------
    (Wall)   |          |
```
* **Step 1:** Опер стоит за углом.
* **Step 2:** Делает микрошаг влево, открывая 10 градусов обзора.
* В коде: Опер движется по заранее просчитанной кривой Безье вокруг угла, бросая Raycast каждый кадр.
