![BUSINESS AS USUAL](https://img.shields.io/badge/BUSINESS_AS_USUAL-%23F57C00?style=for-the-badge&logo=data:image/png;base64,...)
![CI Build](https://github.com/cruckman900/BusinessAsUsual/actions/workflows/ci.yml/badge.svg)
![License](https://img.shields.io/github/license/cruckman900/BusinessAsUsual)
![Last Commit](https://img.shields.io/github/last-commit/cruckman900/BusinessAsUsual)
![Contributors](https://img.shields.io/github/contributors/cruckman900/BusinessAsUsual)
# 🎸 Business As Usual: Modular ASP.NET Core Backend

Welcome to **Business As Usual**—a multi-tenant, modular ASP.NET Core platform built for scale, clarity, and creative firepower. This backend is the engine behind your business suite, orchestrating products, services, accounting, inventory, scheduling, and billing with precision and swagger.

---

## 🚀 Tech Stack

- **Framework:** ASP.NET Core 9.0
- **Architecture:** Clean Architecture + Modular Design
- **Containerization:** Docker
- **Cloud Deployment:** AWS (EC2, RDS, S3, IAM)
- **Database:** PostgreSQL (via EF Core)
- **CI/CD:** GitHub Actions + Docker Hub
- **Security:** JWT Auth, HTTPS, CORS, Rate Limiting
- **Docs:** Swagger + NOTES.md (timestamped changelog)

---

## 🧩 Modular Breakdown

| Module        | Purpose                                   | Status     |
|---------------|--------------------------------------------|------------|
| `Core`        | Domain models, interfaces, base logic      | ✅ Stable   |
| `Infrastructure` | DB context, services, external integrations | ✅ Stable   |
| `Application` | Use cases, DTOs, business rules            | ✅ Stable   |
| `WebAPI`      | Controllers, endpoints, middleware         | 🚧 In Progress |
| `Identity`    | Auth, registration, roles, JWT             | ✅ Stable   |
| `Tenant`      | Multi-tenant logic, scoped services        | 🚧 Planned  |

---

## 🛠️ Setup & Onboarding

```bash
# Clone the repo
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd business-as-usual-backend

# Build and run with Docker
docker-compose up --build

# Access Swagger UI
http://localhost:5000/swagger
