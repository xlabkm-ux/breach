# unity-feature-implementation

## Purpose
Implement a gameplay or tooling feature in a scoped, reviewable way for the Unity 6 project.

## Inputs
- task goal
- affected systems
- target assets/files
- constraints and risks

## Workflow
1. Restate implementation/review goal.
2. Identify affected systems and regression zones.
3. Decide whether scenes, prefabs, ScriptableObjects, graphs, settings, or packages are touched.
4. Perform the minimal reviewable change or review pass.
5. Run relevant validation.
6. Summarize changed assets, verification, and residual risks.

## Mandatory checks
- Unity console status
- serialization risk
- save/localization impact when applicable
- platform-profile impact when applicable
- distinction between real changes and churn

## Response format
1. goal
2. plan
3. changes made or findings
4. verification performed
5. remaining risks
