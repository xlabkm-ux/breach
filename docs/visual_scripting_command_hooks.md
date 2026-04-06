# Visual Scripting Command Hooks (Step 9/10 bridge)

Current C# command system now emits Visual Scripting custom events, so graph orchestration can subscribe without changing runtime command code.

## Emitted events
- `breach.squad.command.move`
- `breach.squad.command.hold`
- `breach.squad.command.follow`
- `breach.squad.command.attack`

Emitter:
- `Assets/Scripts/Squad/SquadCommandService.cs`
- `Assets/Scripts/Squad/SquadCommandVsEvents.cs`

Payload type:
- `SquadCommandPayload`
  - `EventKey`
  - `WorldPosition`
  - `Target`

## Status
- Step 9/10 C# command logic: done.
- Visual Scripting integration hooks: done.
- Actual graph assets (`MissionFlow`, `SquadCommandFlow`, etc.): still pending Unity-side graph authoring.
