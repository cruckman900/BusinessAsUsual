# AI Priority Queue — Business As Usual

This file lists AI initiatives ordered by priority. Each item includes goal, deliverables, acceptance criteria, estimated effort (S,M,L), and dependencies. Use this as the single source of truth for assigning work to teams and scheduling sprints.

Priority queue (1 = highest)

1) SmartCRM Assistant (RAG chat)
- Goal: Provide conversational access to CRM data and docs (answers + sources)
- Deliverables: /api/ai/query, vector index seeded with CRM data, Blazor <AiChat/> component, caching & rate-limits
- Acceptance: 80% accurate answers on canned queries; source links shown; opt-out per tenant
- Effort: M (2–3 sprints)
- Dependencies: AI microservice, embeddings library, vector DB

2) Summarization (Lead/Activity summaries)
- Goal: One-click summaries of lead activities, notes, and meeting transcripts
- Deliverables: /api/ai/summarize, UI button on Lead page, short bullet results
- Acceptance: Summaries under 6 bullets, judged useful by QA
- Effort: S (1 sprint)
- Dependencies: AI microservice

3) Smart Search (semantic + typed results)
- Goal: Replace keyword search with semantic search across docs, leads, and customers
- Deliverables: /api/search, vector index integration, UI result ranking
- Acceptance: Improved click-through and lower search refinements
- Effort: M (2 sprints)
- Dependencies: Vector DB, embeddings pipeline

4) Lead Scoring & Next-Best-Action
- Goal: Predict lead conversion probability and recommend next actions
- Deliverables: /api/ai/predict/lead-score, UI score in lead card, recommended action suggestions
- Acceptance: measurable lift in conversion in A/B test
- Effort: M/L (3–6 sprints)
- Dependencies: Historical data, model training pipeline

5) Auto-Tagging & Enrichment
- Goal: Auto-populate company details, tags, and classifications for leads/customers
- Deliverables: enrichment pipeline, API integration with external enrichment services, UI tags
- Acceptance: ≥70% enrichment correctness for demo dataset
- Effort: M
- Dependencies: External enrichment APIs (or local heuristics)

6) Workflow Automation / Rules Engine
- Goal: Enable non-devs to automate tasks (email, assign, create task) based on triggers
- Deliverables: rule UI, runtime executor, durable queue integration
- Acceptance: Create and run a rule that auto-assigns leads based on tags
- Effort: L
- Dependencies: Background jobs, event bus

7) Conversational Assistant with Actions (Human-in-the-loop)
- Goal: Allow chat to propose data-changing actions that require explicit confirmation
- Deliverables: Chat UI action cards, confirm flow, audit logging
- Acceptance: All data-changing actions require confirmation; audit trail created
- Effort: M
- Dependencies: SmartCRM Assistant

8) Mobile AI Assistant (MAUI)
- Goal: Mobile-first assistant with offline read and queued writes
- Deliverables: MAUI prototype, offline SQLite store, sync queue
- Acceptance: Create lead offline and sync without duplication
- Effort: L
- Dependencies: Contracts package, mobile auth

9) Analytics & Forecasting (Sales)
- Goal: Provide pipeline forecasting and historical analytics
- Deliverables: analytics pipeline, scheduled training, dashboards
- Acceptance: Forecast accuracy against test set; dashboard widgets
- Effort: L
- Dependencies: ETL pipeline, reporting DB

10) Governance, Monitoring & Cost Controls
- Goal: Track model usage, costs, and provide governance (model cards, drift alerts)
- Deliverables: telemetry, usage dashboards, model versioning, feedback endpoints
- Acceptance: Per-tenant cost tracking and throttling
- Effort: M
- Dependencies: Telemetry stack (Prometheus, Grafana), logging

Usage
- The product manager assigns items from this queue to sprints; items may be split into sub-tasks and moved between priority positions.
- For quorum: team lead + product manager must approve re-prioritization.
