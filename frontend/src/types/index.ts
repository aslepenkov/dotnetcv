export interface User {
    id: string;
    email: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}

export interface Order {
    id: string;
    userId: string;
    total: number;
    status: string;
    createdAt: string;
}