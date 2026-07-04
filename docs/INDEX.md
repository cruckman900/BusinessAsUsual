# 📚 BusinessAsUsual — Documentation Index

> **The single source of truth for navigating BAU documentation.**
> Start here. Every canonical doc is listed below with its purpose and status.

**Legend:** 🟢 Canonical (authoritative) · 🟡 Active reference · 🔵 Historical/archived · 🗄️ See [`archive/`](archive/)

---

## 🚀 Start Here

| Doc | Purpose | Status |
|-----|---------|--------|
| [README.md](README.md) | Docs folder landing page & map | 🟢 |
| [ONBOARDING.md](ONBOARDING.md) | Contributor setup, local run, AI quickstart | 🟢 |
| [QUICK_START_GUIDE.md](QUICK_START_GUIDE.md) | Fast path to a running app | 🟡 |
| [QUICKSTART_RUNNING_THE_SYSTEM.md](QUICKSTART_RUNNING_THE_SYSTEM.md) | End-to-end multi-service run | 🟡 |
| [PORT_REGISTRY.md](PORT_REGISTRY.md) | Authoritative service → port map | 🟢 |

## 🧭 Product Strategy & Roadmap

| Doc | Purpose | Status |
|-----|---------|--------|
| [CloudProud.md](CloudProud.md) | **Canonical product roadmap** (all phases) | 🟢 |
| [STRATEGY.md](STRATEGY.md) | Positioning, go-to-market, architecture alignment | 🟢 |
| [AI-PriorityQueue.md](AI-PriorityQueue.md) | Ranked AI initiative backlog | 🟢 |
| [DEVELOPER-ISSUES.md](DEVELOPER-ISSUES.md) | Sprint-ready developer task list | 🟢 |
| [CRM_FEATURE_ROADMAP.md](CRM_FEATURE_ROADMAP.md) | CRM module feature plan | 🟡 |
| [MODULE_STATUS.md](MODULE_STATUS.md) | Built / in-progress / planned matrix | 🟢 |
| [CHANGELOG.md](CHANGELOG.md) | Release & milestone log | 🟢 |
| [TODO.md](TODO.md) | Legacy backlog (being folded into roadmap) | 🟡 |

## 🏗️ Architecture

| Doc | Purpose | Status |
|-----|---------|--------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | **Canonical backend architecture** + AI microservice | 🟢 |
| [MICROSERVICEARCHITECTUREOVERVIEW.md](MICROSERVICEARCHITECTUREOVERVIEW.md) | Service topology & endpoints | 🟢 |
| [MODULE_REGISTRY_AND_UI_INJECTION.md](MODULE_REGISTRY_AND_UI_INJECTION.md) | Module discovery & UI injection | 🟡 |
| [MULTI_ASSEMBLY_ROUTING.md](MULTI_ASSEMBLY_ROUTING.md) | Cross-assembly Blazor routing | 🟡 |
| [WEB_UI_ARCHITECTURE_DECISION.md](WEB_UI_ARCHITECTURE_DECISION.md) | Integrated components vs iframes rationale | 🟢 |
| [SIMPLE_NAVIGATION.md](SIMPLE_NAVIGATION.md) | **Current** navigation model | 🟢 |
| [NAVIGATION_ORCHESTRATOR.md](NAVIGATION_ORCHESTRATOR.md) | Navigation orchestration details | 🟡 |
| [MOBILE_ARCHITECTURE.md](MOBILE_ARCHITECTURE.md) | Mobile strategy ⚠️ *reconcile w/ CloudProud MAUI* | 🟡 |
| [MOBILE_UI_CONTRACT_SYSTEM.md](MOBILE_UI_CONTRACT_SYSTEM.md) | Shared mobile UI contract system | 🟡 |

## 🔒 Enterprise Readiness

| Doc | Purpose | Status |
|-----|---------|--------|
| [SECURITY.md](SECURITY.md) | Security, RBAC, tenant isolation, compliance | 🟢 |
| [INTEGRATIONS.md](INTEGRATIONS.md) | Webhooks, connectors, API keys | 🟢 |
| [AWSDEPLOYMENT.md](AWSDEPLOYMENT.md) | Cloud deployment reference | 🟡 |
| [DATABASE_FREE_DEVELOPMENT.md](DATABASE_FREE_DEVELOPMENT.md) | Mock-data local development mode | 🟡 |

## 🎨 Design, Brand & Standards

| Doc | Purpose | Status |
|-----|---------|--------|
| [UI_STANDARDS.md](UI_STANDARDS.md) | UI/UX standards | 🟢 |
| [STYLEGUIDE.md](STYLEGUIDE.md) | Code & content style guide | 🟢 |
| [BRANDING.md](BRANDING.md) | Brand guidelines | 🟡 |
| [Design-Evolution.md](Design-Evolution.md) | Design history & direction | 🟡 |
| [OrganizationalBackground.md](OrganizationalBackground.md) | Animated SVG background spec | 🟡 |
| [SPLASH.md](SPLASH.md) | Splash screen spec | 🟡 |
| [UI-Enhancement-Summary.md](UI-Enhancement-Summary.md) | UI enhancement notes | 🟡 |
| [BADGES.md](BADGES.md) | README badge reference | 🟡 |

## 🛠️ Developer Tooling & Reference

| Doc | Purpose | Status |
|-----|---------|--------|
| [HR_ROUTES_COMPLETE_REFERENCE.md](HR_ROUTES_COMPLETE_REFERENCE.md) | HR route reference table | 🟡 |
| [HR_HIERARCHICAL_NAVIGATION.md](HR_HIERARCHICAL_NAVIGATION.md) | HR nav hierarchy reference | 🟡 |
| [PSCOMMANDS.md](PSCOMMANDS.md) | Handy PowerShell commands | 🟡 |
| [SMARTCOMMIT.md](SMARTCOMMIT.md) | Commit message conventions | 🟡 |
| [JIRABACKLOGTEMPLATE.md](JIRABACKLOGTEMPLATE.md) | Backlog import template | 🟡 |
| [METADATA.md](METADATA.md) | Project metadata reference | 🟡 |
| [LICENSE.md](LICENSE.md) | License | 🟢 |

---

## 🗄️ Archived Documentation

Point-in-time session artifacts (fix summaries, phase-completion reports, deprecated
designs) have been moved to [`archive/`](archive/) to keep the active docs clean.
They remain in version control for historical reference. See
[archive/README.md](archive/README.md) for the full list.

---

## 🧩 Quick Facts

- **Target framework:** .NET 9
- **Web UI:** Blazor + MudBlazor
- **Total modules cataloged:** 68 across 7 categories (see [MODULE_STATUS.md](MODULE_STATUS.md))
- **Built modules:** HR, CRM, Module Registry, Platform core, AI assistant (UI)
- **Canonical roadmap:** [CloudProud.md](CloudProud.md)

_Last updated: this doc is maintained as the master index. When adding a new doc, add a row here._
