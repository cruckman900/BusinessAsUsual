# Business As Usual — Strategy & Architecture

This document outlines the architectural vision, workflow strategy, and onboarding clarity for contributors to Business As Usual.

---

## 🎯 Project Intentions

- Build a clean, scalable ASP.NET Core platform on **.NET 9**
- Deliver the UI with **Blazor + MudBlazor** (interactive component model)
- Architect with Clean Architecture principles
- Modularize components as independently deployable microservices
- Containerize with Docker for consistent dev environments
- Deploy to AWS using ECS, ECR, RDS, and CodePipeline
- Maintain expressive documentation and creative branding

---

## 🧱 Solution Structure

```
BusinessAsUsual/
├── frontend/
│   ├── BusinessAsUsual.Admin/           # Blazor + MudBlazor (UI Layer) Admin Site
│       └── Dockerfile 
│   └── BusinessAsUsual.Web/             # Blazor + MudBlazor (UI Layer) Client Shell
│       └── Dockerfile 
├── backend/
│   ├── BusinessAsUsual.API/             # Public-facing APIs (API Layer)
│       └── Dockerfile 
│   ├── BusinessAsUsual.Application/     # Use cases, interfaces (Application Layer)
│   ├── BusinessAsUsual.Domain/          # Entities, enums, value objects (Domain Layer)
│   └── BusinessAsUsual.Infrastructure/  # EF Core, external services (Infrastructure Layer)
├── BusinessAsUsual.Core/                # Shared kernel, module catalog & discovery
├── services/                            # Independently deployable modules
│   ├── HR/                              # Domain/Application/API/Infrastructure/Web
│   ├── CRM/                             # Domain/Application/API/Web
│   └── ModuleRegistry/                  # Module discovery & UI injection
├── .github/
│   └── workflows/
│       └── ci.yml                       # GitHub Actions workflow
├── .dockerignore
├── .editorconfig
├── NOTES.md                             # Milestones and creative logs
├── STRATEGY.md                          # Architecture and onboarding guide
├── LICENSE.md                           # Project license
├── docker-compose
└── README.md                            # Public-facing project overview
```

---

## 🧠 Architectural Philosophy

### Clean Architecture Layers

- **Domain**: Core business logic, entities, value objects
- **Application**: Use cases, service contracts, interfaces
- **Infrastructure**: EF Core, external APIs, file systems
- **UI**: Blazor + MudBlazor components, rendered through the client shell

### Microservices

**Built:** HR, CRM, Module Registry (see [MODULE_STATUS.md](MODULE_STATUS.md))

**Planned:** Auth, Billing, Products, Notifications, AI (see [AI-PriorityQueue.md](AI-PriorityQueue.md))

### AI Strategy (Positioning & Go‑to‑Market)

- Positioning: BAU will be marketed as a modular, .NET‑native business platform with built-in AI that accelerates user workflows (search, summarize, recommend) and can be self‑hosted or run in customers' clouds. This appeals to mid‑market customers and Microsoft‑centric organizations that require customizability and data control.
- Go‑to‑Market priorities:
  1. Ship high‑value, low‑cost AI features (SmartCRM Assistant, Summarization, Smart Search) as part of the core offering.
  2. Offer paid integrations and managed hosting for customers who prefer zero‑ops.
  3. Publish developer SDKs and marketplace modules to grow the ecosystem.
- Messaging pillars: Ownership (self‑host), Extensibility (modules), Productivity (AI), and Microsoft stack synergy (.NET + Azure integration).

Refer to `/docs/CloudProud.md` and `/docs/AI-PriorityQueue.md` for an actionable roadmap and prioritized AI initiatives.

Each service will be independently deployable, containerized, and optionally split into its own repo.

---

## 🐳 Docker Strategy

- Linux-based containers via Docker Desktop
- Dockerfile scaffolded from project creation
- Future: docker-compose for multi-service orchestration

---

## ☁️ AWS Deployment Strategy

| Service           | Purpose                              |
|-------------------|--------------------------------------|
| **ECR**           | Store Docker images                  |
| **ECS (Fargate)** | Deploy containers serverlessly       |
| **RDS**           | Host PostgreSQL or SQL Server        |
| **S3**            | Static assets, backups, logs         |
| **CodePipeline**  | CI/CD orchestration                  |
| **IAM**           | Secure access control                |

---

## 🧪 CI/CD Pipeline

- GitHub Actions for build, test, and Docker push
- AWS CodePipeline for deployment orchestration
- Linting via `.editorconfig` and `dotnet format`
- Future: test coverage reports and badge integration

---

## 🧾 Onboarding Checklist

- ✅ Clone the repo
- ✅ Install Docker Desktop
- ✅ Run the app via Docker or Visual Studio
- ✅ Read `STRATEGY.md` for architecture and workflow
- ✅ Check `NOTES.md` for creative milestones
- ✅ Ask questions, suggest riffs, and contribute with confidence

---

## 🕯️ Final Note

This is more than a repo—it’s a platform built with clarity, creativity, and purpose. Every module is a movement. Every milestone is a memory. Welcome aboard.
