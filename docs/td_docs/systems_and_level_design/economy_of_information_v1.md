# TACTICAL BREACH: Economy of Information

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/systems_and_level_design/economy_of_information_v1.md`

## 1. Overview
- **Fog of War:** Layout is known (blueprints), but dynamic entities (AI/Hostages) stay hidden until spotted or heard.
- **Tool Costs:** Optiwands and drones provide info but cost time and risk exposure.


## 2. Technical Rendering of FoW (Реализация в Unity)
Игрок видит статичную геометрию (стены, двери), но не видит динамические объекты (врагов, заложников, новые ловушки), пока они не попадут в конус зрения (Line of Sight) оперативников или в зону действия девайсов (Optiwand).

### Выбранный технический метод: Mesh Generation / FOV Mesh
- **Алгоритм:** Динамическая генерация 2D-меша на основе Raycast-снимков из глаз каждого оперативника (Field of View Mesh).
- **Маска (Shader):**
  - Объекты врага рендерятся только если они пересекаются с FOV Mesh (с использованием `Stencil Buffer`).
  - Если враг издает сильный шум ВНЕ поля зрения, на "черном" слое FoW рисуется только красная пульсирующая волна.
- **"Последнее известное положение" (Ghosting):** Если враг уходит в туман войны, на его последнем месте оставляется полупрозрачный белый силуэт (Ghost Marker), пока игрок снова не проверит зону.
