# Products Service

A Django REST Framework microservice for managing products in the microservices architecture demo.

## Overview

This service demonstrates:
- **Django REST Framework** for building RESTful APIs
- **PostgreSQL** integration with Django ORM
- **Redis** caching for performance optimization
- **OpenTelemetry** for distributed tracing
- **OpenAPI/Swagger** documentation
- **Production-ready** deployment with Gunicorn

## Features

- CRUD operations for products
- Redis-based caching for GET operations
- Automatic cache invalidation on updates
- Product statistics endpoint
- Filtering by category and active status
- Health check endpoint
- OpenAPI/Swagger documentation

## API Endpoints

- `GET /api/products/` - List all products (cached)
- `GET /api/products/{id}/` - Get product by ID (cached)
- `POST /api/products/` - Create a new product
- `PUT /api/products/{id}/` - Update a product
- `PATCH /api/products/{id}/` - Partially update a product
- `DELETE /api/products/{id}/` - Delete a product
- `GET /api/products/stats/` - Get product statistics (cached)
- `GET /health` - Health check endpoint
- `GET /api/swagger/` - Swagger UI documentation
- `GET /api/schema/` - OpenAPI schema

## Query Parameters

- `category` - Filter products by category
- `is_active` - Filter by active status (true/false)

## Caching Strategy

- **List operations**: Cached for 5 minutes using Django's cache_page decorator
- **Retrieve operations**: Individual products cached for 5 minutes
- **Statistics**: Cached for 5 minutes
- **Cache invalidation**: Automatic on create, update, or delete operations

## Technology Stack

- Python 3.11
- Django 4.2
- Django REST Framework 3.14
- PostgreSQL (via Django ORM)
- Redis (via django-redis)
- Gunicorn (production server)
- OpenTelemetry (distributed tracing)

## Development

### Local Setup

1. Install dependencies:
```bash
pip install -r requirements.txt
```

2. Set environment variables (see `.env.example`)

3. Run migrations:
```bash
python manage.py migrate
```

4. Create superuser (optional):
```bash
python manage.py createsuperuser
```

5. Run development server:
```bash
python manage.py runserver 0.0.0.0:5003
```

### Docker

The service is configured to run in Docker Compose. See the main project README for instructions.

## Database Schema

### Product Model

- `id` (UUID, Primary Key)
- `name` (String, max 255)
- `description` (Text)
- `price` (Decimal, 10 digits, 2 decimal places)
- `stock_quantity` (Integer)
- `category` (String, max 100)
- `is_active` (Boolean)
- `created_at` (DateTime, auto)
- `updated_at` (DateTime, auto)

## Configuration

Environment variables:
- `SECRET_KEY` - Django secret key
- `DEBUG` - Debug mode (True/False)
- `DB_NAME` - PostgreSQL database name
- `DB_USER` - PostgreSQL username
- `DB_PASSWORD` - PostgreSQL password
- `DB_HOST` - PostgreSQL host
- `DB_PORT` - PostgreSQL port
- `REDIS_URL` - Redis connection URL
- `JAEGER_HOST` - Jaeger host for tracing
- `JAEGER_PORT` - Jaeger port for tracing

## Production Considerations

- Use environment variables for all sensitive configuration
- Set `DEBUG=False` in production
- Use a strong `SECRET_KEY`
- Configure proper CORS origins
- Set up proper logging
- Use connection pooling for database
- Monitor Redis connection health
- Set up health check monitoring
