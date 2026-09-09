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
| 5.0.0-beta.* | net10.0 | `.50` |

> **Correction (2026-09-09, after initial merge to CI):** originally targeted `5.0.0-alpha.*`,
> matching what the reference docs used. The CluedIn 5.0 line has since progressed to a beta
> channel — confirmed by restoring a throwaway project: `5.0.0-alpha.*` still resolves (frozen at
> `5.0.0-alpha.695`), but this repo's own floating default `5.0.0-*` (used everywhere else -
> `Packages.props`, other repos' `Directory.Build.props` net10.0 fallback) now resolves to
> `5.0.0-beta.573`, i.e. beta is current and alpha is stale. Updated `multiVersionCluedInTargets` to
> `5.0.0-beta.*` accordingly. **Lesson for migrating the other repos:** don't copy `5.0.0-alpha.*`
> from this doc or the older reference docs verbatim — restore a throwaway project against
> `5.0.0-*` first and use whatever prerelease label that actually resolves to, since the label
> shifts over the CluedIn release cycle (alpha → beta → rc → stable). Re-verified locally that both
> src projects and the integration test project still build clean against `5.0.0-beta.*`/net10.0,
> and that RestSharp still resolves to 114.0.0 there (same as under alpha — the Step 6 guards are
> unaffected).

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
        - cluedInVersion: '5.0.0-beta.*'
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
- `CluedIn.Testing.Base`/`CluedIn.CrawlerIntegrationTesting` were originally hardcoded at `5.0.0-*`
  because neither was multi-version targeted yet (see Step 6 — this got fixed once both were
  migrated in their own repos). Now reference the version-suffixed package IDs those repos actually
  publish (`CluedIn.Testing.Base.$(_CluedInPackageSuffix)`, etc.) — **not** a single package ID with
  `$(_CluedIn)` as the version, despite that being what the MasterDataServices doc's snippet shows.
  See Step 6 for why.

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

## Step 6 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-beta.*

Status: **Done** — `src/` and integration tests both verified locally (build **and** `dotnet test`)
and in real CI across all three legs.

All builds below were run locally against the real feeds (`dotnet restore`/`dotnet build -p:_CluedIn=<v>
-p:CluedInMultiVersionTargetFramework=<tfm>`), not just reasoned about.

### Finding: RestSharp major-version break (not a CluedIn.Core API break)

`CluedIn.Core`/`CluedIn.Crawling` bring RestSharp in transitively, and the version differs sharply
by CluedIn generation:
- CluedIn 4.7.0/4.8.0 (net6.0) → **RestSharp 106.15.0** (legacy API: `Method.GET` uppercase enum,
  `client.ExecuteAsync<T>()` returns `IRestResponse<T>`)
- CluedIn 5.0.0-beta.* (net10.0) → **RestSharp 114.0.0** (rewritten API: `Method.Get` PascalCase,
  `ExecuteAsync<T>()` returns `RestResponse<T>` directly, no `IRestResponse<T>` interface)

`GoogleMapsExternalSearchProvider.cs` was written against the newer RestSharp API. Fixed with
`#if CLUEDIN_V50` guards at every divergent call site (9 total: 5× `Method.Get`/`Method.GET`, 4×
explicitly-declared `RestResponse<T>`/`IRestResponse<T>` locals and the
`ConstructVerifyConnectionResponse<T>` parameter type). `var`-inferred response locals (e.g. the
`LocationDetailsResponse` call) needed no guard — the type is inferred correctly either way, only
explicitly-declared-type locals broke.

Verified: both src projects (`ExternalSearch.Providers.GoogleMaps`,
`Provider.ExternalSearch.GoogleMaps`) now build cleanly (0 errors) against all three targets:
4.7.0/net6.0, 4.8.0/net6.0, 5.0.0-beta.*/net10.0 (and, before the alpha→beta correction above,
5.0.0-alpha.*/net10.0 too - RestSharp resolves to the same 114.0.0 either way, so the guards are
unaffected by which 5.0 prerelease label is targeted). Full solution build at the local-dev default
(net10.0) also still passes.

### Resolved: integration tests now build and run against all three legs

Originally, `CluedIn.Testing.Base` and `CluedIn.CrawlerIntegrationTesting` (used by
`test/integration/Integration.Tests`) were **net10.0-only** — `CluedIn.Testing.Base` had never been
published for the 4.x line at all, and even pinning to a fixed version failed with
`NU1202: Package CluedIn.Testing.Base 5.0.0-alpha.5 is not compatible with net6.0`. This was a
missing TFM, not a version-selection problem, and not fixable from this repo — it required
multi-version targeting the `crawler-testing` and `CluedIn.Testing.Base` repos themselves (their own
`docs/multi-version-targeting-migration.md` cover that work; both merged and published as
`1.0.0-beta.1`).

