// src/components/ServiceStatus.tsx
import React from "react";

interface ServiceStatusProps {
  name: string;
  isOnline: boolean;
}

const ServiceStatus: React.FC<ServiceStatusProps> = ({ name, isOnline }) => {
  return (
    <div style={{ padding: "10px", color: isOnline ? "green" : "red" }}>
      {name}: {isOnline ? "✅ Online" : "❌ Offline"}
    </div>
  );
};

export default ServiceStatus;
