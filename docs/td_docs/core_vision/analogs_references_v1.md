# TACTICAL BREACH: Analogs & References

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/core_vision/analogs_references_v1.md`

## 1. Overview
- **Benchmarks:** *Door Kickers* (Perspective/Planning), *Ready or Not* (Atmosphere/ROE/Lethality), *Classic Rainbow Six* (Fragility/Planning).


## 2. Reference Decomposition (Декомпозиция референсов)

### 1. Door Kickers (Top-Down Planning)
*   🟢 **ЧТО МЫ БЕРЕМ:**
    *   Свободное рисование путей.
    *   Прозрачность плана: весь уровень открыт с точки зрения архитектуры.
    *   Синхронизация по Go-кодам ("Крест", "Ждать взрыва").
*   🛑 **ЧТО МЫ КАТЕГОРИЧЕСКИ ОТБРАСЫВАЕМ:**
    *   Чрезмерное микроконтроллирование каждого градуса поворота оперативника (наш ИИ должен автоматически держать опасные углы).
    *   Бесконечное замедление времени ради "идеального" пиксельхантинга.

### 2. Ready or Not / SWAT 4 (Atmosphere & RoE)
*   🟢 **ЧТО МЫ БЕРЕМ:**
    *   Высокое напряжение, тяжеловесность шагов и действий (атмосфера).
    *   Экономика звука и цена ошибки.
    *   Правила применения силы (Rules of Engagement) – сначала кричим, потом стреляем.
*   🛑 **ЧТО МЫ КАТЕГОРИЧЕСКИ ОТБРАСЫВАЕМ:**
    *   Шутер от первого лица (мы остаемся в Top-down изометрии).
    *   Долгую ходьбу по пустым офисам: наша Mission Grammar диктует фатальную концентрацию за 5 минут.
