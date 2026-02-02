"""
Views for the products API with Redis caching.
"""
from rest_framework import viewsets, status
from rest_framework.decorators import action
from rest_framework.response import Response
from django.core.cache import cache
from django.db.models import Sum
from django.utils.decorators import method_decorator
from django.views.decorators.cache import cache_page
from django_redis import get_redis_connection
from .models import Product
from .serializers import ProductSerializer, ProductCreateSerializer
import logging

logger = logging.getLogger(__name__)


class ProductViewSet(viewsets.ModelViewSet):
    """
    ViewSet for viewing and editing Product instances.
    Implements Redis caching for GET operations.
    """
    queryset = Product.objects.all()
    serializer_class = ProductSerializer
    
    def get_serializer_class(self):
        """Return appropriate serializer based on action."""
        if self.action == 'create':
            return ProductCreateSerializer
        return ProductSerializer

    def get_queryset(self):
        """Optionally filter products by category or active status."""
        queryset = Product.objects.all()
        category = self.request.query_params.get('category', None)
        is_active = self.request.query_params.get('is_active', None)
        
        if category:
            queryset = queryset.filter(category=category)
        if is_active is not None:
            is_active_bool = is_active.lower() == 'true'
            queryset = queryset.filter(is_active=is_active_bool)
        
        return queryset

    @method_decorator(cache_page(60 * 5))  # Cache for 5 minutes
    def list(self, request, *args, **kwargs):
        """List all products with caching."""
        logger.info("Listing products")
        return super().list(request, *args, **kwargs)

    def retrieve(self, request, *args, **kwargs):
        """Retrieve a single product with caching."""
        instance = self.get_object()
        cache_key = f'product_{instance.id}'
        
        # Try to get from cache
        cached_data = cache.get(cache_key)
        if cached_data:
            logger.info(f"Product {instance.id} retrieved from cache")
            return Response(cached_data)
        
        # If not in cache, serialize and cache it
        serializer = self.get_serializer(instance)
        cache.set(cache_key, serializer.data, timeout=300)  # 5 minutes
        logger.info(f"Product {instance.id} retrieved from database and cached")
        return Response(serializer.data)

    def _invalidate_list_cache(self):
        """Invalidate list cache using Redis pattern matching."""
        try:
            redis_client = get_redis_connection("default")
            # Clear cache version to invalidate all cached views
            cache.clear()
        except Exception as e:
            logger.warning(f"Failed to invalidate cache: {e}")

    def create(self, request, *args, **kwargs):
        """Create a new product and invalidate cache."""
        serializer = self.get_serializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        self.perform_create(serializer)
        
        # Invalidate list cache
        self._invalidate_list_cache()
        logger.info(f"Product {serializer.instance.id} created")
        
        headers = self.get_success_headers(serializer.data)
        return Response(serializer.data, status=status.HTTP_201_CREATED, headers=headers)

    def update(self, request, *args, **kwargs):
        """Update a product and invalidate its cache."""
        partial = kwargs.pop('partial', False)
        instance = self.get_object()
        serializer = self.get_serializer(instance, data=request.data, partial=partial)
        serializer.is_valid(raise_exception=True)
        self.perform_update(serializer)
        
        # Invalidate cache for this product
        cache_key = f'product_{instance.id}'
        cache.delete(cache_key)
        self._invalidate_list_cache()
        logger.info(f"Product {instance.id} updated")
        
        return Response(serializer.data)

    def destroy(self, request, *args, **kwargs):
        """Delete a product and invalidate cache."""
        instance = self.get_object()
        instance_id = instance.id
        
        # Invalidate cache
        cache_key = f'product_{instance_id}'
        cache.delete(cache_key)
        self._invalidate_list_cache()
        
        self.perform_destroy(instance)
        logger.info(f"Product {instance_id} deleted")
        return Response(status=status.HTTP_204_NO_CONTENT)

    @action(detail=False, methods=['get'])
    def stats(self, request):
        """Get product statistics with caching."""
        cache_key = 'products_stats'
        cached_stats = cache.get(cache_key)
        
        if cached_stats:
            logger.info("Product stats retrieved from cache")
            return Response(cached_stats)
        
        stats = {
            'total_products': Product.objects.count(),
            'active_products': Product.objects.filter(is_active=True).count(),
            'total_stock': Product.objects.aggregate(
                total=Sum('stock_quantity')
            )['total'] or 0,
            'categories': list(Product.objects.values_list('category', flat=True).distinct())
        }
        
        cache.set(cache_key, stats, timeout=300)  # 5 minutes
        logger.info("Product stats calculated and cached")
        return Response(stats)
