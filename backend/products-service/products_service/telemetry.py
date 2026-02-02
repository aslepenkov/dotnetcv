"""
OpenTelemetry configuration for distributed tracing.
"""
import os
from opentelemetry import trace
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor
from opentelemetry.exporter.jaeger.thrift import JaegerExporter
from opentelemetry.instrumentation.django import DjangoInstrumentor
from opentelemetry.instrumentation.requests import RequestsInstrumentor
from opentelemetry.sdk.resources import Resource

# Note: opentelemetry-exporter-jaeger uses thrift protocol
# For newer versions, consider using OTLP exporter instead


def setup_telemetry():
    """Initialize OpenTelemetry for distributed tracing."""
    try:
        jaeger_host = os.getenv('JAEGER_HOST', 'jaeger')
        jaeger_port = int(os.getenv('JAEGER_PORT', '6831'))
        
        # Create resource
        resource = Resource.create({
            "service.name": "products-service",
            "service.version": "1.0.0",
        })
        
        # Set up tracer provider
        trace.set_tracer_provider(TracerProvider(resource=resource))
        
        # Configure Jaeger exporter
        jaeger_exporter = JaegerExporter(
            agent_host_name=jaeger_host,
            agent_port=jaeger_port,
        )
        
        # Add span processor
        span_processor = BatchSpanProcessor(jaeger_exporter)
        trace.get_tracer_provider().add_span_processor(span_processor)
        
        # Instrument Django
        DjangoInstrumentor().instrument()
        
        # Instrument requests if available
        try:
            RequestsInstrumentor().instrument()
        except Exception:
            # Requests instrumentation is optional
            pass
    except Exception as e:
        # Log error but don't fail startup if telemetry fails
        import logging
        logger = logging.getLogger(__name__)
        logger.warning(f"Failed to setup telemetry: {e}")
