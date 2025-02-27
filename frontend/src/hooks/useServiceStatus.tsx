// src/hooks/useServiceStatus.ts
import { useEffect, useState } from "react";
import { checkHealth } from "../services/healthService";

const services = [
  { name: "Service 1", url: "http://localhost:5001/health" },
  { name: "Service 2", url: "http://localhost:5002/health" },
  { name: "Service 3", url: "http://localhost:5003/health" },
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
