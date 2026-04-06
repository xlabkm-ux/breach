# MCP Server Gaps (Breach Integration)

## Current status
This project runs against `XLab.UnityMcp.Server` + Unity Editor bridge (`Library/XLabMcpBridge`).

## Confirmed contract mismatches (current)
1. `tests.results` stability:
- Command is implemented and responds.
- In some runs it can return a stale/running snapshot instead of final summary unless polled repeatedly.
- We need deterministic final-state behavior (`completed` with totals) for CI-like gating.

2. Local package hotfix persistence:
- MCP package is referenced from local path in `Packages/manifest.json`.
- Critical bridge fixes may live outside this repository unless mirrored to source package repo.
- Repro on a fresh machine depends on that external folder state.

## Required MCP improvements
1. Extend preflight with `tools.list` runtime probe from live bridge layer, not only server declaration.
2. Improve `tests.results` polling/finalization behavior to guarantee terminal output payloads.
3. Add bridge watchdog + recovery path:
- stale heartbeat detection,
- queue timeout diagnostics,
- optional auto-restart instruction path for editor-side bridge processor.
4. Decide one canonical execution mode for tools:
- either run all tool calls through MCP server (stdio dispatcher),
- or keep bridge parity as source of truth and enforce release/versioning discipline for the local package.

## Impact on steps
- Current gameplay steps are unblocked for Unity mutations and validation.
- Remaining risk is mostly release reproducibility and deterministic test-status reporting.
