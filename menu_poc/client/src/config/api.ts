// Configuração da API .NET Core
export const API_CONFIG = {
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:5001/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
};

export const API_ENDPOINTS = {
  // Auth
  login: '/auth/login',
  validate: '/auth/validate',
  
  // Menu
  menus: '/menu',
  menusSearch: '/menu/search',
  menuById: (id: number) => `/menu/${id}`,
};
