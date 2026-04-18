# Visual Scripting Command Hooks (Step 9/10 bridge)

Current C# command system now emits Visual Scripting custom events, so graph orchestration can subscribe without changing runtime command code.

## Emitted events
- `TACTICAL BREACH.squad.command.move`
- `TACTICAL BREACH.squad.command.hold`
- `TACTICAL BREACH.squad.command.follow`
- `TACTICAL BREACH.squad.command.attack`

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
- Canonical graph assets are now seeded by editor bootstrap for squad command and enemy alert parity.
- `MissionDirector` and `Enemy_Grunt` receive `ScriptMachine` bridges through the bootstrap generator.
