# TACTICAL BREACH: Docs Index

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/core_vision/project_docs_index.md`

## 1. Overview
- **Status:** Draft v2 (Expanded with Systems & LD)
- **Philosophy:** Focus on risk management and command-based CQB in tight architecture where errors are lethal.
- **Structure:**

### 👁️ Core Vision (`/core_vision`)
Основополагающие документы проекта, описывающие суть игры, направление развития и референсы.
- [`project_docs_index.md`](./project_docs_index.md): Текущий документ. Точка входа, описывающая иерархию и навигацию по документации.
- [`gameplay_brief_v1.md`](./gameplay_brief_v1.md): Основной цикл (core-loop), системы и механики шума. Что мы делаем и во что играем.
- [`vision_roadmap_v1.md`](./vision_roadmap_v1.md): Глобальные цели проекта, фазы разработки и долгосрочный роадмап (разрушаемость, ночные миссии).
- [`analogs_references_v1.md`](./analogs_references_v1.md): Отсылки к играм-ориентирам (Door Kickers, Ready or Not), с анализом их сильных сторон.

### 📐 Systems & Level Design (`/systems_and_level_design`)
Правила создания контента, балансировки механик и генерации/проектирования уровней.
- [`combat_readability_guidelines_v1.md`](../systems_and_level_design/combat_readability_guidelines_v1.md): Визуальный язык и правила читаемости боя (цвета трассеров, UI оверлеи подавления и шума).
- [`enemy_archetypes_v1.md`](../systems_and_level_design/enemy_archetypes_v1.md): Роли искусственного интеллекта (Патулирующий, Охранник, Штурмовик) и их базовое поведение.
- [`mission_grammar_v1.md`](../systems_and_level_design/mission_grammar_v1.md): Грамматика, ритм и структура миссий (планирование -> проникновение -> кульминация).
- [`economy_of_information_v1.md`](../systems_and_level_design/economy_of_information_v1.md): Механики работы с информацией. Что игрок знает (чертежи здания), а что должен выяснить через камеры и дроны (Fog of War).
- [`level_design_rules_v1.md`](../systems_and_level_design/level_design_rules_v1.md): Строгие метрики геометрии. Размеры проемов (1.5м), правила расстановки укрытий и "направляющих".

### 🎮 Player Interaction (`/player_interaction`)
Документация, описывающая взаимодействие игрока с системами управления и боевые тактические паттерны.
- [`control_reference_v1.md`](../player_interaction/control_reference_v1.md): Эргономика управления (ПКМ, ЛКМ). Приоритеты контекстных команд для взаимодействия с дверьми и отрядом.
- [`cqb_reference_sheet_v1.md`](../player_interaction/cqb_reference_sheet_v1.md): Правила тактического выживания (поведение в дверях, перекрестное прикрытие, тайминги). Памятка-учебник для игроков и разработчиков ИИ.

### ⚙️ Architecture & QA (`/architecture_and_qa`)
Технические стандарты разработки и жесткие требования к приемке этапов (Vertical Slice).
- [`technical_implementation_v1.md`](../architecture_and_qa/technical_implementation_v1.md): Архитектурные паттерны кода Unity. Использование ScriptableObject, Event Bus, Dot-product алгоритмов для укрытий.
- [`rescue_slice_acceptance_v1.md`](../architecture_and_qa/rescue_slice_acceptance_v1.md): QA-чеклист. Абсолютные, не подлежащие обсуждению требования (Quality Filter) для прохождения этапа Vertical Slice.

## 2. Proposed Expansions (TODO)
- **Mathematical Boundaries:** Define exact constants, speeds, and radii.
- **Edge Cases:** Document conflicting states and interruption logic.
- **Technical Mapping:** Link real-world rules to C# interfaces (interface IEntity, ScriptableObject).
- **Test Scenarios:** Add Given-When-Then conditions.
