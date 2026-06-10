import { orderApi as api } from '../api';
import { Order } from '../types/order';

export const orderApi = {
    getAll: async (): Promise<Order[]> => {
        const response = await api.get('/orders');
        return response.data;
    },
    getById: async (id: string): Promise<Order> => {
        const response = await api.get(`/orders/${id}`);
        return response.data;
    },
    create: async (order: Omit<Order, 'id'>): Promise<Order> => {
        const response = await api.post('/orders', order);
        return response.data;
    },
    update: async (id: string, order: Partial<Order>): Promise<Order> => {
        const response = await api.put(`/orders/${id}`, order);
        return response.data;
    },
    delete: async (id: string): Promise<void> => {
        await api.delete(`/orders/${id}`);
    },
};