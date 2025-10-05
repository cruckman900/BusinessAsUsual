🧱 ARCHITECTURE.md — Modular Backbone of Business As Usual
This file breaks down the ASP.NET Core backend like a stage rig: every layer has a role, every module is tuned for clarity and performance.

🧩 Layered Breakdown
- Presentation Layer: API controllers, request validation, response formatting.
- Application Layer: Use cases, interfaces, and orchestration logic. No direct dependencies.
- Domain Layer: Core models, enums, and business rules. Pure and portable.
- Infrastructure Layer: Database access, external services, and configuration wiring.
- Cross-Cutting Concerns: Logging, caching, authentication, and error handling.

🐳 Dockerized Flow
- Each layer lives in its own project folder
- Dockerfile builds and runs the backend with environment-specific configs
- Health endpoint confirms container readiness (/healthz or /status)

🧠 Design Principles
- Clean Architecture: Dependency inversion, separation of concerns, testability
- SOLID: Especially Single Responsibility and Interface Segregation
- Future-Proofing: Easy to extend with modules for billing, scheduling, inventory, etc.

📓 Bonus Notes
- Log architecture tweaks in CHANGELOG.md
- Reference LICENSE.md for backend protection
- Sync with frontend via shared DTOs or API contracts
