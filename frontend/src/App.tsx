import React from "react";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { Box, AppBar, Toolbar, Button } from "@mui/material";
import Sidebar from "./components/Sidebar";
import Users from "./pages/Users";
import Orders from "./pages/Orders";

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
        <AppBar position="static">
          <Toolbar>
            <Button color="inherit" component={Link} to="/users">
              Users
            </Button>
            <Button color="inherit" component={Link} to="/orders">
              Orders
            </Button>
          </Toolbar>
        </AppBar>
        
        <Box sx={{ display: 'flex', flex: 1 }}>
          <Sidebar />
          <Box component="main" sx={{ flex: 1, p: 3 }}>
            <Routes>
              <Route path="/users" element={<Users />} />
              <Route path="/orders" element={<Orders />} />
              <Route path="/" element={<Users />} />
            </Routes>
          </Box>
        </Box>
      </Box>
    </BrowserRouter>
  );
};

export default App;
