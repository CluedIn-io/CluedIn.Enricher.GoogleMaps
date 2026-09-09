# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.GoogleMaps` from a single-version build to
the multi-version targeting pattern. It is kept up to date as work lands (not written after the
fact), so it can double as a reference for migrating the remaining repos.

Prior art consulted before starting this migration:
- `CluedIn.Crawling.MasterDataServices/docs/multi-version-targeting-migration.md`
- `CluedIn.Connector.AzureEventHubs/docs/multi-version-targeting-migration.md`
- `CluedIn.Connector.AzureDataLake` and `CluedIn.Mcp` (completed, but no migration doc — inspected
  `azure-pipelines.yml` / `Directory.Build.props` / `Packages.props` directly)
- `AzurePipelines.Templates/crawler.build.jobs.yml` and `common/detect-tfm.yml` directly (checked
  out locally at `D:\Source\CluedIn\AzurePipelines.Templates`, branch
  `feature/multi-version-packaging`) — **the authoritative source**, since the two migration docs
  above turned out to be stale on the `multiVersionCluedInTargets` schema (see Step 1).
- `CluedIn.Enricher.RestApi` — turned out to *not* be migrated yet either (same unmigrated shape as
  this repo), despite being referenced in the template's own comments as an example of the calling
  convention. Not a usable reference; ignore it until it's migrated too.

---

## Overview

The goal is to produce separate NuGet packages per CluedIn version from a single branch, using the
shared `crawler.build.jobs.yml` pipeline template. Each package targets the correct .NET TFM for
that CluedIn generation — auto-detected by the template itself (see `common/detect-tfm.yml`), not
hand-specified in `azure-pipelines.yml`.

| CluedIn version | .NET TFM | Package suffix |
|---|---|---|
| 4.7.0 | net6.0 | `.470` |
| 4.8.0 | net6.0 | `.480` |
| 5.0.0-alpha.* | net10.0 | `.50` |

4.6.0 was deliberately excluded (decision made 2026-09-09) — kept the version window aligned with
`CluedIn.Crawling.MasterDataServices` rather than the wider `CluedIn.Connector.AzureEventHubs`
window (4.6.0+), since there's no known 4.6-only dependency in this enricher's small API surface.

Branch: `feature/multi-version-targeting` (off `develop`).

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Done**

Replaced the single-version `crawler.build.yml` steps-template with the multi-version
`crawler.build.jobs.yml` jobs-template.

**Correction to prior docs:** both `CluedIn.Crawling.MasterDataServices` and
`CluedIn.Connector.AzureEventHubs`'s migration docs show `multiVersionCluedInTargets` as a flat
list of version strings. That schema is stale. Verified directly against the live
`crawler.build.jobs.yml` (branch `feature/multi-version-packaging`): it's a list of objects, and
`targetFramework` is **not** a consumed parameter at all — the framework is auto-detected per
target via `common/detect-tfm.yml` (restores a throwaway project against the resolved
`CluedIn.Core` version and inspects its `lib/<tfm>` folder in the NuGet cache). `targetFramework`
does appear in `CluedIn.Connector.AzureDataLake`'s pipeline yaml, but it's a harmless no-op left
over from an older template revision — the current template ignores it.

```yaml
jobs:
  - template: crawler.build.jobs.yml@templates
    parameters:
      pool:
        vmImage: 'ubuntu-22.04'
      ...
      probeCluedInVersion: ${{ parameters.probeCluedInVersion }}
      multiVersionCluedInTargets:
        - cluedInVersion: '4.7.0'
        - cluedInVersion: '4.8.0'
        - cluedInVersion: '5.0.0-alpha.*'
```

Also added the `probeCluedInVersion` pipeline parameter (one-off check against a version outside
the matrix), switched `pipelineTemplateRef` default to `refs/heads/feature/multi-version-packaging`
(matching AzureDataLake/Mcp), dropped the top-level `pool:`/explicit `UseDotNet@2` step (each job
in the jobs-template declares/installs its own), and moved to an ubuntu-22.04 pool per job.

---

## Step 2 — `Directory.Build.props`

Status: **Done**

Honours `CluedInMultiVersionTargetFramework`, falling back to `net10.0` for local dev:

```xml
<TargetFramework Condition="'$(CluedInMultiVersionTargetFramework)' != ''">$(CluedInMultiVersionTargetFramework)</TargetFramework>
<TargetFramework Condition="'$(CluedInMultiVersionTargetFramework)' == ''">net10.0</TargetFramework>
```

