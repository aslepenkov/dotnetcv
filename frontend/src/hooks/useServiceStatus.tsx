// src/hooks/useServiceStatus.ts
import { useEffect, useState } from "react";
import { checkHealth } from "../services/healthService";

const services = [
  { name: "MicroService1", url: "http://localhost:5001/health" },
  { name: "MicroService2", url: "http://localhost:5002/health" },
  { name: "LocalStack", url: "http://localhost:4566/_localstack/health" },
  {
    name: "PostgreSQL (Lambda)",
    url: "http://localhost:4566/restapis/{your-api-id}/default/_user_request_/postgres-health"
  },
  { name: "Service 4", url: "http://localhost:5004/health" },
  { name: "Service 5", url: "http://localhost:5005/health" },
];

export function useServiceStatus() {
  const [statuses, setStatuses] = useState<Record<string, boolean>>({});

  useEffect(() => {
    async function fetchStatuses() {
      const newStatuses: Record<string, boolean> = {};
      for (const service of services) {
        newStatuses[service.name] = await checkHealth(service.url);
      }
      setStatuses(newStatuses);
    }

    fetchStatuses();
    const interval = setInterval(fetchStatuses, 5000); // Refresh every 5 seconds
    return () => clearInterval(interval);
  }, []);

  return statuses;
}
