import React, { useEffect, useState } from 'react';
import { Button, Grid, List, ListItem, ListItemText, Typography } from '@mui/material';
import { userApi } from '../api/userApi';
import { orderApi } from '../api/orderApi';
import { User } from '../types/user';
import { Order } from '../types/order';

export const Dashboard: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [orders, setOrders] = useState<Order[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const usersResponse = await userApi.getAll();
                setUsers(usersResponse);
                const ordersResponse = await orderApi.getAll();
                setOrders(ordersResponse);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();
    }, []);

    const handleCreateUser = async () => {
        try {
            const newUser = await userApi.create({ email: `user${Date.now()}@example.com`, name: 'New User' });
            setUsers([...users, newUser]);
        } catch (error) {
            console.error('Error creating user:', error);
        }
    };

    const handleCreateOrder = async () => {
        try {
            const newOrder = await orderApi.create({ total: 100, status: 'Pending' });
            setOrders([...orders, newOrder]);
        } catch (error) {
            console.error('Error creating order:', error);
        }
    };

    return (
        <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
                <Typography variant="h6">Users</Typography>
                <Button variant="contained" onClick={handleCreateUser}>Create User</Button>
                <List>
                    {users.map((user) => (
                        <ListItem key={user.id}>
                            <ListItemText primary={user.name} secondary={user.email} />
                        </ListItem>
                    ))}
                </List>
            </Grid>
            <Grid item xs={12} md={6}>
                <Typography variant="h6">Orders</Typography>
                <Button variant="contained" onClick={handleCreateOrder}>Create Order</Button>
                <List>
                    {orders.map((order) => (
                        <ListItem key={order.id}>
                            <ListItemText primary={`Order #${order.id}`} secondary={`Total: ${order.total}, Status: ${order.status}`} />
                        </ListItem>
                    ))}
                </List>
            </Grid>
        </Grid>
    );
};