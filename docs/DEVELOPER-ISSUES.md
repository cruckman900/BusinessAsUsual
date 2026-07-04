# Developer Issues & First-Sprint Tasks

This file contains small, actionable tasks and issues new developers can pick up to make measurable progress toward the AI roadmap. Each item is written so it can be copied into GitHub Issues or assigned directly.

## Sprint 0 — Setup & Contracts (2 weeks)

1. TASK: Add OpenAPI skeleton for CRM module
   - Create `openapi/crm-v1.yaml` with basic CRUD endpoints for leads, opportunities, and customers.
   - Acceptance: `swagger` loads and sample client can be generated.
   - Effort: 1 day

2. TASK: Add Contracts package project
   - Create `BusinessAsUsual.Contracts` class library with DTOs for LeadDto, OpportunityDto, CustomerDto.
   - Acceptance: Web and API projects reference the package.
   - Effort: 2 days

3. TASK: Wire AI.Api to solution build
   - Add `services/AI/AI.Api/AI.Api.csproj` to the solution and ensure it builds.
   - Acceptance: `dotnet build` succeeds for full solution.
   - Effort: 1 day

## Sprint 1 — AI Basics (2–3 weeks)

4. TASK: Implement embeddings upsert endpoint stub
   - Expand `EmbeddingsController.Upsert` to accept typed model and log received items.
   - Acceptance: Example POST returns 200 and items visible in logs.
   - Effort: 1 day

5. TASK: Add dev docker-compose for AI + Qdrant
   - Add `docker-compose.ai.yml` that runs AI.Api and qdrant service for local dev.
   - Acceptance: `docker compose -f docker-compose.ai.yml up` brings both services up.
   - Effort: 2 days

6. TASK: Integrate AiAssistant component into Main app and make calls to AI.Api
   - Ensure assistant toggles and performs a POST to `/api/ai/embeddings/query` when used.
   - Acceptance: Chat box shows placeholder response.
   - Effort: 1 day

## Sprint 2 — RAG Prototype (3 weeks)

7. TASK: Seed vector DB with CRM mock data
   - Write a small utility that loads leads/customers/notes into vector DB with embeddings (use an embedding provider or dummy vectors).
   - Acceptance: Index contains entries with tenantId metadata.
   - Effort: 3 days

8. TASK: Implement semantic query endpoint
   - Implement `/api/ai/embeddings/query` to perform topK retrieval and return sources.
   - Acceptance: Query returns topK sources and scores.
   - Effort: 4 days

## Cross-cutting issues

- ISSUE: Document decision between Qdrant vs FAISS for production. Provide pros/cons in docs/AI-DECISIONS.md.
- ISSUE: Add telemetry hooks to AI.Api (OpenTelemetry + Prometheus exporter) and wire to local Grafana dashboard.

---

Use these items to fill the backlog. Break tasks into small PRs, link to CloudProud.md for context, and reference AI-PriorityQueue.md for prioritization.
