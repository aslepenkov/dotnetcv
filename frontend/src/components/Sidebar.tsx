// src/components/Sidebar.tsx
import React from "react";
import { useServiceStatus } from "../hooks/useServiceStatus";
import ServiceStatus from "./ServiceStatus";

const Sidebar: React.FC = () => {
  const statuses = useServiceStatus();

  return (
    <div style={{ width: "250px", borderRight: "2px solid #ddd", padding: "10px" }}>
      <h3>Service Status</h3>
      {Object.entries(statuses).map(([name, isOnline]) => (
        <ServiceStatus key={name} name={name} isOnline={isOnline} />
      ))}
    </div>
  );
};

export default Sidebar;
