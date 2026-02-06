
# 🎸 Business As Usual — Modular ASP.NET Core Platform

**Business As Usual** is a multi-tenant, modular ASP.NET Core platform built for clarity, scale, and expressive engineering.  
It powers a suite of business modules—products, services, accounting, inventory, scheduling, billing, and more—through a clean, extensible architecture.

![BUSINESS AS USUAL](https://img.shields.io/badge/BUSINESS_AS_USUAL-%23F57C00?style=for-the-badge&logo=data:image/png;base64,...)
![Proprietary](https://img.shields.io/badge/License-Proprietary-red?style=for-the-badge)
![Contributors](https://img.shields.io/github/contributors/cruckman900/BusinessAsUsual?style=for-the-badge)
![CI Build](https://github.com/cruckman900/BusinessAsUsual/actions/workflows/ci.yml/badge.svg)

![Hits](https://hits.sh/github.com/cruckman900/BusinessAsUsual-Android.svg?style=for-the-badge)
![Stars](https://img.shields.io/github/stars/cruckman900/BusinessAsUsual-Android?style=for-the-badge)
![Issues](https://img.shields.io/github/issues/cruckman900/BusinessAsUsual-Android?style=for-the-badge)
![Last Commit](https://img.shields.io/github/last-commit/cruckman900/BusinessAsUsual-Android?style=for-the-badge)

## 🚀 Tech Stack

- **Framework:** ASP.NET Core 9.0  
- **Architecture:** Clean Architecture + Modular Design  
- **Containerization:** Docker  
- **Cloud Deployment:** AWS (EC2, RDS, S3, IAM)  
- **Database:** PostgreSQL (via EF Core)  
- **CI/CD:** GitHub Actions + Docker Hub  
- **Security:** JWT Auth, HTTPS, CORS, Rate Limiting  
- **Docs:** Swagger + `/docs` (architecture, onboarding, theming, metadata)

## 🧩 Modular Breakdown

| Module           | Purpose                                      | Status         |
|------------------|----------------------------------------------|----------------|
| `Core`           | Domain models, interfaces, base logic        | ✅ Stable      |
| `Infrastructure` | DB context, services, external integrations  | ✅ Stable      |
| `Application`    | Use cases, DTOs, business rules              | ✅ Stable      |
| `WebAPI`         | Controllers, endpoints, middleware           | 🚧 In Progress |
| `Identity`       | Auth, registration, roles, JWT               | ✅ Stable      |
| `Tenant`         | Multi-tenant logic, scoped services          | 🚧 Planned     |

## 📚 Documentation

All detailed documentation lives in the `/docs` directory:

- **Onboarding:** `/docs/ONBOARDING.md`  
- **Architecture:** `/docs/ARCHITECTURE.md`  
- **Theme System:** `/docs/Themes/README.md`  
- **Branding & Metadata:** `/docs/BRANDING.md` and `/docs/METADATA.md`  
- **Changelog:** `/docs/CHANGELOG.md`  
- **Splash Logic, Badges, and More:** See additional files in `/docs`

This keeps the root README clean while giving contributors a clear map of the system.

## 🛠️ Setup & Onboarding

```bash
# Clone the repo
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd BusinessAsUsual

# Build and run with Docker
docker-compose up --build

# Access Swagger UI
http://localhost:5000/swagger

🧭 Project Goals
- Provide a modular, multi-tenant foundation for business applications
- Maintain a clean, extensible architecture suitable for long-term growth
- Deliver expressive, accessible UI/UX through a unified theme system
- Support real-world deployment pipelines (Docker + AWS)
- Keep documentation clear, discoverable, and contributor-friendly
```
