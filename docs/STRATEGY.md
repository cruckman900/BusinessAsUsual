# Business As Usual — Strategy & Architecture

This document outlines the architectural vision, workflow strategy, and onboarding clarity for contributors to Business As Usual.

---

## 🎯 Project Intentions

- Build a clean, scalable ASP.NET Core MVC application
- Architect with Clean Architecture principles
- Modularize components for future microservices
- Containerize with Docker for consistent dev environments
- Deploy to AWS using ECS, ECR, RDS, and CodePipeline
- Maintain expressive documentation and creative branding

---

## 🧱 Solution Structure

```
BusinessAsUsual/
├── frontend/
│   ├── BusinessAsUsual.Admin/           # MVC frontend (UI Layer) Admin Site
│   ├── BusinessAsUsual.Web/             # MVC frontend (UI Layer) Client Site
├── backend/
│   ├── BusinessAsUsual.API/             # Public-facing APIs (API Layer)
│   ├── BusinessAsUsual.Application/     # Use cases, interfaces (Application Layer)
│   ├── BusinessAsUsual.Domain/          # Entities, enums, value objects (Domain Layer)
│   └── BusinessAsUsual.Infrastructure/  # EF Core, external services (Infrastructure Layer)
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
├── .github/
│   └── workflows/
│       └── ci.yml                       # GitHub Actions workflow
├── .dockerignore
├── .editorconfig
├── NOTES.md                             # Milestones and creative logs
├── STRATEGY.md                          # Architecture and onboarding guide
├── LICENSE.md                           # Project license
└── README.md                            # Public-facing project overview
```

---

## 🧠 Architectural Philosophy

### Clean Architecture Layers

- **Domain**: Core business logic, entities, value objects
- **Application**: Use cases, service contracts, interfaces
- **Infrastructure**: EF Core, external APIs, file systems
- **UI**: MVC frontend, Razor Pages, controllers

### Microservices (Planned)

- Auth
- Billing
- Products
- Notifications

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
