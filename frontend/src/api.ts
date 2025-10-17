import axios from 'axios';

export const userApi = axios.create({
    baseURL: 'http://users-service:5001',
});

export const orderApi = axios.create({
    baseURL: 'http://orders-service:5002',
});