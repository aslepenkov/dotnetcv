"""
WSGI config for products_service project.

It exposes the WSGI callable as a module-level variable named ``application``.

For more information on this file, see
https://docs.djangoproject.com/en/4.2/howto/deployment/wsgi/
"""

import os

from django.core.wsgi import get_wsgi_application

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'products_service.settings')

# Initialize OpenTelemetry before Django application
from products_service.telemetry import setup_telemetry
setup_telemetry()

application = get_wsgi_application()
