import axios, { AxiosInstance } from 'axios';
import { API_CONFIG, API_ENDPOINTS } from '../config/api';
import { authService, ApiResponse } from './authService';

export interface Menu {
  id: number;
  nome: string;
  ordem: number;
  icone: string;
  descricao?: string;
  status: 'Ativo' | 'Inativo';
  criadoEm: string;
  atualizadoEm: string;
}

export interface CreateMenuDto {
  nome: string;
  ordem: number;
  icone: string;
  descricao?: string;
  status: 'Ativo' | 'Inativo';
}

export interface UpdateMenuDto {
  nome: string;
  ordem: number;
  icone: string;
  descricao?: string;
  status: 'Ativo' | 'Inativo';
}

class MenuService {
  private apiClient: AxiosInstance;

  constructor() {
    this.apiClient = axios.create(API_CONFIG);

    // Interceptor para adicionar token JWT automaticamente
    this.apiClient.interceptors.request.use(
      (config) => {
        const token = authService.getToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Interceptor para tratamento de erros
    this.apiClient.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token expirado ou inválido
          authService.logout();
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  async list(): Promise<Menu[]> {
    try {
      const response = await this.apiClient.get<ApiResponse<Menu[]>>(
        API_ENDPOINTS.menus
      );

      if (response.data.success && response.data.data) {
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao listar menus');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  async search(nome?: string): Promise<Menu[]> {
    try {
      const params = nome ? { nome } : {};
      const response = await this.apiClient.get<ApiResponse<Menu[]>>(
        API_ENDPOINTS.menusSearch,
        { params }
      );

      if (response.data.success && response.data.data) {
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao buscar menus');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  async getById(id: number): Promise<Menu> {
    try {
      const response = await this.apiClient.get<ApiResponse<Menu>>(
        API_ENDPOINTS.menuById(id)
      );

      if (response.data.success && response.data.data) {
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao obter menu');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  async create(menu: CreateMenuDto): Promise<Menu> {
    try {
      const response = await this.apiClient.post<ApiResponse<Menu>>(
        API_ENDPOINTS.menus,
        menu
      );

      if (response.data.success && response.data.data) {
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao criar menu');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  async update(id: number, menu: UpdateMenuDto): Promise<Menu> {
    try {
      const response = await this.apiClient.put<ApiResponse<Menu>>(
        API_ENDPOINTS.menuById(id),
        menu
      );

      if (response.data.success && response.data.data) {
        return response.data.data;
      }

      throw new Error(response.data.message || 'Erro ao atualizar menu');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  async delete(id: number): Promise<boolean> {
    try {
      const response = await this.apiClient.delete<ApiResponse<boolean>>(
        API_ENDPOINTS.menuById(id)
      );

      if (response.data.success) {
        return true;
      }

      throw new Error(response.data.message || 'Erro ao deletar menu');
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  private handleError(error: unknown): void {
    if (axios.isAxiosError(error) && error.response) {
      const apiError = error.response.data as ApiResponse<never>;
      
      // Se houver erros de validação, formatar mensagem
      if (apiError.errors) {
        const errorMessages = Object.entries(apiError.errors)
          .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
          .join('\n');
        console.error('Erros de validação:', errorMessages);
      }
    }
  }
}

export const menuService = new MenuService();
