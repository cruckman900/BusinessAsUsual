# 🔒 Security & Compliance

> **Enterprise security posture for BusinessAsUsual.**
> This document defines how BAU protects data, controls access, isolates tenants,
> and prepares for compliance certification. It is the canonical reference for
> security decisions across all modules and services.

**Status:** 🟢 Canonical · Living document — update as controls are implemented.

---

## 🎯 Security Principles

1. **Secure by default** — deny-first authorization; features ship locked down.
2. **Defense in depth** — controls at network, app, data, and audit layers.
3. **Least privilege** — every identity gets the minimum access required.
4. **Tenant isolation** — one tenant can never read another's data.
5. **Auditable** — security-relevant actions are logged immutably.
6. **Privacy-aware** — PII is minimized, redacted, and access-controlled.

---

## 👤 Authentication & Identity

| Concern | Approach |
|---------|----------|
| User authentication | Token-based (JWT / OIDC); pluggable identity provider |
| Service-to-service | Signed service tokens / mTLS between microservices |
| API keys | Scoped, revocable keys for external integrations (see [INTEGRATIONS.md](INTEGRATIONS.md)) |
| Session lifetime | Short-lived access tokens + refresh; idle timeout |
| MFA | Planned for admin & privileged roles |

## 🛡️ Authorization (RBAC)

BAU uses **role-based access control** surfaced through the Platform
`User Management` module (Users / Roles / Permissions).

- **Roles** group permissions; **permissions** gate module actions.
- Enforcement happens at the **API layer** (never trust the UI alone).
- Module-level and submodule-level permissions align with
  [`ModuleCatalog.cs`](../BusinessAsUsual.Core/Modules/ModuleCatalog.cs).
- Recommended baseline roles: `Owner`, `Admin`, `Manager`, `Member`, `ReadOnly`.

> 🚧 Status: RBAC core is **in progress** (see [MODULE_STATUS.md](MODULE_STATUS.md)).

## 🏢 Multi-Tenancy & Data Isolation

| Layer | Isolation Strategy |
|-------|--------------------|
| Data | Tenant ID scoping on all tenant-owned entities; enforced in query filters |
| API | Tenant claim validated on every request; cross-tenant access denied |
| AI service | Embeddings and context partitioned per tenant; no cross-tenant retrieval |
| Storage | Logical partitioning; physical separation optional per plan tier |

## 🔐 Data Protection

- **In transit:** TLS for all external and inter-service traffic.
- **At rest:** Encrypt databases and file storage; manage keys via a secrets vault.
- **Secrets:** Never commit secrets. Use user-secrets locally, a vault in cloud.
- **PII handling:** Minimize collection; redact PII before sending to AI models
  (see AI governance in [ARCHITECTURE.md](ARCHITECTURE.md)).
- **Backups:** Encrypted, tested restore procedures; retention per policy.

## 🤖 AI-Specific Security

The AI microservice introduces unique risks addressed in [ARCHITECTURE.md](ARCHITECTURE.md):

- **PII redaction** before any external model call.
- **Tenant-scoped retrieval** — RAG context never crosses tenants.
- **Prompt-injection defenses** — untrusted content is sandboxed/labeled.
- **Rate limiting** on `/api/ai/*` to prevent abuse and cost runaway.
- **Audit logging** of AI queries and responses for traceability.

## 📝 Audit Logging

The Platform `Audit Logs` module captures:

- **System events** (config changes, module toggles).
- **Security events** (logins, permission changes, failed auth).
- Immutable, timestamped, attributable to an identity + tenant.

## ✅ Compliance Roadmap

BAU targets enterprise SaaS compliance as it matures:

| Framework | Relevance | Status |
|-----------|-----------|--------|
| **SOC 2 Type II** | Trust criteria for SaaS | 🗓️ Target |
| **GDPR** | EU personal data | 🗓️ Design-aligned (data minimization, right-to-erasure) |
| **CCPA/CPRA** | California privacy | 🗓️ Design-aligned |
| **HIPAA** | Healthcare module pack | 🗓️ Required before Healthcare GA |
| **PCI DSS** | Payments module | 🗓️ Required before Payments GA (prefer tokenized gateways) |

## 🚨 Vulnerability Management

- Keep dependencies patched; monitor advisories for NuGet packages.
- Run security scans in CI (SAST/dependency audit).
- Have a documented disclosure & incident-response process.

## 📋 Developer Security Checklist

- [ ] Authorize every new API endpoint (deny by default).
- [ ] Scope every query by tenant.
- [ ] Never log secrets or raw PII.
- [ ] Validate & sanitize all external input.
- [ ] Add audit entries for security-relevant actions.
- [ ] Redact PII before AI calls.
- [ ] Add tests for authorization boundaries.

---

_See also: [ARCHITECTURE.md](ARCHITECTURE.md) · [INTEGRATIONS.md](INTEGRATIONS.md) · [CloudProud.md](CloudProud.md)_
