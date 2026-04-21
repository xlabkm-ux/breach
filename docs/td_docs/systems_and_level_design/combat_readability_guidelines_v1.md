# TACTICAL BREACH: Combat Readability

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/systems_and_level_design/combat_readability_guidelines_v1.md`

## 1. Overview
- **Visuals:** Tracers (Yellow/White - Team, Red/Orange - Enemy).
- **Suppression:** Screen blur and increased recoil under fire.
- **Audio-Visuals:** Sound ripples for player and enemy noise.


## 2. UI & Feedback Priority Stack (Иерархия отрисовки)
Так как игра полна хаоса, интерфейс должен подавать информацию строго по значимости спасения жизни оперативника. Рендер UI слоев (Z-Index / Canvas Order):

В порядке убывания (от самого верхнего к нижнему визуалу):
1. **Critical Warning (Критическая угроза жизни):** Направленный индикатор входящего урона (Red Flash Screen Edge). Иконка гранаты под ногами.
2. **Friendly Fire Laser:** Процедурный цвет лазерного прицела меняется на **Ярко-Желтый**, если на линии огня находится союзник или заложник.
3. **Tracers (Трассеры):** Летящие пули всегда рисуются поверх персонажей и эффектов. 
   * Трассеры команды – `Белые / Голубоватые`. 
   * Трассеры противника – `Ярко-Красные / Оранжевые`.
4. **Noise Ripples (Волны Шума):** Визуализация шагов или криков (белые 2D волны по полу).
5. **Selection & Command Outlines:** Контуры выделения юнитов (белый `Thin Outline`) и иконки "Hold / Moving".
6. **Suppression FX (Эффект Подавления):** Блюр (Post-Processing Depth of Field) и туннельное зрение. Рисуется за всеми UI-элементами.

## 3. Readability Rules (Правила Читаемости)
- **Цветовое Кодирование:** Запрещено использовать чисто красные цвета в дизайне уровней (деколях стен, лампах), чтобы они не конфликтовали с предупреждениями об уроне и вражескими трассерами.
- **Силуэты (Silhouettes):** Противник должен моментально идентифицироваться по контуру. Тяжелая броня = объемный силуэт; Заложник = безоружный / прижатый к полу силуэт.
