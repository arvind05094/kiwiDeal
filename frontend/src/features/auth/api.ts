import client from '@/shared/api/client';
import type { LoginRequest, RegisterRequest, AuthResponse } from './types';

export const authApi = {
  login: (data: LoginRequest) =>
    client.post<AuthResponse>('/api/v1/auth/login', data),

  register: (data: RegisterRequest) =>
    client.post<AuthResponse>('/api/v1/auth/register', data),

  logout: () =>
    client.post('/api/v1/auth/logout'),
};
