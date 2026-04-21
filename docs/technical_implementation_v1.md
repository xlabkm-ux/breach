# TACTICAL BREACH Tech Implementation v1
- **Architecture:** ScriptableObject-based State Machine for mission phases.
- **Command Pattern:** Decouples UI from logic using an Event Bus for scalability.
- **AI Perception:** FOV raycasts to Head, Center, and Feet using LayerMasks (Walls/Obstacles vs. Glass).
- **Cover System:** Manual markers (CoverPoint) with Dot Product checks to determine protection effectiveness. Integrated with IK and Animator for auto-snap and crouching.
- **Save/Load:** JsonUtility for serialization of unit state, HP, and mission progress.
