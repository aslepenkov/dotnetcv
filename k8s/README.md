# Kubernetes Showcase

This directory contains Kubernetes manifests to demonstrate scaling and high availability for the microservices.

## Prerequisites

- A running Kubernetes cluster (e.g., Minikube, Kind, Docker Desktop K8s).
- `kubectl` installed and configured.
- Metrics Server installed in the cluster (required for HPA to work).

## Deployment Steps

1. **Build the Docker images:**
   ```bash
   docker build -t users-service:latest ./backend/users-service
   docker build -t orders-service:latest ./backend/orders-service
   ```
   *(If using Minikube, run `eval $(minikube docker-env)` before building)*

2. **Deploy the infrastructure (Postgres, Redis, RabbitMQ):**
   ```bash
   kubectl apply -f k8s/infrastructure.yaml
   ```

3. **Deploy the microservices:**
   ```bash
   kubectl apply -f k8s/users-service.yaml
   kubectl apply -f k8s/orders-service.yaml
   ```

4. **Enable Auto-scaling:**
   ```bash
   kubectl apply -f k8s/hpa.yaml
   ```

## Demonstrating High Load & Scaling

The `users-service` has a synthetic bottleneck endpoint specifically for this showcase.

1. **Monitor the HPA:**
   ```bash
   kubectl get hpa users-service-hpa --watch
   ```

2. **Simulate high load:**
   In another terminal, port-forward the service:
   ```bash
   kubectl port-forward svc/users-service 5001:5001
   ```
   Then run a loop to hit the bottleneck endpoint:
   ```bash
   while true; do curl "http://localhost:5001/users/bottleneck?iterations=200000000"; done
   ```

3. **Observe Scaling:**
   As the CPU usage increases beyond 50%, the HPA will start spinning up more pods (up to 10). You can see this by watching the HPA status or listing pods:
   ```bash
   kubectl get pods -l app=users-service -w
   ```
