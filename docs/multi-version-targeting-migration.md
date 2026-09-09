# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.GoogleMaps` from a single-version build to
the multi-version targeting pattern. It is kept up to date as work lands (not written after the
fact), so it can double as a reference for migrating the remaining repos.

Prior art consulted before starting this migration:
- `CluedIn.Crawling.MasterDataServices/docs/multi-version-targeting-migration.md`
- `CluedIn.Connector.AzureEventHubs/docs/multi-version-targeting-migration.md`
- `CluedIn.Connector.AzureDataLake` and `CluedIn.Mcp` (completed, but no migration doc — inspected
  `azure-pipelines.yml` / `Directory.Build.props` / `Packages.props` directly as the more
  up-to-date reference for pipeline template parameter shape)

---

## Overview

The goal is to produce separate NuGet packages per CluedIn version from a single branch, using the
shared `crawler.build.jobs.yml` pipeline template. Each package targets the correct .NET TFM for
that CluedIn generation.

| CluedIn version | .NET TFM | Package suffix |
|---|---|---|
| 4.7.0 | net6.0 | `.470` |
| 4.8.0 | net6.0 | `.480` |
| 5.0.0-alpha.* | net10.0 | `.50` |

4.6.0 was deliberately excluded (decision made 2026-09-09) — kept the version window aligned with
`CluedIn.Crawling.MasterDataServices` rather than the wider `CluedIn.Connector.AzureEventHubs`
window (4.6.0+), since there's no known 4.6-only dependency in this enricher's small API surface.
Revisit if that assumption turns out wrong once builds are attempted against 4.7.0.

Branch: `feature/multi-version-targeting` (off `develop`).

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Not started**

Replace the single-version `crawler.build.yml` steps-template with the multi-version
`crawler.build.jobs.yml` jobs-template.

> **Open question / needs verification before implementing:** the two prior migration docs
> (MasterDataServices, AzureEventHubs) show `multiVersionCluedInTargets` as a flat list of version
> strings:
> ```yaml
> multiVersionCluedInTargets:
>   - 4.7.0
>   - 4.8.0
>   - 5.0.0-alpha.*
> ```
> but the two most recently completed repos (`CluedIn.Connector.AzureDataLake`, `CluedIn.Mcp`)
> actually use a list of objects:
> ```yaml
> multiVersionCluedInTargets:
>   - cluedInVersion: '4.7.0'
>   - cluedInVersion: '4.8.0'
>   - cluedInVersion: '5.0.0-alpha.*'
> ```
> Treat the object-list shape as current and verify against the live `crawler.build.jobs.yml` in
> `AzurePipelines.Templates` before wiring this up — the markdown docs appear stale on this point.

Also drop the explicit `UseDotNet@2` step (SDK install is handled per build leg by the jobs
template) and add the `probeCluedInVersion` pipeline parameter for one-off out-of-matrix checks.

---

## Step 2 — `Directory.Build.props`

Status: **Not started**

Honour `CluedInMultiVersionTargetFramework`, falling back to `net10.0` for local dev:

```xml
<TargetFramework Condition="'$(CluedInMultiVersionTargetFramework)' != ''">$(CluedInMultiVersionTargetFramework)</TargetFramework>
<TargetFramework Condition="'$(CluedInMultiVersionTargetFramework)' == ''">net10.0</TargetFramework>
```

---

## Step 3 — `Packages.props`

Status: **Not started**

- Guard `_CluedIn` so the pipeline value wins:
  ```xml
  <_CluedIn Condition="'$(_CluedIn)' == ''">5.0.0-*</_CluedIn>
  ```
- Derive `DefineConstants` from `_CluedIn` (strip pre-release suffix, compare with
  `System.Version`):
  - `CLUEDIN_V47` for >= 4.7.0
  - `CLUEDIN_V48` for >= 4.8.0
  - `CLUEDIN_V50` for >= 5.0.0
- Conditionally pin test packages that dropped support for net6.0:
  - `Microsoft.NET.Test.Sdk`: `18.3.0` (net10.0) / `17.12.0` (net6.0)
  - `xunit.runner.visualstudio`: `3.1.5` (net10.0) / `2.8.2` (net6.0)

---

## Step 4 — Test projects

Status: **Not started**

