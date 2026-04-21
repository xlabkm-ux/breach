# TACTICAL BREACH: Docs Index

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/project_docs_index.md`

## 1. Overview
- **Status:** Draft v2 (Expanded with Systems & LD)
- **Philosophy:** Focus on risk management and command-based CQB in tight architecture where errors are lethal.
- **Structure:**

### 👁️ Core Vision (`/td_docs/core_vision`)
Основополагающие документы проекта, описывающие суть игры, направление развития и референсы.
- [`gameplay_brief_v1.md`](./td_docs/core_vision/gameplay_brief_v1.md): Основной цикл (core-loop), системы и механики шума. Что мы делаем и во что играем.
- [`vision_roadmap_v1.md`](./td_docs/core_vision/vision_roadmap_v1.md): Глобальные цели проекта, фазы разработки и долгосрочный роадмап (разрушаемость, ночные миссии).
- [`analogs_references_v1.md`](./td_docs/core_vision/analogs_references_v1.md): Отсылки к играм-ориентирам (Door Kickers, Ready or Not), с анализом их сильных сторон.

### 📐 Systems & Level Design (`/td_docs/systems_and_level_design`)
Правила создания контента, балансировки механик и генерации/проектирования уровней.
- [`combat_readability_guidelines_v1.md`](./td_docs/systems_and_level_design/combat_readability_guidelines_v1.md): Визуальный язык и правила читаемости боя.
- [`enemy_archetypes_v1.md`](./td_docs/systems_and_level_design/enemy_archetypes_v1.md): Роли искусственного интеллекта и их базовое поведение.
- [`mission_grammar_v1.md`](./td_docs/systems_and_level_design/mission_grammar_v1.md): Грамматика, ритм и структура миссий.
- [`economy_of_information_v1.md`](./td_docs/systems_and_level_design/economy_of_information_v1.md): Механики работы с информацией.
- [`level_design_rules_v1.md`](./td_docs/systems_and_level_design/level_design_rules_v1.md): Строгие метрики геометрии.
- [`extended_missions_plan_v1.md`](./td_docs/systems_and_level_design/extended_missions_plan_v1.md): Полный план развития миссий.

### 🎮 Player Interaction (`/td_docs/player_interaction`)
Документация, описывающая взаимодействие игрока с системами управления и боевые тактические паттерны.
- [`control_reference_v1.md`](./td_docs/player_interaction/control_reference_v1.md): Эргономика управления (ПКМ, ЛКМ).
- [`cqb_reference_sheet_v1.md`](./td_docs/player_interaction/cqb_reference_sheet_v1.md): Правила тактического выживания.

### ⚙️ Architecture & QA (`/td_docs/architecture_and_qa`)
Технические стандарты разработки и жесткие требования к приемке этапов (Vertical Slice).
- [`technical_implementation_v1.md`](./td_docs/architecture_and_qa/technical_implementation_v1.md): Архитектурные паттерны кода Unity.
- [`rescue_slice_acceptance_v1.md`](./td_docs/architecture_and_qa/rescue_slice_acceptance_v1.md): QA-чеклист. Блокирующие требования для Vertical Slice.

### 📊 Project Management (`/project_management`)
Организационные документы и бэклог.
- [`tactical_breach_game_workflows_backlog.md`](./project_management/tactical_breach_game_workflows_backlog.md): Задачи и текущее состояние проекта.
- [`test_and_release_plan_v1.md`](./project_management/test_and_release_plan_v1.md): План релизов и тест-планы.
- [`tactical_breach_test_plan_checklist.md`](./project_management/tactical_breach_test_plan_checklist.md): Детальные тест-кейсы для QA.

### 📝 Templates & Contracts (`/templates`)
Шаблоны для кодовых контрактов и компонентов (перенесены с корня).
- Шаблоны геймплея: `/templates/gameplay/...`
- Системные контракты: `/templates/architecture/...`

## 2. Proposed Expansions (TODO)
- **Mathematical Boundaries:** Define exact constants, speeds, and radii.
- **Edge Cases:** Document conflicting states and interruption logic.
- **Technical Mapping:** Link real-world rules to C# interfaces (interface IEntity, ScriptableObject).
- **Test Scenarios:** Add Given-When-Then conditions.
