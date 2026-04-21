# TACTICAL BREACH: State Flow Template

> **Project:** TACTICAL BREACH
> **Version:** 1.0
> **Status:** Template / Contract
> **Path:** $relPath

## System / Feature

## States
| State | Description | Entry condition | Exit condition |
|---|---|---|---|
| Idle |  |  |  |
| Setup |  |  |  |
| Active |  |  |  |
| Success |  |  |  |
| Failure |  |  |  |

## Allowed transitions
- Idle -> Setup
- Setup -> Active
- Active -> Success
- Active -> Failure

## Forbidden transitions
- Success -> Active
- Failure -> Active without reset

## Transition events
- event name
- source state
- target state
- validation rules