**Extra fix found while build-testing (not in either prior doc):** also had to pin
`<LangVersion>13.0</LangVersion>` explicitly. `Constants.cs` uses a raw string literal (C# 11+),
but net6.0's own default LangVersion is C# 10 — without an explicit override, the net6.0 leg fails
with `CS8936`. The SDK installed (10.x) supports compiling C# 13 down to net6.0 fine; this is purely
about the compiler's TFM-implied default, not actual runtime capability.

---

## Step 3 — `Packages.props`

Status: **Done**

Renamed `packages.props` → `Packages.props` (was lowercase; matches the casing `CluedIn.Mcp`/
`CluedIn.Connector.AzureDataLake` use, avoids relying on Windows' case-insensitive filesystem).

- Guarded `_CluedIn` so the pipeline value wins; derived `DefineConstants`
  (`CLUEDIN_V47`/`CLUEDIN_V48`/`CLUEDIN_V50`) from it, stripping the pre-release suffix first
  (same pattern as the MasterDataServices/AzureEventHubs docs).
- Pinned `Microsoft.NET.Test.Sdk` (18.3.0 / 17.12.0) and `xunit.runner.visualstudio` (3.1.5 / 2.8.2)
  conditionally on `CLUEDIN_V50`.
- **Reverted an initial attempt** to tie `CluedIn.Testing.Base` / `CluedIn.CrawlerIntegrationTesting`
  to `$(_CluedIn)` (which is what MasterDataServices does) — see Step 6's "Known limitation" below
  for why that doesn't work here. Both stay hardcoded at `5.0.0-*`, same as before this migration.

---

## Step 4 — Test projects

Status: **Done**

- Stripped `test/Directory.Build.props` down to just `IsTestProject` — no more unconditional
  `PackageReference`s (avoids the CS0433 xunit-v2/v3 clash the MasterDataServices doc describes).
- **Deleted `test/unit/Directory.Build.props`.** It was dead scaffold: pinned ancient `Moq 4.5.30`
  / `Should 1.1.20` with hardcoded versions (bypassing central package management entirely), but no
  `test/unit` csproj exists in this repo to consume it — this enricher only has integration tests.
  Confirmed via `git status`/`find` there was nothing else under `test/unit`.
- Added conditional `ItemGroup`s directly to
  `test/integration/Integration.Tests/ExternalSearch.GoogleMaps.Integration.Tests.csproj`: xunit v3
  + `AutoFixture.Xunit3` under `CLUEDIN_V50`, xunit v2 + `AutoFixture.Xunit2` otherwise (this
  project is the only one that picks up `test/Directory.Build.props` by MSBuild's automatic
  ancestor-directory discovery).
- Added `test/integration/Integration.Tests/GlobalUsings.cs`:
  ```csharp
  #if CLUEDIN_V50
  global using AutoFixture.Xunit3;
  #else
  global using AutoFixture.Xunit2;
  global using Xunit.Abstractions;
  #endif
  ```
  `Xunit.Abstractions` is needed because `GoogleMapsTests.cs` uses `ITestOutputHelper` via a bare
  `using Xunit;` — that type lives in `Xunit` itself under xunit v3, but in `Xunit.Abstractions`
  under xunit v2.

---

## Step 5 — `NuGet.Config` casing

Status: **Done**

`git mv`'d `Nuget.config` → `NuGet.Config` (two-step rename, since Windows is case-insensitive and
a single `git mv` is a no-op). The `.sln` already referenced the correct `NuGet.config` casing, so
no other file needed touching.

**Feed check:** did *not* need to add the extra `Public` feed AzureEventHubs required — this repo's
existing `develop`/`release`/`AzurePipelines` feeds already resolve `CluedIn.Core` /
`CluedIn.Crawling` / `CluedIn.ExternalSearch` at 4.7.0 and 4.8.0 (confirmed via local `dotnet
restore -p:_CluedIn=4.7.0` and `4.8.0`, both succeeded immediately).

---

## Step 6 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-alpha.*

Status: **`src/` done and verified locally; integration tests blocked (see below)**

All builds below were run locally against the real feeds (`dotnet restore`/`dotnet build -p:_CluedIn=<v>
-p:CluedInMultiVersionTargetFramework=<tfm>`), not just reasoned about.

### Finding: RestSharp major-version break (not a CluedIn.Core API break)

`CluedIn.Core`/`CluedIn.Crawling` bring RestSharp in transitively, and the version differs sharply
by CluedIn generation:
- CluedIn 4.7.0/4.8.0 (net6.0) → **RestSharp 106.15.0** (legacy API: `Method.GET` uppercase enum,
  `client.ExecuteAsync<T>()` returns `IRestResponse<T>`)
- CluedIn 5.0.0-alpha.* (net10.0) → **RestSharp 114.0.0** (rewritten API: `Method.Get` PascalCase,
  `ExecuteAsync<T>()` returns `RestResponse<T>` directly, no `IRestResponse<T>` interface)

`GoogleMapsExternalSearchProvider.cs` was written against the newer RestSharp API. Fixed with
`#if CLUEDIN_V50` guards at every divergent call site (9 total: 5× `Method.Get`/`Method.GET`, 4×
explicitly-declared `RestResponse<T>`/`IRestResponse<T>` locals and the
`ConstructVerifyConnectionResponse<T>` parameter type). `var`-inferred response locals (e.g. the
`LocationDetailsResponse` call) needed no guard — the type is inferred correctly either way, only
explicitly-declared-type locals broke.

Verified: both src projects (`ExternalSearch.Providers.GoogleMaps`,
`Provider.ExternalSearch.GoogleMaps`) now build cleanly (0 errors) against all three targets:
4.7.0/net6.0, 4.8.0/net6.0, 5.0.0-alpha.*/net10.0. Full solution build at the local-dev default
(net10.0) also still passes.

### Known limitation: integration tests can't build against the net6.0 legs at all

`CluedIn.Testing.Base` and `CluedIn.CrawlerIntegrationTesting` (used by
`test/integration/Integration.Tests`) were **never published for the 4.x line** — confirmed via
restore: querying `>= 4.7.0` found nothing between roughly 4.0.0/4.5.0 and 5.0.0-alpha in any feed.
Even pinning them to a fixed `5.0.0-*` (their current/original value, reverted to in Step 3) doesn't
help: those packages are **net10.0-only** — `NU1202: Package CluedIn.Testing.Base 5.0.0-alpha.5 is
not compatible with net6.0`. This isn't a version-selection problem, it's a missing TFM entirely.

Practical impact: **none by default.** `runIntegrationTests` defaults to `false`, and the jobs
template's per-target unit-test step only looks at `test/unit` (which doesn't exist in this repo —
see Step 4). Normal CI/PR builds only build+pack `src/` per leg, which works cleanly today. The gap
only surfaces if someone sets `runIntegrationTests: true`, which would then fail to restore
`test/integration` for the 4.7.0/4.8.0 legs specifically. This repo also has no working
`build/integration-test.ps1` setup/teardown script today, so integration tests couldn't practically
run in CI even before this migration.

