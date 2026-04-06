# BREACH Starter Kit

Стартовый комплект рабочих файлов для Unity 6 проекта на базе приложенных документов.

## Что входит
- `AGENTS.md` — основной операционный контракт агента
- `.codex/config.toml` — базовая конфигурация Codex
- `code_review.md` — правила локального review
- `.agents/skills/*/SKILL.md` — стартовые workflow skills
- `gameplay/*.md` — шаблоны pipeline и content contracts
- `architecture/*.md` — save/localization contracts
- `mcp/*` — стартовые prompts, resources, policies и validators
- `docs/project_brief_v1.md` — консолидированный brief игры
- `docs/file_manifest.md` — список артефактов и назначение

## Рекомендуемый порядок внедрения
1. Перенести файлы в корень репозитория.
2. Сверить `AGENTS.md` и `.codex/config.toml` с реальной структурой проекта.
3. Подключить `.agents/skills/` и `mcp/` к вашему локальному workflow.
4. Использовать шаблоны из `gameplay/` для каждой новой фичи.
5. Зафиксировать первые правки одним reviewable change set.
