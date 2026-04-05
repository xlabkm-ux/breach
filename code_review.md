# Code Review and Change Review Guide

## Цель
Этот документ задает единые правила локального review для кода, Unity assets, графов, данных и release-sensitive изменений.

## Критические дефекты
Считать блокирующими:
- сломанный core loop или mission flow
- missing references в сценах и префабах
- invalid graph bindings
- риск повреждения сохранений или отсутствие safe fallback
- schema mismatch без version handling
- поломку localization-safe UI или narrative flow
- нераскрытые изменения `ProjectSettings/` или `Packages/`
- деградацию `PC_Default`, `Android_Default` или `Android_Low`
- нарушение логики friendly fire, ballistic traces, visibility/noise, hostage safety, objective completion

## Что проверять в каждом change set
1. Соответствует ли изменение заявленной цели.
2. Минимальны ли затронутые файлы и assets.
3. Отличимы ли функциональные правки от serialization churn.
4. Есть ли понятная верификация.
5. Понятны ли остаточные риски.

## Проверка gameplay logic
### Обязательно
- state-flow не содержит тупиков и неожиданных переходов
- objective-loop не теряет валидный прогресс
- pacing role фичи понятен
- reward role фичи понятен
- mission rating не конфликтует с intended player behavior

### Для текущего проекта
- приказы отряду читаемы и воспроизводимы
- выбор между stealth / assault / hybrid остается валидным
- cover, line of fire и опасные зоны читаются визуально
- friendly fire и hostage penalty работают предсказуемо
- ранение и смерть бойцов корректно влияют на миссию

## Проверка Unity asset changes
### Сцены
- нет missing references
- изменены только целевые объекты
- нет случайного churn
- mission flow остается рабочим

### Префабы
- nested prefab integrity сохранена
- overrides понятны
- ссылки не потеряны

### Visual Scripting graphs
- bindings валидны
- execution flow читаем
- reaction density не превышена
- повторяемая логика не расползлась по графу

### ScriptableObjects и данные
- схема данных стабильна
- breaking changes отмечены
- save compatibility оценена
- meta progression не смешана с run state без причины

## Проверка UI и контента
- UI использует string keys
- есть fallback language path
- нет переполнения, обрезки и сломанной навигации
- critical feedback legible under all active quality profiles
- narrative templates остаются короткими, reusable и localization-safe

## Проверка ProjectSettings и Packages
Для каждого такого изменения должны быть указаны:
- причина
- затронутые платформы
- ожидаемое изменение поведения
- build impact
- пост-валидация

### Safe-by-default
- build profiles
- quality matrix
- platform overrides
- Android-oriented performance reductions
- basic app metadata

### Требуют отдельного подтверждения
- graphics API
- color space
- scripting backend
- input backend
- permissions
- min/target API
- signing
- physics/audio timing
- stripping
- package/app id
- любые UX/store/compliance affecting settings

## Release-sensitive checks
### Windows 10-11
- input and UI sanity
- readability at supported resolutions
- stable mission flow

### Android
- performance sanity on `Android_Default` and `Android_Low`
- readable UI under scaled layouts
- no catastrophic memory/performance regressions

### Save and localization
- local save works
- autosave triggers still valid
- versioning/fallback path intact
- string keys cover new UI and narrative content

## Review output format
Для каждого review давайте:
1. Summary
2. High-severity findings
3. Medium findings
4. Verification gaps
5. Residual risks
