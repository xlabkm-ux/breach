# TACTICAL BREACH: Gameplay Brief v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/core_vision/gameplay_brief_v1.md`

## 1. Overview
- **Summary:** Turning chaotic shootouts into controlled, planned operations. Position and geometry win fights, not click speed.
- **Pillars:** Lethality (1-2 hits kill), Information Superiority (scouting is key), Methodical approach, Noise/Chaos management (actions have sound signatures).
- **Systems:**
  - **Breach Points:** Choose between main doors (high risk), windows, or back entrances.
  - **Noise Discipline:** Quiet (2m radius), Run/Kick (10m), Unsuppressed/C2 (20m+). Sound is visualised as ripples.
  - **AI States:** Idle (Peaceful), Suspicious (Searching), Alert/Combat (Deadly, seeking cover).


## 2. Metrics & Parameters (Балансные Константы)

### Перемещение (Movement Speeds)
Базовая скорость оперативника (1.0 = X м/с, настраивается в CharacterMotor).
- **Quiet Walk (Скрытный шаг):** `0.5x` от базовой скорости. Положение: пригнувшись (Crouch).
- **Tactical Walk (Штурмовой шаг):** `1.0x` базовая скорость в режиме Weapon Up (Оружие наготове).
- **Run / Sprint (Бег):** `1.5x`. Оружие опущено. Невозможно стрелять быстро.

### Дисциплина Шума (Noise Radii)
В игре шум является 3D Сферой триггером (SphereOverlap), мгновенно привлекающим внимание ИИ.
- **Тихий шаг (`action="walk_quiet"`):** Радиус шума: `2 метра`. Враги слышат только вплотную.
- **Штурмовой шаг / Перезарядка:** Радиус шума: `5 метров`. Достаточно, чтобы насторожить охрану из соседней комнаты с открытой дверью.
- **Бег / Выбивание двери ногой (Kick):** Радиус шума: `10 метров`. Активирует триггеры "Suspicious" через стену.
- **C2 Charge / Неглушимый выстрел:** Радиус шума: `25+ метров`. Глобальный триггер अलर्ट`Alert` (Бой) для всех ИИ в зоне покрытия.

## 3. Lethality & Information (Летальность)
- **Time to Kill (TTK):** 1 выстрел в голову = смерть. 1-2 выстрела в торс (в зависимости от наличия бронежилета) = смерть. Летальность симметрична (враги так же хрупки, как и оперативники).
- **Information Overkill:** Выстрелы "через дверь" вслепую (Wallbang) возможны, но штрафуются непредсказуемостью (риск попасть в заложника). Разведка камерой (Optiwand) стоит времени, но снижает риск до нуля.