**Once both were published, a second problem surfaced**: they aren't a single package ID with
`$(_CluedIn)` as the version (unlike `CluedIn.Core` etc.) — each CluedIn leg publishes under a
**different package ID**, suffixed with the dotless `Major.Minor.Patch` (confirmed against the
actual `develop` feed, not assumed from any doc): `CluedIn.Testing.Base.470`,
`CluedIn.Testing.Base.480`, `CluedIn.Testing.Base.500` (**`.500`, not `.50`** — the reference docs'
package-suffix tables are wrong here; the suffix is the literal 3-part version with dots stripped).
Fixed by computing the suffix once in `Packages.props`:

```xml
<_CluedInPackageSuffix>$(_CluedInVersionOnly.Replace('.', ''))</_CluedInPackageSuffix>
...
<PackageReference Update="CluedIn.Testing.Base.$(_CluedInPackageSuffix)" Version="1.0.0-*" />
<PackageReference Update="CluedIn.CrawlerIntegrationTesting.$(_CluedInPackageSuffix)" Version="1.0.0-*" />
```

and referencing the same property-interpolated name in `Integration.Tests.csproj`'s own
`PackageReference Include`s. The assembly/namespace inside each suffixed package is still plain
`CluedIn.Testing.Base` (the suffix only applies to the outer `PackageId`, not `AssemblyName`), so no
source changes were needed — only the package reference itself.

Verified with real `dotnet test` runs (not just `dotnet build`) across all three legs — the one
non-skipped test (`TestNoClueProduction`) passes on 4.7.0/net6.0, 4.8.0/net6.0, and
5.0.0-beta.*/net10.0. The other two tests remain `[Theory(Skip = "Requires a working api key")]`,
unrelated to this migration.

**Pipeline enabled accordingly**: flipped `runIntegrationTests` default to `true` in
`azure-pipelines.yml`, and removed the `createIntegrationEnvironmentScriptFilePath`/
`Arguments` parameters — they pointed at a `./build/integration-test.ps1` that never existed in this
repo, and none of the currently-runnable tests need any real environment setup (no external calls,
no live CluedIn host — everything goes through `BaseExternalSearchTest`'s in-process mocks).

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

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-beta.* — corrected from 5.0.0-alpha.* post-merge, see note above) — schema verified against live template
- [x] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework` with net10.0 local fallback; `LangVersion` pinned to 13.0
- [x] `Packages.props` — renamed from lowercase; `_CluedIn` guarded; `DefineConstants` derived; `Microsoft.NET.Test.Sdk` / `xunit.runner.visualstudio` pinned per TFM
- [x] `test/Directory.Build.props` — package refs removed, properties only
- [x] Test csproj (`Integration.Tests`) — conditional xunit v2/v3 + AutoFixture selection; `GlobalUsings.cs` added
- [x] `test/unit/Directory.Build.props` — deleted (was dead scaffold, no project consumed it)
- [x] `NuGet.Config` — renamed from `Nuget.config`; feeds confirmed sufficient for 4.7.0/4.8.0, no extra feed needed
- [x] Source — `#if CLUEDIN_V50` guards added for the RestSharp 106↔114 API break (9 call sites in `GoogleMapsExternalSearchProvider.cs`)
- [x] `GitVersion.yml` — `next-version: 1.0`; `ignore.commits-before: 2026-06-18T00:00:00`
- [x] `src/` builds clean (0 errors) for all three legs, verified locally via real `dotnet restore`/`build`
- [x] Integration tests — `CluedIn.Testing.Base`/`CluedIn.CrawlerIntegrationTesting` migrated in their own repos and published; `Packages.props`/`Integration.Tests.csproj` updated to reference the suffixed package IDs (`.470`/`.480`/`.500`); `runIntegrationTests` flipped to default `true`; dead `integration-test.ps1` script reference removed; real `dotnet test` passes on all three legs
- [x] Push branch and confirm the actual Azure DevOps pipeline run is green end-to-end — PR #55, all three legs (4.7.0, 4.8.0, 5.0.0-alpha.* at the time) plus the `Multi-version: publish` job passed in CI on the first run
- [x] Re-confirm CI is still green after switching the third leg from 5.0.0-alpha.* to 5.0.0-beta.* — re-ran on PR #55, all legs + publish passed again
- [ ] Re-confirm CI is still green now that integration tests are enabled and the test-support package references changed
