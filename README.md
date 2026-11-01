# ⚡ Recovery Backoffice

---

```bash
dotnet run --project src/Host/Recovery.BackOffice.AppHost   # 🎉 whole stack up: API + 2 React apps + Postgres + Valkey + RustFS
```

> Then open the **Aspire dashboard** at `https://localhost:15888`, the **API + Scalar docs** at `https://localhost:7030/scalar`, the **admin** app at `http://localhost:5173`, and the **dashboard** app at `http://localhost:5174`. Sign in with a seeded demo account (e.g. `admin@acme.com` / `Password123!`).

---

## ✨ What's inside

### Backend — modular monolith, vertical slices
- **.NET 10 · C# latest · Minimal APIs · [Mediator](https://github.com/martinothamar/Mediator) (source-generated CQRS) · FluentValidation**
- **EF Core 10** on **PostgreSQL** (Npgsql), with domain events, the specification pattern, soft-delete + audit interceptors, and tenant-isolated `DbContext`s.
- **JWT auth + ASP.NET Identity** — issuance/refresh, roles & fine-grained permissions, rate-limited auth, password policies, sessions, impersonation.
- **Multitenancy** via [Finbuckle](https://www.finbuckle.com/) — tenant resolution, provisioning, per-tenant migrations & seeding, isolation enforced by default.
- **Cross-cutting**: HybridCache on **Valkey** (Redis-compatible), **Hangfire** jobs, presigned S3 storage (**RustFS** locally), mailing, idempotency, quotas, rate limiting, API versioning, RFC 9457 `ProblemDetails`.
- **Observability**: Serilog structured logging + **OpenTelemetry** traces/metrics/logs, health probes, security/exception auditing.
- **Docs**: **OpenAPI** + the **Scalar** API reference UI.

### Front-ends — two React 19 apps
- **`clients/admin`** (operator console) and **`clients/dashboard`** (tenant app): **React 19 + Vite 7 + TypeScript**, **TanStack Query v5**, **React Router 7**, **Radix + Tailwind v4** (shadcn-style), real-time via **SignalR**/**SSE**.
- Runtime config (`/config.json`, no rebuild per environment), hand-written typed API client, and **Playwright** E2E suites.

### Modules (bounded contexts)
**Identity · Multitenancy · Billing · Catalog · Tickets · Chat · Files · Webhooks · Auditing · Notifications** — each a runtime project plus a `.Contracts` project (its only public surface), boundaries enforced by architecture tests.

### Cloud-native & DevOps
- **.NET Aspire** orchestrates the entire stack locally with one command (Postgres + pgAdmin, Valkey + RedisInsight, RustFS, migrator, demo-seeder, API, and both React apps).
- **Docker Compose** production stack (`deploy/docker`) and **Terraform** for AWS (`deploy/terraform`); API image published to GHCR.
- A one-shot **DbMigrator** (migrations are never run at API startup)

### Quality
**1,600+ backend tests** (xUnit, Shouldly, NSubstitute, AutoFixture, **NetArchTest** boundaries, **Testcontainers** integration) and **200+ front-end E2E tests** (Playwright). Path-scoped CI for backend and frontend; warnings-as-errors.

---

> **Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) · [Docker](https://www.docker.com/) (SQLServer/Valkey/RustFS via Aspire) · [Node 20+](https://nodejs.org/) (for the React apps).

---

## 🧱 Tech stack

| Backend | | Frontend | |
|---|---|---|---|
| Runtime | .NET 10 / C# latest | Framework | React 19 + Vite 7 + TS 5 |
| API | Minimal APIs + Mediator (CQRS) | Data | TanStack Query v5 |
| Validation | FluentValidation | Routing | React Router 7 |
| ORM / DB | EF Core 10 / PostgreSQL | UI | Radix + Tailwind v4 (shadcn) |
| Auth | JWT + ASP.NET Identity | Realtime | SignalR · SSE |
| Multitenancy | Finbuckle 10 | Tests | Playwright |
| Cache / Jobs | Valkey · Hangfire | | |
| Storage | S3 / RustFS (presigned) | **Infra** | |
| Docs | OpenAPI + Scalar | Orchestration | .NET Aspire |
| Observability | Serilog + OpenTelemetry | Deploy | Docker Compose · Terraform |
| Testing | xUnit · Testcontainers · NetArchTest | | |

---

## 🗺️ Repository layout

| Path | What |
|---|---|
| `src/BuildingBlocks/` | Shared framework libraries (Core, Persistence, Web, Caching, Eventing, Storage, Quota…) |
| `src/Modules/{Name}/` | Bounded contexts — each with a runtime project + a `.Contracts` project (its public API) |
| `src/Host/Recovery.BackOffice.Starter.Api` | Composition-root Web API host |
| `src/Host/Recovery.BackOffice.Starter.AppHost` | .NET Aspire orchestrator (Postgres, Valkey, RustFS, migrator, API, both React apps) |
| `src/Host/Recovery.BackOffice.Starter.DbMigrator` | One-shot migrate/seed runner (DB is **not** migrated at API startup) |
| `clients/admin`, `clients/dashboard` | The two React apps |
| `deploy/` | Docker Compose, Terraform (AWS), Dokploy |
| `src/Tests/` | Unit, Architecture (NetArchTest), Integration (Testcontainers) |

---

## ☁️ Deploy

**Single-host via Docker Compose:**

```bash
cd deploy/docker
cp .env.example .env   # new pre-generates this with strong secrets
docker compose up -d --build
```
---

## 🧪 Testing

```bash
dotnet test src/Recovery.BackOffice.Starter.slnx        # backend: unit + architecture + Testcontainers integration
cd clients/admin     && npm run test:e2e # Playwright (operator app)
cd clients/dashboard && npm run test:e2e # Playwright (tenant app)
```

> Integration tests require Docker (Testcontainers spins real Postgres). Architecture tests enforce module boundaries.

---

## 📖 Documentation