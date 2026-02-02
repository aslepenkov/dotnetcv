# Architecture Overview

This document provides a detailed overview of the architecture of the High-Scale Microservices Architecture Demo project.

## Core Design Principles

The architecture is based on the following core design principles:

- **Domain-Driven Design (DDD)**: The system is divided into three main domains: `Users`, `Orders`, and `Products`. Each domain has its own bounded context, which is implemented as a separate microservice. This approach helps to reduce coupling and improve maintainability.
- **Polyglot Architecture**: The system demonstrates technology diversity with .NET services (Users, Orders) and a Python/Django service (Products), showcasing the flexibility of microservices architecture.
- **Event-Driven Architecture (EDA)**: The microservices communicate with each other using an event-driven architecture. This approach helps to improve scalability and resilience, as the services are not directly dependent on each other.
- **Defense in Depth**: The system is designed with multiple layers of defense to protect against failures. This includes health checks, circuit breakers, and failover mechanisms.
- **Zero-Trust Security**: The system is designed with a zero-trust security model, which means that all services are treated as untrusted. This includes service isolation, independent scaling, and secure communication.

## Components

The system is composed of the following components:

- **`users-service`**: A .NET microservice that is responsible for managing users.
- **`orders-service`**: A .NET microservice that is responsible for managing orders.
- **`products-service`**: A Python/Django microservice that is responsible for managing products. Demonstrates polyglot architecture and Django REST Framework patterns.
- **`frontend`**: A React application that provides the user interface for the system.
- **`postgres`**: A PostgreSQL database that is used to store data for the backend services.
- **`redis`**: A Redis cache that is used for distributed caching.
- **`rabbitmq`**: A RabbitMQ message broker that is used for event-driven communication.
- **`jaeger`**: A Jaeger instance that is used for distributed tracing.
- **`loki`**: A Loki instance that is used for log aggregation.
- **`promtail`**: A Promtail instance that is used to collect logs from the containers.
- **`grafana`**: A Grafana instance that is used to visualize logs and metrics.
- **`localstack`**: A LocalStack instance that is used to emulate AWS services for local development.

## Interactions

The components interact with each other in the following ways:

- The `frontend` communicates with the `users-service`, `orders-service`, and `products-service` using RESTful APIs.
- The `users-service` and `orders-service` communicate with each other using an event-driven architecture.
- The `users-service`, `orders-service`, and `products-service` use the `postgres` database to store data.
- The `users-service`, `orders-service`, and `products-service` use the `redis` cache for distributed caching.
- The `users-service` and `orders-service` use the `rabbitmq` message broker for event-driven communication.
- All backend services use the `jaeger` instance for distributed tracing via OpenTelemetry.
- All backend services use the `loki` instance for log aggregation.
- The `promtail` instance collects logs from the containers and sends them to the `loki` instance.
- The `grafana` instance is used to visualize logs and metrics from the `loki` instance.
- The `localstack` instance is used to emulate AWS services for local development.