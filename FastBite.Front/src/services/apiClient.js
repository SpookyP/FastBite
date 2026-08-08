// src/services/apiClient.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const fetchComToken = async (endpoint, options = {}) => {
    // O oidc-client guarda a sessão com uma chave específica no localStorage. 
    // Para simplificar no react-oidc-context, costumamos ler do user gerido pelo auth:
    const storageString = localStorage.getItem(`oidc.user:https://localhost:PORTA_DO_IDENTITY:NOME_DO_CLIENT`);
    let token = null;
    
    if (storageString) {
        const oidcStorage = JSON.parse(storageString);
        token = oidcStorage?.access_token;
    }

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers
    });

    if (!response.ok) {
        throw new Error(`Erro na API: ${response.status} ${response.statusText}`);
    }

    return response.json();
};