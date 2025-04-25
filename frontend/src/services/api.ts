import axios from 'axios';
import { User, Order } from '../types';

const service1Api = axios.create({
  baseURL: 'http://localhost:5001/api'
});

const service2Api = axios.create({
  baseURL: 'http://localhost:5002/api'
});

// User API calls
export const userApi = {
  getUsers: () => service1Api.get<User[]>('/users').then(res => res.data),
  getUser: (id: string) => service1Api.get<User>(`/users/${id}`).then(res => res.data),
  createUser: (data: { email: string; name: string }) => 
    service1Api.post<User>('/users', data).then(res => res.data)
};

// Order API calls
export const orderApi = {
  getOrders: () => service2Api.get<Order[]>('/orders').then(res => res.data),
  getOrder: (id: string) => service2Api.get<Order>(`/orders/${id}`).then(res => res.data),
  createOrder: (data: { userId: string; total: number }) => 
    service2Api.post<Order>('/orders', data).then(res => res.data)
};