// src/App.tsx
import React from "react";
import Sidebar from "./components/Sidebar";

const App: React.FC = () => {
  return (
    <div style={{ display: "flex" }}>
      <Sidebar />
      <div style={{ padding: "20px", flex: 1 }}>
        <h1>Welcome to DotnetCV Frontend</h1>
      </div>
    </div>
  );
};

export default App;