**Not fixable from this repo.** Multi-targeting `CluedIn.Testing.Base`/`CluedIn.CrawlerIntegrationTesting`
to include net6.0 is a decision for whoever owns those packages (shared test-infra, not this repo).
Until then, leave `runIntegrationTests` at its default `false` for this repo.

---

## Step 7 — Reset the semantic version (`GitVersion.yml`)

Status: **Done**

```yaml
next-version: 1.0
ignore:
  sha: []
  commits-before: 2026-06-18T00:00:00
```

Highest pre-existing `4.x` tag is `4.6.2`, commit dated `2026-06-17T17:25:42+10:00`. This feature
branch's first commit is `2026-09-09T19:27:21+10:00`. `2026-06-18T00:00:00` sits safely between the
two.

---

## Checklist

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-alpha.*) — schema verified against live template
- [x] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework` with net10.0 local fallback; `LangVersion` pinned to 13.0
- [x] `Packages.props` — renamed from lowercase; `_CluedIn` guarded; `DefineConstants` derived; `Microsoft.NET.Test.Sdk` / `xunit.runner.visualstudio` pinned per TFM
- [x] `test/Directory.Build.props` — package refs removed, properties only
- [x] Test csproj (`Integration.Tests`) — conditional xunit v2/v3 + AutoFixture selection; `GlobalUsings.cs` added
- [x] `test/unit/Directory.Build.props` — deleted (was dead scaffold, no project consumed it)
- [x] `NuGet.Config` — renamed from `Nuget.config`; feeds confirmed sufficient for 4.7.0/4.8.0, no extra feed needed
- [x] Source — `#if CLUEDIN_V50` guards added for the RestSharp 106↔114 API break (9 call sites in `GoogleMapsExternalSearchProvider.cs`)
- [x] `GitVersion.yml` — `next-version: 1.0`; `ignore.commits-before: 2026-06-18T00:00:00`
- [x] `src/` builds clean (0 errors) for all three legs, verified locally via real `dotnet restore`/`build`
- [ ] Integration tests — **known gap**, not fixable from this repo (see Step 6); `runIntegrationTests` left at default `false`
- [ ] Push branch and confirm the actual Azure DevOps pipeline run is green end-to-end (local builds don't exercise the jobs-template's pack/publish/GitVersion-tool steps)
