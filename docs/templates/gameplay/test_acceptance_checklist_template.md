# TACTICAL BREACH: Test and Acceptance Checklist Template

> **Project:** TACTICAL BREACH
> **Version:** 1.0
> **Status:** Template / Contract
> **Path:** $relPath

## Compile / boot
- [ ] Project compiles
- [ ] No new blocking console errors

## Gameplay sanity
- [ ] Intended state-flow works
- [ ] Objective progression works
- [ ] Success/fail states work

## Tactical readability
- [ ] Cover and line-of-fire remain readable
- [ ] Detection/noise behavior is understandable
- [ ] Order feedback is legible

## Save / progression
- [ ] Save schema impact reviewed
- [ ] Autosave triggers valid
- [ ] Meta progression separation preserved
- [ ] Fallback path defined if needed

## Localization / UI
- [ ] New UI text uses string keys
- [ ] Narrative text uses string keys
- [ ] Fallback language path exists
- [ ] No critical overflow or wrapping issues

## Platform profiles
- [ ] `PC_Default` checked if impacted
- [ ] `Android_Default` checked if impacted
- [ ] `Android_Low` checked if impacted

## Asset integrity
- [ ] Scene references valid
- [ ] Prefab references valid
- [ ] Graph bindings valid
- [ ] ScriptableObject schema stable

## Residual risks logged
- [ ] Yes

