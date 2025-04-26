import axios from 'axios';
import { User, Order } from '../types';

const users-serviceApi = axios.create({
  baseURL: 'http://localhost:5001/api'
});

const orders-serviceApi = axios.create({
  baseURL: 'http://localhost:5002/api'
});

// User API calls
export const userApi = {
  getUsers: () => users-serviceApi.get<User[]>('/users').then(res => res.data),
  getUser: (id: string) => users-serviceApi.get<User>(`/users/${id}`).then(res => res.data),
  createUser: (data: { email: string; name: string }) => 
    users-serviceApi.post<User>('/users', data).then(res => res.data)
};

// Order API calls
export const orderApi = {
  getOrders: () => orders-serviceApi.get<Order[]>('/orders').then(res => res.data),
  getOrder: (id: string) => orders-serviceApi.get<Order>(`/orders/${id}`).then(res => res.data),
  createOrder: (data: { userId: string; total: number }) => 
    orders-serviceApi.post<Order>('/orders', data).then(res => res.data)
};