# 🚀 High-Scale Microservices Architecture Demo

A production-grade demonstration of cloud-native microservices architecture, implementing industry best practices for building highly scalable, maintainable, and resilient distributed systems.

## 💫 Why This Project?

This project showcases practical implementation of:
- **Scalability Patterns**: Demonstrates horizontal scaling capabilities with stateless services
- **System Design Principles**: SOLID, DRY, CQRS, and Clean Architecture patterns in action
- **Cloud-Native Development**: AWS-ready architecture with local development capabilities
- **Modern Dev Practices**: Infrastructure as Code, CI/CD ready, Containerization
- **Observability First**: Complete monitoring and logging infrastructure built-in

# Architecture Overview

This document provides a comprehensive, in-depth overview of the architecture for the **High-Scale Microservices Architecture Demo** project. It describes the design philosophy, component breakdown, communication patterns, data flow, and operational concerns of the entire system.

---

## Table of Contents

1. [Overview & Design Philosophy](#overview--design-philosophy)
2. [High-Level System Architecture](#high-level-system-architecture)
3. [Core Design Principles](#core-design-principles)
4. [Backend Services](#backend-services)
   - [Users Service (.NET)](#users-service-net)
   - [Orders Service (.NET)](#orders-service-net)
   - [Products Service (Python/Django)](#products-service-pythondjango)
5. [Frontend Application](#frontend-application)
6. [Data & Caching Layer](#data--caching-layer)
7. [Message Broker & Event-Driven Communication](#message-broker--event-driven-communication)
8. [Observability Stack](#observability-stack)
9. [Local Development Cloud Stack (LocalStack)](#local-development-cloud-stack-localstack)
10. [Kubernetes Deployment](#kubernetes-deployment)
11. [Container Orchestration (Docker Compose)](#container-orchestration-docker-compose)
12. [Data Flow & Communication Patterns](#data-flow--communication-patterns)
13. [Security Architecture](#security-architecture)
14. [Technology Stack Summary](#technology-stack-summary)
15. [Access Points & Ports](#access-points--ports)

---

## Overview & Design Philosophy

This project is a **production-grade demonstration** of cloud-native microservices architecture. It showcases how to build, deploy, and operate a system composed of multiple independent services, each responsible for a specific business domain. The design intentionally mixes technologies (polyglot architecture) to demonstrate the flexibility and real-world applicability of microservices.

The system models a simple e-commerce domain:
- **Users**: Customer account management
- **Products**: Product catalog and inventory
- **Orders**: Order processing and lifecycle

---

## High-Level System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                               CLIENT LAYER                                   │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                        React + TypeScript Frontend                     │   │
│  │                  (Port: 5173, Vite Dev Server)                        │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                             SERVICE LAYER                                  │
│                                                                              │
│  ┌─────────────────────┐    ┌─────────────────────┐    ┌───────────────┐ │
│  │   Users Service     │    │   Orders Service    │    │ Products      │ │
│  │   (.NET 9 / ASP.NET │◄──►│   (.NET 9 / ASP.NET │    │ Service       │ │
│  │    Core + EF Core)  │    │    Core + EF Core)  │    │ (Python /     │ │
│  │        Port: 5001   │    │        Port: 5002   │    │  Django DRF)  │ │
│  │        DB:          │◄──►│        DB:          │    │  Port: 5003   │ │
│  │        dotnetcv_    │    │        dotnetcv_    │◄──►│  DB: dotnetcv │ │
│  │        users        │    │        orders       │    │               │ │
│  └──────────┬──────────┘    └──────────┬──────────┘    └───────────────┘ │
│             │                            │                                  │
│             │         Outbox              │                                  │
│             │         Pattern             │                                  │
│             └────────►RabbitMQ◄──────────┘                                  │
└─────────────────────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                             SHARED SERVICES                                  │
│                                                                              │
│   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│   │ PostgreSQL│  │  Redis   │  │ RabbitMQ │  │ LocalStack│  │ OpenTel. │   │
│   │(Primary  │  │(Cache & │  │(Event    │  │(AWS Emul.│  │(Tracing) │   │
│   │ Datastore│  │ Session │  │ Broker)  │  │          │  │          │   │
│   └──────────┘  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         OBSERVABILITY STACK                                │
│                                                                              │
│   ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                        │
│   │   Grafana   │  │    Loki     │  │   Promtail  │                        │
│   │(Dashboards) │  │(Log Storage)│  │(Log Collect)│                        │
│   │  Port: 3000 │  │  Port: 3100 │  │ (Docker)    │                        │
│   └─────────────┘  └─────────────┘  └─────────────┘                        │
│                                                                              │
│   ┌─────────────────────────────────────────────────────────────────┐      │
│   │             Jaeger — Distributed Tracing                         │      │
│   │             (OpenTelemetry / Jaeger Agent / Jaeger UI)           │      │
│   │                          Port: 16686                             │      │
│   └─────────────────────────────────────────────────────────────────┘      │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Core Design Principles

### 1. Domain-Driven Design (DDD)

The system is partitioned into **three bounded contexts**, each mapped to an independent microservice:

| Domain | Service | Responsibility |
|--------|---------|----------------|
| Users | `users-service` | User registration, profiles, authentication |
| Orders | `orders-service` | Order creation, lifecycle management |
| Products | `products-service` | Product catalog, inventory, search |

Each service owns its own data and logic. No service accesses another service's database directly.

### 2. Polyglot Architecture

Technology diversity is intentional and demonstrates real-world flexibility:
- **.NET 9** for high-performance, strongly typed backend services (Users, Orders)
- **Python/Django** for rapid API development with a rich ecosystem (Products)
- **React/TypeScript** for a modern, type-safe frontend
- **PostgreSQL** as the relational data store for all services
- **Redis** for distributed caching and session management
- **RabbitMQ** for reliable event-driven messaging

### 3. Event-Driven Architecture (EDA)

Cross-service communication is **asynchronous and decoupled**. Events are published via the Outbox Pattern to guarantee delivery:

1. A command is processed within a database transaction.
2. An `OutboxMessage` is written to the same transaction.
3. A background `OutboxProcessor` polls and publishes unprocessed messages to RabbitMQ.
4. Other services consume the events and act on them.

This ensures **eventual consistency** without distributed transaction coordination.

### 4. Defense in Depth

Multiple layers of resilience are built in:
- **Health Checks**: Every service exposes `/health` endpoints monitored by Docker and Kubernetes.
- **Circuit Breaker**: Polly circuit breaker on HTTP clients prevents cascading failures.
- **Retry Policies**: Configured for transient failures (database, message broker).
- **Graceful Degradation**: Redis caching provides read fallback if the database is slow.

### 5. Zero-Trust Security

All services are treated as untrusted:
- Service isolation via container boundaries
- No shared database schemas between services
- CORS policies restrict frontend access
- Secrets managed via environment files (not in source control)

---

## Backend Services

### Users Service (.NET)

| Property | Value |
|----------|-------|
| **Framework** | ASP.NET Core 9.0 |
| **Port** | 5001 |
| **Database** | PostgreSQL (`dotnetcv_users`) |
| **ORM** | Entity Framework Core 9.0 |
| **Architecture** | CQRS with MediatR |
| **API Docs** | Swagger/OpenAPI at `/swagger` |
| **Migrations** | FluentMigrator |
| **Caching** | Redis / StackExchange.Redis |
| **Messaging** | MassTransit + RabbitMQ |
| **Observability** | OpenTelemetry + Jaeger |

#### Architecture Layers

```
backend/users-service/
├── Program.cs                          # Entry point, DI registration
├── app.csproj                          # Project dependencies
├── appsettings.json / .Development     # Configuration
│
├── Domain/
│   └── User.cs                         # Domain entity (sealed record, immutability)
│   └── Outbox.cs                       # Outbox message record
│
├── Application/
│   ├── Commands/
│   │   └── CreateUserCommand.cs        # CQRS command + MediatR handler
│   └── Queries/
│       ├── GetUserQuery.cs             # CQRS query
│       └── GetUsersQuery.cs            # CQRS query
│
├── Controllers/
│   └── UsersController.cs              # RESTful API controller
│
└── Infrastructure/
    ├── ServiceCollectionExtensions.cs  # DI module registrations
    ├── ApplicationBuilderExtensions.cs # Middleware pipeline configuration
    ├── UserDbContext.cs                # EF Core DbContext
    ├── Database.cs                     # FluentMigrator runner
    ├── OutboxProcessor.cs              # Background event publisher
    ├── ResiliencePolicies.cs           # Polly circuit breaker config
    └── Migrations/                     # FluentMigrator migration scripts
```

#### Key Patterns
- **CQRS**: Separate read and write models. Commands mutate state and emit events; queries read projections.
- **Outbox Pattern**: Events are persisted atomically with the domain transaction, then published asynchronously.
- **Domain Model**: `User` is an immutable `record` with factory methods (`Create`) and validation.

### Orders Service (.NET)

| Property | Value |
|----------|-------|
| **Framework** | ASP.NET Core 9.0 |
| **Port** | 5002 |
| **Database** | PostgreSQL (`dotnetcv_orders`) |
| **Architecture** | CQRS with MediatR |
| **API Docs** | Swagger/OpenAPI at `/swagger` |
| **Messaging** | MassTransit + RabbitMQ |

The Orders Service mirrors the Users Service in architecture and uses the same infrastructure patterns (Outbox, CQRS, Redis, OpenTelemetry), ensuring consistency across the .NET services. It listens for user events to maintain its own bounded context and ensure eventual consistency.

### Products Service (Python/Django)

| Property | Value |
|----------|-------|
| **Framework** | Django 4.2 + Django REST Framework |
| **Port** | 5003 |
| **Database** | PostgreSQL (`dotnetcv`) |
| **ORM** | Django ORM |
| **API Docs** | DRF Spectacular (OpenAPI) at `/api/swagger/` |
| **Caching** | django-redis (Redis) |
| **Observability** | OpenTelemetry + Jaeger |
| **WSGI Server** | Gunicorn |

#### Architecture

```
backend/products-service/
├── manage.py                           # Django management commands
├── requirements.txt                    # Python dependencies
├── Dockerfile                          # Container build instructions
│
├── products/
│   ├── models.py                       # Product model with DB indexes
│   ├── views.py                        # DRF ViewSet with Redis caching
│   ├── serializers.py                  # Model serializers (read + write)
│   ├── urls.py                         # URL routing for the API
│   └── migrations/                     # Django migration files
│
└── products_service/
    ├── settings.py                     # Django configuration
    ├── urls.py                         # Root URL configuration
    ├── wsgi.py                         # WSGI application entry point
    ├── asgi.py                         # ASGI entry
    └── telemetry.py                    # OpenTelemetry instrumentation
```

#### Key Features
- **Redis Caching**: Per-object and list caching with cache invalidation on writes.
- **Django ViewSets**: Follows RESTful conventions (`list`, `retrieve`, `create`, `update`, `destroy`).
- **Custom Action**: `/api/stats/` endpoint for aggregate product metrics.

---

## Frontend Application

| Property | Value |
|----------|-------|
| **Framework** | React 19 + TypeScript |
| **Build Tool** | Vite 6 |
| **UI Library** | Material-UI (MUI) v5 |
| **HTTP Client** | Axios |
| **Routing** | React Router v7 |
| **Port** | 5173 |

### Architecture

```
frontend/src/
├── App.tsx                            # Root component with router
├── main.tsx                           # React DOM entry
├── index.css / App.css                # Global styles
├── vite-env.d.ts                      # Vite type definitions
│
├── api/
│   ├── userApi.ts                     # Users API client functions
│   ├── orderApi.ts                    # Orders API client functions
│   └── api.ts                         # Axios instance configuration
│
├── components/
│   ├── Layout.tsx                     # MUI drawer + app bar layout
│   ├── ServiceStatus.tsx              # Health status indicator component
│   └── Sidebar.tsx                    # Navigation sidebar
│
├── pages/
│   ├── Dashboard.tsx                  # Main view - lists users & orders
│   ├── Users.tsx                      # Users management page
│   └── Orders.tsx                     # Orders management page
│
├── hooks/
│   └── useServiceStatus.ts            # Real-time health polling hook
│
├── services/
│   ├── api.ts                         # API client helpers
│   └── healthService.ts               # Health check logic
│
└── types/
    ├── index.ts                       # Shared type definitions
    ├── user.ts                        # User entity type
    └── order.ts                       # Order entity type
```

### Health Dashboard

The frontend includes a real-time health monitoring dashboard (via the `useServiceStatus` hook) that polls all backend services every 5 seconds, displaying green/red status indicators for:
- Users Service (localhost:5001)
- Orders Service (localhost:5002)
- LocalStack (localhost:4566)
- Lambda-based PostgreSQL health endpoint
- Future services (ports 5004, 5005)

---

## Data & Caching Layer

### PostgreSQL (Primary Data Store)

- **Image**: `postgres:latest`
- **Port**: `5432`
- **Shared Infrastructure**: All services use the same PostgreSQL instance but with **separate databases** to maintain isolation:
  - `dotnetcv` — used by the Products Service
  - `dotnetcv_users` — used by the Users Service
  - `dotnetcv_orders` — used by the Orders Service
- **Initialization**: `backend/postgres-init/01-create-databases.sql` runs on container startup.
- **Management UI**: PgWeb (`localhost:8081`) provides a web-based database browser.

### Redis (Distributed Cache)

- **Image**: `redis:latest`
- **Port**: `6379`
- **Uses**:
  - .NET services: `IConnectionMultiplexer` for key-value caching
  - Django service: `django-redis` for per-object and list caching
  - Session management (Django)
- **Cache Invalidation**: Implemented in the Products Service — cache is cleared on any write operation to maintain consistency.

---

## Message Broker & Event-Driven Communication

### RabbitMQ

- **Image**: `rabbitmq:3-management`
- **AMQP Port**: `5672` (service-to-service)
- **Management UI**: `15672`
- **Credentials**: `guest` / `guest`

### Outbox → RabbitMQ → Consumer Flow

```
┌────────────┐      ┌──────────────┐      ┌──────────┐      ┌────────────┐
│  Command   │      │  Save to DB  │      │  Outbox  │      │  Publish   │
│  Handler   │─────►│ + Outbox msg │─────►│ Processor│─────►│ to Rabbit │
│            │ TX   │              │      │ (poll)   │      │   MQ      │
└────────────┘      └──────────────┘      └──────────┘      └────────────┘
                                                                   │
                                                                   ▼
                                                            ┌────────────┐
                                                            │ Consumer   │
                                                            │ (Orders    │
                                                            │  Service)  │
                                                            └────────────┘
```

### Message Publishing via MassTransit

Both .NET services register MassTransit with the RabbitMQ transport:

```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString("RabbitMQ"));
    });
});
```

### OutboxProcessor

A hosted background service (`OutboxProcessor`) polls the `OutboxMessages` table every 10 seconds:
1. Finds messages where `ProcessedAt IS NULL`
2. Deserializes the message payload using `Newtonsoft.Json`
3. Publishes via MassTransit to RabbitMQ
4. Marks the message as processed

This guarantees:
- **Atomicity**: The domain write and event write happen in the same DB transaction.
- **At-least-once delivery**: Failed publishes are retried on the next poll cycle.
- **Idempotency**: Consumers must handle duplicate events gracefully.

---

## Observability Stack

### Distributed Tracing — Jaeger

- **Image**: `jaegertracing/all-in-one:latest`
- **UI Port**: `16686`
- **Protocol**: OpenTelemetry with Jaeger UDP agent exporter

Both .NET services instrument via OpenTelemetry:
- `AddAspNetCoreInstrumentation()` — HTTP request/response tracing
- `AddHttpClientInstrumentation()` — Outgoing HTTP call tracing
- `AddJaegerExporter()` — Exports spans to the Jaeger agent

### Log Aggregation — Loki + Promtail

- **Loki** (`grafana/loki:latest`, port `3100`): Centralized log storage and indexing.
- **Promtail** (`grafana/promtail:latest`): Log collection agent deployed as a sidecar (Docker container). Mounts `/var/lib/docker/containers` and `/var/run/docker.sock` to tail container logs and push them to Loki.
- **Config**: `localstack/promtail-config.yml`

### Metrics & Dashboards — Grafana

- **Image**: `grafana/grafana:latest`
- **Port**: `3000`
- **Provisioning**: Dashboard and datasource definitions in `grafana/provisioning/`
- **Data Sources**: Loki (logs), can be extended with Prometheus.
- **Use Cases**:
  - Service health overviews
  - Error rate and latency dashboards
  - Business metrics (order volume, user activity)

---

## Local Development Cloud Stack (LocalStack)

LocalStack emulates AWS services locally for testing cloud integration without cloud costs.

- **Image**: `localstack/localstack:latest`
- **Port**: `4566` (unified endpoint for all AWS services)
- **Emulated Services**: S3, SNS, SQS, Lambda, SSM, API Gateway, IAM, CloudWatch Logs
- **Persistence**: Enabled (`PERSISTENCE=1`) for state across restarts

### Lambda Function: `PostgresHealthLambda`

- **Runtime**: .NET 8 (deployed via `dotnet8` runtime in LocalStack)
- **Location**: `lambda/PostgresHealthLambda/`
- **Functionality**:
  - Reads a PostgreSQL connection string from **AWS SSM Parameter Store** (`/config/postgres/connection-string`).
  - Connects to the database and returns a health status.
- **Security**: SecureString type for the SSM parameter (WithDecryption).
- **Trigger**: Exposed via **API Gateway** at `/postgres-health`.

### LocalStack Initialization

`localstack/init-localstack.sh` runs on container startup and:
1. Creates an IAM execution role for Lambda.
2. Deploys the `PostgresHealthLambda` function from a pre-built ZIP.
3. Stores the database connection string in SSM Parameter Store.
4. Creates an API Gateway REST API with a `GET /postgres-health` endpoint.
5. Links the API Gateway endpoint to the Lambda function.
6. Grants API Gateway permission to invoke the Lambda.

---

## Kubernetes Deployment

The `k8s/` directory contains Kubernetes manifests for production-style deployment:

| File | Description |
|------|-------------|
| `infrastructure.yaml` | Deployments and Services for PostgreSQL, Redis, RabbitMQ |
| `users-service.yaml` | Deployment + Service for the Users Service (port 5001) |
| `orders-service.yaml` | Deployment + Service for the Orders Service (port 5002) |
| `hpa.yaml` | Horizontal Pod Autoscaler (HPA) targeting the Users Service |

### HPA Configuration

```yaml
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: users-service
  minReplicas: 1
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 50
```

The Users Service includes a synthetic `/bottleneck` endpoint to simulate high CPU load for testing HPA scaling behavior when `ENABLE_BOTTLENECK=true`.

### Service Health Probes

Every containerized service exposes Kubernetes-ready health probes:
- **readinessProbe**: `/health`, initial delay 10s, period 5s
- **livenessProbe**: `/health`, initial delay 15s, period 10s

---

## Container Orchestration (Docker Compose)

The entire system is defined in `docker-compose.yml` for local development. All services are connected on the default Docker bridge network.

### Service Dependency Graph

```
                    ┌────────────────────────────────────────────┐
                    │                LocalStack                   │
                    │  (S3, SNS, SQS, Lambda, API Gateway, SSM)   │
                    └─────────────────────────────────────────────┘
                                      │
    ┌──────────┐    ┌──────────┐     │      ┌──────────┐    ┌──────────┐
    │  Users   │◄──►│  Orders  │◄────┼─────►│ Products │    │  Redis   │
    │ Service  │    │ Service  │     │      │ Service  │    │ (Cache)  │
    │   5001   │    │   5002   │     │      │   5003   │    │  6379    │
    └────┬─────┘    └────┬─────┘     │      └────┬─────┘    └──────────┘
         │               │           │             │
         └─────┬─────────┘           │             │
               │                     │             │
               ▼                     ▼             ▼
         ┌──────────┐         ┌──────────┐  ┌──────────┐
         │ RabbitMQ │         │ PostgreSQL│  │  Jaeger  │
         │ 5672/    │         │ (3 dbs)  │  │ 16686    │
         │ 15672    │         │   5432   │  └──────────┘
         └──────────┘         └──────────┘
                                     │
                         ┌──────────┘
                         ▼
                   ┌───────────┐
                   │ PgWeb     │
                   │ 8081      │
                   └───────────┘
```

### Frontend Service (Vite Dev Server)

- Hot-reload development via `npm run dev -- --host`
- Volume mounts the `frontend/` directory for live file editing
- CORS pre-configured in backend services for `http://localhost:5173`

---

## Data Flow & Communication Patterns

### 1. Synchronous (REST API)

The frontend communicates with all backend services directly via RESTful HTTP APIs.

| Frontend → Users API | Frontend → Orders API | Frontend → Products API |
|---|---|---|
| `GET /users` | `GET /orders` | `GET /api/products/` |
| `POST /users` | `POST /orders` | `POST /api/products/` |
| `GET /users/{id}` | `GET /orders/{id}` | `GET /api/products/{id}/` |

### 2. Asynchronous (Event-Driven)

When a user is created:

```
┌───────────────┐     ┌──────────────────┐     ┌────────────────┐
│ Users Service │     │   RabbitMQ       │     │ Orders Service │
│               │     │                  │     │                │
│ CreateUser()  │────►│ UserCreatedEvent │───►│ Consumer       │
│ Save to DB    │     │                  │     │ Projection     │
│ Outbox entry  │     │                  │     │                │
└───────────────┘     └──────────────────┘     └────────────────┘
```

Benefits:
- Services remain independent despite data needs.
- Scalable: consumers can be scaled independently.
- Resilient: RabbitMQ acts as a durable buffer during outages.

---

## Security Architecture

### Service Isolation
- Each service runs in its own Docker container.
- Services communicate over Docker network (isolated from host by default).
- No cross-service database access.

### Authentication / Authorization
- **JWT-ready**: Infrastructure is prepared for JWT-based authentication.
- **CORS**: Strictly configured to allow only `http://localhost:5173`. Both .NET services and Django validate origins.

### Secrets Management
- **`.env` files**: `users-service/secrets.env` and `orders-service/secrets.env` (ignored by Git) hold connection strings and credentials.
- **SSM Parameter Store**: Used by the Lambda function to retrieve the database connection string securely.

### Resilience
- **Circuit Breaker**: Polly circuit breaker on `HttpClient` — trips after 5 consecutive failures, resets after 30 seconds.
- **Graceful Degradation**: Redis cache provides stale data fallback if the database is unreachable.

---

## Technology Stack Summary

| Category | Technology |
|----------|------------|
| **Backend (.NET)** | .NET 9, ASP.NET Core, Entity Framework Core 9, PostgreSQL (Npgsql), MassTransit, MediatR, Polly, Redis (StackExchange), OpenTelemetry, FluentMigrator, Swagger |
| **Backend (Python)** | Python 3.11, Django 4.2, Django REST Framework, django-redis, drf-spectacular, OpenTelemetry, Gunicorn, psycopg2 |
| **Frontend** | React 19, TypeScript, Vite 6, Material-UI v5, Axios, React Router v7 |
| **Messaging** | RabbitMQ 3 (Management Plugin) |
| **Database** | PostgreSQL 16+ |
| **Cache** | Redis 7+ |
| **Observability** | Jaeger, Loki, Promtail, Grafana, OpenTelemetry |
| **Cloud (Local)** | LocalStack (S3, SQS, SNS, Lambda, API Gateway, IAM, SSM, CloudWatch) |
| **Containerization** | Docker, Docker Compose |
| **Orchestration** | Kubernetes (manifests in `k8s/`) |
| **CI/CD Ready** | GitHub Actions (workflows defined) |
| **Testing** | xUnit (C#), Jest/Cypress (frontend) |

---

## Access Points & Ports

| Service | URL | Port | Description |
|---------|-----|------|-------------|
| Frontend | `http://localhost:5173` | 5173 | React + Vite dev server |
| Users API | `http://localhost:5001/swagger` | 5001 | .NET REST API documentation |
| Orders API | `http://localhost:5002/swagger` | 5002 | .NET REST API documentation |
| Products API | `http://localhost:5003/api/swagger/` | 5003 | DRF Spectacular documentation |
| Grafana | `http://localhost:3000` | 3000 | Metrics & log dashboards |
| Jaeger UI | `http://localhost:16686` | 16686 | Distributed tracing viewer |
| RabbitMQ Mgmt | `http://localhost:15672` | 15672 | Message broker management |
| PgWeb | `http://localhost:8081` | 8081 | PostgreSQL web UI |
| LocalStack | `http://localhost:4566` | 4566 | AWS service emulator |
| Loki | `http://localhost:3100` | 3100 | Log storage API |

---

## Development Workflow

### Quick Start

```bash
# 1. Clone the repository
git clone <repo-url> && cd dotnetcv

# 2. Start all services
docker compose up -d

# 3. Open the application
#    - Frontend:         http://localhost:5173
#    - Users API Docs:   http://localhost:5001/swagger
#    - Orders API Docs:  http://localhost:5002/swagger
#    - Products API:     http://localhost:5003/api/swagger/
#    - Grafana:          http://localhost:3000
#    - Jaeger:           http://localhost:16686
#    - RabbitMQ:         http://localhost:15672
#    - PgWeb:            http://localhost:8081
```

### Project Structure

```
dotnetcv/
├── ARCHITECTURE.md                          # This document
├── readme.md                                # Project overview & quick start
├── docker-compose.yml                       # All local services definition
├── dotnetcv.sln                             # Visual Studio solution file
│
├── backend/
│   ├── postgres-init/01-create-databases.sql # Database initialization
│   │
│   ├── users-service/                       # .NET 9 microservice
│   │   ├── Application/                   # CQRS commands & queries
│   │   ├── Controllers/                     # REST API controllers
│   │   ├── Domain/                          # Domain entities & events
│   │   ├── Infrastructure/                  # DbContext, messaging, resilience
│   │   └── Migrations/                      # FluentMigrator scripts
│   │
│   ├── orders-service/                      # .NET 9 microservice (mirrors users)
│   │   └── ...
│   │
│   └── products-service/                    # Python/Django microservice
│       ├── products/                        # Models, views, serializers
│       ├── products_service/                # Django project configuration
│       └── manage.py                        # Django CLI
│
├── frontend/                                # React + TypeScript application
│   └── src/
│       ├── api/                             # API client functions
│       ├── components/                      # Reusable UI components
│       ├── hooks/                           # Custom React hooks
│       ├── pages/                           # Page-level components
│       ├── services/                        # Business logic / API wrappers
│       └── types/                           # TypeScript interfaces
│
├── k8s/                                     # Kubernetes deployment manifests
│   ├── users-service.yaml
│   ├── orders-service.yaml
│   ├── hpa.yaml
│   └── infrastructure.yaml
│
├── lambda/                                  # AWS Lambda functions
│   └── PostgresHealthLambda/
│       ├── src/PostgresHealthLambda/        # .NET Lambda source code
│       └── test/PostgresHealthLambda.Tests/ # Unit tests
│
├── localstack/                              # LocalStack configuration
│   ├── init-localstack.sh                   # Service initialization
│   ├── init-gatewayapi.sh                   # API Gateway setup
│   └── promtail-config.yml                  # Log shipping configuration
│
└── grafana/                                 # Grafana provisioning
    └── provisioning/
```

---

*This architecture document was crafted to serve as a comprehensive reference for developers, operators, and architects working with or extending this microservices platform. For the latest changes, consult the repository and inline source code comments.*