- Remove package selection from `test/Directory.Build.props` — leave it with shared properties
  only (`IsTestProject`, etc.). Doing xunit-version selection there causes a **CS0433** clash when
  a non-V50 leg's conditional `ItemGroup` combines with an unconditional xunit v3 include higher up
  (root-caused in the MasterDataServices doc, Step 4a).
- Select xunit generation per test csproj instead:
  ```xml
  <ItemGroup Condition="$(DefineConstants.Contains('CLUEDIN_V50'))">
    <PackageReference Include="xunit.v3" />
    <PackageReference Include="AutoFixture.Xunit3" />
  </ItemGroup>
  <ItemGroup Condition="!$(DefineConstants.Contains('CLUEDIN_V50'))">
    <PackageReference Include="xunit" />
    <PackageReference Include="AutoFixture.Xunit2" />
  </ItemGroup>
  ```
- Add a `GlobalUsings.cs` per test project to switch the `AutoFixture` namespace instead of
  scattering `#if` across test files.
- Clean up `test/unit/Directory.Build.props` — it's currently dead scaffold (pins `Moq 4.5.30` /
  `Should 1.1.20`, no unit test csproj exists to consume it). Either delete it or stand up a real
  unit test project against it; don't carry it forward untouched.

---

## Step 5 — `NuGet.Config` casing and feeds

Status: **Not started**

- `git mv` the checked-in `Nuget.config` to `NuGet.Config` (two-step rename, since Windows is
  case-insensitive and a single `git mv` is a no-op — same fix as in both prior docs).
- Verify the existing `develop` / `release` / `AzurePipelines` feeds actually resolve
  `CluedIn.Core`/`CluedIn.Crawling`/`CluedIn.ExternalSearch` at `4.7.0` and `4.8.0` before assuming
  they're sufficient. AzureEventHubs needed an additional public feed
  (`https://pkgs.dev.azure.com/CluedIn-io/Public/_packaging/Public/nuget/v3/index.json`) for
  4.6–4.8 packages not on nuget.org — add it here too if restore fails with `NU1101`.

---

## Step 6 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-alpha.*

Status: **Not started**

Small surface — 2 src projects, ~7 hand-written files
(`GoogleMapsExternalSearchProvider.cs`, `GoogleMapsProviderProviderComponent.cs`,
`GoogleMapsSearchProviderProvider.cs`, plus job data/models/vocabularies which are unlikely to hit
API breaks). Approach:

1. Confirm 5.0.0-alpha.* leg still builds/passes (it already does today, single-target).
2. Add the 4.8.0 (net6.0) leg, fix compiler errors with `#if CLUEDIN_V48` / `#if CLUEDIN_V47`
   guards as they surface.
3. Add the 4.7.0 (net6.0) leg, same approach.

Findings so far: _(none yet — fill in as guards are added)_

---

## Step 7 — Reset the semantic version (`GitVersion.yml`)

Status: **Not started**

Current `GitVersion.yml` has `next-version: 5.0`, still tracking the CluedIn version directly. Once
the CluedIn version is encoded in the package suffix instead, reset:

```yaml
next-version: 1.0
ignore:
  commits-before: <timestamp>
```

`<timestamp>` must sit after the last commit carrying an old high-version tag (`4.6.2` is the
highest tag in this repo today) and before the first commit of this feature branch:

```bash
git log --format="%aI %H %D" | grep "tag: 4\."
git log origin/develop..HEAD --format="%aI %H" | tail -1
```

Values to fill in once the branch has commits: _(pending)_

---

## Checklist

- [ ] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-alpha.*) — schema verified against live template
- [ ] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework` with net10.0 local fallback
- [ ] `Packages.props` — `_CluedIn` guarded; `DefineConstants` derived; `Microsoft.NET.Test.Sdk` / `xunit.runner.visualstudio` pinned per TFM
- [ ] `test/Directory.Build.props` — package refs removed, properties only
- [ ] Test csprojs — conditional xunit v2/v3 + AutoFixture selection; `GlobalUsings.cs` added
- [ ] `test/unit/Directory.Build.props` — resolved (deleted or backed by a real project)
- [ ] `NuGet.Config` — renamed from `Nuget.config`; feeds verified for 4.7.0/4.8.0
- [ ] Source — `#if` guards added for any API breaks found across 4.7.0/4.8.0/5.0.0-alpha.*
- [ ] `GitVersion.yml` — `next-version: 1.0`; `ignore.commits-before` set
- [ ] All three legs (4.7.0, 4.8.0, 5.0.0-alpha.*) build and test green in CI
