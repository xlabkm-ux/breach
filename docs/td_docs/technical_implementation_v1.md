# TACTICAL BREACH: Tech Implementation v1

> **Project:** TACTICAL BREACH
> **Version:** 1.0 (Draft)
> **Status:** Pending Expansion
> **Path:** docs/td_docs/technical_implementation_v1.md

## 1. Overview
- **Architecture:** ScriptableObject-based State Machine for mission phases.
- **Command Pattern:** Decouples UI from logic using an Event Bus for scalability.
- **AI Perception:** FOV raycasts to Head, Center, and Feet using LayerMasks (Walls/Obstacles vs. Glass).
- **Cover System:** Manual markers (CoverPoint) with Dot Product checks to determine protection effectiveness. Integrated with IK and Animator for auto-snap and crouching.
- **Save/Load:** JsonUtility for serialization of unit state, HP, and mission progress.


## 2. Proposed Expansions (TODO)
- **Mathematical Boundaries:** Define exact constants, speeds, and radii.
- **Edge Cases:** Document conflicting states and interruption logic.
- **Technical Mapping:** Link real-world rules to C# interfaces (interface IEntity, ScriptableObject).
- **Test Scenarios:** Add Given-When-Then conditions.
