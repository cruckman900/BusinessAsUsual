### [2025-10-03] 🧱 Business As Usual: Project Kickoff

- ASP.NET Core selected for modular, scalable architecture
- AWS chosen for cloud deployment, CI/CD, and database hosting
- Docker introduced for dev environment consistency
- Initial goal: bare-bones landing page with DB connection and CI pipeline
- Branding assets created: logo, badge, OG image

### [2025-10-03] 🐳 Container Support Enabled

- Enabled Docker support during ASP.NET project creation
- Dockerfile scaffolded for ASP.NET Core runtime
- Ready for local container testing and AWS deployment
- CI/CD and environment parity now baked into the foundation

### [2025-10-03] 🐳 Container Build Type Selected

- Chose Linux as container build type for ASP.NET Core project
- Ensures leaner images, faster builds, and AWS compatibility
- Ready for Docker-based dev environments and cloud deployment

### [2025-10-03] 🧠 Project Setup Decisions

- ✅ Enabled top-level statements for clean, modern startup
- ❌ Deferred .NET Aspire orchestration—will revisit when scaling to multi-service architecture
- Project now bootstrapped with clarity and purpose—ready for CI, Docker, and AWS integration

### [2025-10-03] 🐳 Docker Desktop Activated

- Selected “Yes” to start Docker Desktop on project creation
- Container support now fully operational for ASP.NET Core app
- Ready for local container testing, CI integration, and cloud deployment

### [2025-10-03] 📄 Markdown Added to Solution

- Added `NOTES.md` to Solution Explorer via Add → New Item
- Markdown file now tracked in project and ready for changelogs, milestones, and poetic riffs

### [2025-10-03] 🔧 Scaffolding Without AWS

- Dockerfile finalized with clean `.dockerignore`
- `docker-compose.yml` added for local PostgreSQL
- GitHub Actions CI pipeline scaffolded
- EF Core setup initiated with modular architecture
- AWS pending—scaffold ready to deploy when phone reactivates

### [2025-10-03] 📁 File Placement Finalized

- Docker, CI, and linting files placed in correct folders
- Documentation and strategy files organized at root
- Branding assets stored in `/branding`
- Source code modularized under `/src`
- Repo now clean, scalable, and ready for AWS ignition

### [2025-10-03] 🐳 Dockerfile Extension Fixed

- Renamed `Dockerfile.txt` to `Dockerfile` (no extension)
- Docker now recognizes the file for image builds
- Containerization pipeline back on track

## [2025-10-03] 🧱 Split frontend and backend
- Moved UI assets to `/frontend`
- Isolated ASP.NET Core backend in `/backend`
- Updated solution and Docker config

## [2025-10-03] 🧼 Wired up code quality tools
- Added `.editorconfig` for consistent style
- Installed StyleCop.Analyzers for strict C# linting
- Enabled SonarLint for static analysis and security checks

## [2025-10-04] 🎯 Solution view unlocked
- Moved BusinessAsUsual.Tests to solution root
- Re-added test project as proper node (not just a Solution Item)
- Confirmed full solution view in Visual Studio

## [2025-10-04] 📘 Program.cs fully documented
- Added XML doc comments to all methods and parameters
- Modularized service and middleware configuration
- Aligned namespace and file header with project standards
- Silenced StyleCop warnings for spacing, naming, and documentation

## [2025-10-04] 🧠 Analyzer scaffold workaround
- Cloned Roslyn SDK templates from GitHub
- Scaffolded BusinessAsUsual.Analyzers via Visual Studio
- Planned Docker integration for build-time enforcement

## [2025-10-04] 🧠 Analyzer project scaffolded
- Created BusinessAsUsual.Analyzers as sibling to main app
- Scoped analyzer for enforcing XML doc tags like <author> and <lastmodifiedby>
- Planned integration into CI and Docker builds

## [2025-10-04] 🧠 Accountability analyzer activated
- Enforced <author> and <lastmodifiedby> tags in XML doc comments
- Analyzer emits warnings for undocumented public methods
- Integrated into BusinessAsUsual.Analyzers project

## [2025-10-04] 🧠 Analyzer enforcement activated
- Analyzer emits BAU001 and BAU002 warnings for missing <author> and <lastmodifiedby> tags
- Scoped to public methods only for contributor accountability
- Integrated into BusinessAsUsual.Analyzers project

## [2025-10-04] 🧠 Analyzer logic initiated
- Confirmed BusinessAsUsualAnalyzers.Analyzer.cs as entry point
- Planned enforcement of <author> and <lastmodifiedby> tags in XML doc comments

## [2025-10-04] 🧼 .gitignore refined
- Excluded node_modules, build artifacts, and Visual Studio cache
- Preserved .github/workflows for CI/CD
- Ensured clean staging for initial commit