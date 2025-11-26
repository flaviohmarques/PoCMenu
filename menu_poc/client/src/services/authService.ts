import axios from 'axios';
import { API_CONFIG, API_ENDPOINTS } from '../config/api';

const TOKEN_KEY = 'menu_management_token';
const USERNAME_KEY = 'menu_management_username';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  expiresIn: number;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: Record<string, string[]>;
  timestamp: string;
}

class AuthService {
  private apiClient = axios.create(API_CONFIG);

  async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await this.apiClient.post<ApiResponse<LoginResponse>>(
        API_ENDPOINTS.login,
        credentials
      );

      if (response.data.success && response.data.data) {
        const { token, username } = response.data.data;
        this.setToken(token);
        this.setUsername(username);
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao fazer login');
    } catch (error) {
      if (axios.isAxiosError(error) && error.response) {
        const apiError = error.response.data as ApiResponse<never>;
        throw new Error(apiError.message || 'Erro ao fazer login');
      }
      throw error;
    }
  }

  async validateToken(token: string): Promise<boolean> {
    try {
      const response = await this.apiClient.post<ApiResponse<boolean>>(
        API_ENDPOINTS.validate,
        { token }
      );

      return response.data.success && response.data.data === true;
    } catch {
      return false;
    }
  }

  logout(): void {
    this.removeToken();
    this.removeUsername();
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
  }

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

  getUsername(): string | null {
    return localStorage.getItem(USERNAME_KEY);
  }

  setUsername(username: string): void {
    localStorage.setItem(USERNAME_KEY, username);
  }

  removeUsername(): void {
    localStorage.removeItem(USERNAME_KEY);
  }

  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }
}

export const authService = new AuthService();
