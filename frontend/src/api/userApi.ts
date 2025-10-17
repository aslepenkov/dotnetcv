import { userApi as api } from '../api';
import { User } from '../types/user';

export const userApi = {
    getAll: async (): Promise<User[]> => {
        const response = await api.get('/users');
        return response.data;
    },
    getById: async (id: string): Promise<User> => {
        const response = await api.get(`/users/${id}`);
        return response.data;
    },
    create: async (user: Omit<User, 'id'>): Promise<User> => {
        const response = await api.post('/users', user);
        return response.data;
    },
    update: async (id: string, user: Partial<User>): Promise<User> => {
        const response = await api.put(`/users/${id}`, user);
        return response.data;
    },
    delete: async (id: string): Promise<void> => {
        await api.delete(`/users/${id}`);
    },
};