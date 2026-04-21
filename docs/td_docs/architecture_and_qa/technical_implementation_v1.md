# TACTICAL BREACH: Tech Implementation v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** `docs/td_docs/architecture_and_qa/technical_implementation_v1.md`

## 1. Overview
- **Architecture:** ScriptableObject-based State Machine for mission phases.
- **Command Pattern:** Decouples UI from logic using an Event Bus for scalability.
- **AI Perception:** FOV raycasts to Head, Center, and Feet using LayerMasks (Walls/Obstacles vs. Glass).
- **Cover System:** Manual markers (CoverPoint) with Dot Product checks to determine protection effectiveness. Integrated with IK and Animator for auto-snap and crouching.
- **Save/Load:** JsonUtility for serialization of unit state, HP, and mission progress.


## 2. Interface Definitions (Базовые контракты)

### Укрытия: `ICoverPoint`
Каждое укрытие на уровне должно реализовывать этот интерфейс для оценки его безопасности относительно угла обстрела.
```csharp
public interface ICoverPoint {
    Vector3 Position { get; }
    Vector3 Forward { get; } // Нормаль укрытия (куда смотрит спина укрывающегося)
    CoverTier Tier { get; } // Hard или Soft

    // Возвращает степень защиты [0.0 ... 1.0] от передаваемой позиции источника угрозы
    float EvaluateSafety(Vector3 threatPosition);
}
```
*   **Математика (Dot Product):** `Safety = Vector3.Dot(Forward, (threatPosition - Position).normalized)`. Если значение приближается к `1` (угроза прямо перед укрытием), укрытие максимально безопасно. Если значение падает ниже `0.3`, укрытие считается отфлангованным (Flanked), и ИИ обязан искать новое.

### Восприятие: `IAIPerception`
Интерфейс для всех сенсоров (зрение, слух) врагов.
```csharp
public interface IAIPerception {
    bool CanSee(Transform target); 
    void OnHearSound(Vector3 origin, float noiseRadius, NoiseType type);
}
```
*   Метод `CanSee` должен обязательно использовать метод "3 лучей" (Голова -> Глаза, Голова -> Торс, Голова -> Ноги).

## 3. Save/Load Architecting
Все состояния сериализуются через `JsonUtility`.
*   Архитектура должна строго различать мету (прогрессия между миссиями) и состояние текущей сессии (координаты, ХП оперативников).
*   В случае несовпадения версий схемы сохранения (например, после патча), сессионный сейв должен безопасно пробрасывать (fallback) игрока на этап перепланировки (Setup Phase).
